using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security;
using StarSmuggler.Events;


namespace StarSmuggler
{
    public class GameManager
    {
        public static GameManager Instance { get; private set; } = new GameManager();

        public PlayerData Player { get; private set; }
        public GameState CurrentState { get; private set; }
        public Port CurrentPort => Player.CurrentPort;
        public Dictionary<string, Dictionary<string, int>> CurrentPrices => Player.CurrentPrices;
        private GameEvent lastEvent;
        private GameState? previousState;
        private GameManager() { }
        public int JumpsSinceLastUpdate => Player.JumpsSinceLastUpdate;

        public void CheckForGameOver()
        {
            var player = Player;
            var port = player.CurrentPort;

            if (player.Credits >= 50)
                return; // Player can still travel

            // Try to find if cargo is sellable for enough to earn 50
            int potentialRevenue = 0;

            foreach (var item in player.CargoHold)
            {
                if (port.AvailableItems.Contains(item.Key))
                {
                    var marketPrice = port.AvailableItems.Find(g => g == item.Key).BasePrice;
                    potentialRevenue += item.Value * marketPrice;

                    if (potentialRevenue >= 50)
                        return; // Can sell enough to continue
                }
            }

            // No way to travel or make money → game over
            SetGameState(GameState.GameOver);
        }

        public int GetTravelCost(Port from, Port to)
        {
            if (from == to)
                return 0;

            int baseCost = 50;

            // Add cost based on zone difference
            int zoneDiff = Math.Abs((int)from.Zone - (int)to.Zone);
            int cost = baseCost + zoneDiff * 25;

            if (zoneDiff >= 2)
                cost += 50;

            return cost;
        }

        public void LoadGame()
        {
            if (SaveLoadManager.TryLoadGame(out var data))
            {
                Player = new PlayerData(data.Credits, data.CargoLimit);

                var port = PortsDatabase.AllPorts.Find(p => p.Name == data.CurrentPortName);
                if (port == null) port = PortsDatabase.GetRandomInnerPort();

                Player.CurrentPort = port;
                Player.CurrentPrices = data.Prices;

                foreach (var pair in data.CargoHold)
                {
                    var item = ItemsDatabase.AllItems.Find(g => g.Name == pair.Key);
                    if (item != null)
                        Player.CargoHold[item] = pair.Value;
                }

                LoadGoodsForCurrentPort();
                SetGameState(GameState.PortOverview); 
            }
            else
            {
                StartNewGame();
            }
        }

        public void StartNewGame()
        {
            var startingPort = PortsDatabase.GetRandomInnerPort();
            Player = new PlayerData(startingCredits: 500, cargoLimit: 30);
            Player.CurrentPort = startingPort;
            Player.CurrentPrices = new Dictionary<string, Dictionary<string, int>>();
            Player.JumpsSinceLastUpdate = 0;
            UpdatePrices(PortsDatabase.AllPorts, ItemsDatabase.AllItems);
            // DataManager.SaveGameData(DataManager.SavePath, GameData);
            LoadGoodsForCurrentPort();
            SetGameState(GameState.PortOverview); 
        }

        public void TravelToPort(Port destination, int cost)
        {
            if (Player.Credits >= cost)
            {
                Console.WriteLine($"Current port: {Player.CurrentPort.Name}");
                Player.Credits -= cost;
                Player.CurrentPort = destination;
                Console.WriteLine($"Destination port: {Player.CurrentPort.Name}");
                LoadGoodsForCurrentPort();
                Player.JumpsSinceLastUpdate++;
                UpdatePrices(PortsDatabase.AllPorts, ItemsDatabase.AllItems);
                SetGameState(GameState.PortOverview); 
                TriggerRandomEvent();
                SaveLoadManager.SaveGame(Player);
                Instance.CheckForGameOver();

            }
            else
            {
                // Handle insufficient funds
                // (e.g., show warning or disable option in UI)
            }
        }

        public void LoadGoodsForCurrentPort()
        {
            var available = ItemsDatabase.GetCommonAndMidTier(6);
            Console.WriteLine($"GM Loading goods for port: {Player.CurrentPort.Name}");
            foreach (var good in available)
            {
                Console.WriteLine($"Available Good: {good.Name}");
            }
            Player.CurrentPort.AvailableItems = available;
            Player.CurrentPort.Prices = CurrentPrices[Player.CurrentPort.Id];
        }

        public void SetGameState(GameState newState)
        {
            if (CurrentState != GameState.MainMenu)
                previousState = CurrentState;
            CurrentState = newState;
            if (Game1.ScreenManagerRef != null)
                Game1.ScreenManagerRef.SetActive(newState);
        }

        private void TriggerRandomEvent()
        {
            var rng = new Random();
            rng.Next(1, 100);
            if (rng.Next(1, 100) > 25) // 25% chance to trigger an event
                return; // No event triggered
            lastEvent = EventDatabase.AllEvents[rng.Next(EventDatabase.AllEvents.Count)];
            lastEvent.Execute(Player, Player.CurrentPort);
        }
        public GameEvent GetLastEvent()
        {
            return lastEvent;
        }

        public bool HasPreviousState()
        {
            return previousState.HasValue;
        }

        public void ReturnToPreviousState()
        {
            if (previousState.HasValue)
            {
                SetGameState(previousState.Value);
                previousState = null;
            }
        }

        public void UpdatePrices(List<Port> ports, List<Item> items)
        {
            var rng = new Random();
            if ((JumpsSinceLastUpdate > 3) || JumpsSinceLastUpdate == 0)
            {
                Console.WriteLine("Updating prices due to enough jumps since last update.");
                foreach (var port in ports)
                {
                    if (!CurrentPrices.ContainsKey(port.Id))
                        CurrentPrices[port.Id] = new Dictionary<string, int>();
                    foreach (var item in items)
                    {
                        float variance = 0.3f;
                        float multiplier = 1f + ((float)(rng.NextDouble() * 2 - 1) * variance);
                        Console.WriteLine($"Base price for {item.Name} at {port.Name}: {item.BasePrice} credits, Multiplier: {multiplier}");
                        int price = Math.Max(1, (int)(item.BasePrice * multiplier));
                        CurrentPrices[port.Id][item.Id] = price;
                        Console.WriteLine($"Updated price for {item.Name} at {port.Name}: {price} credits");
                    }
                }
                Player.JumpsSinceLastUpdate = 0; // Reset jump counter after updating prices
            } else {
                Console.WriteLine("Skipping price update due to insufficient jumps since last update.");
                return;
            }
        }
    }
}
