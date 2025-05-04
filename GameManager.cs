using System;
using System.Collections.Generic;
using StarSmuggler.Events;

namespace StarSmuggler
{
    public class GameManager
    {
        public static GameManager Instance { get; private set; } = new GameManager();

        public PlayerData Player { get; private set; }
        public GameState CurrentState { get; private set; }
        public Port CurrentPort => Player.CurrentPort;
        private GameEvent lastEvent;
        private GameState? previousState;
    


        private GameManager() { }

        public void CheckForGameOver()
        {
            var player = Player;
            var port = player.CurrentPort;

            if (player.Credits >= 50)
                return; // Player can still travel

            // Try to find if cargo is sellable for enough to earn 50
            int potentialRevenue = 0;

            foreach (var good in player.CargoHold)
            {
                if (port.AvailableGoods.Contains(good.Key))
                {
                    var marketPrice = port.AvailableGoods.Find(g => g == good.Key).BasePrice;
                    potentialRevenue += good.Value * marketPrice;

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

                foreach (var pair in data.CargoHold)
                {
                    var good = GoodsDatabase.AllGoods.Find(g => g.Name == pair.Key);
                    if (good != null)
                        Player.CargoHold[good] = pair.Value;
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
                SetGameState(GameState.PortOverview); 
                TriggerRandomEvent();
                SaveLoadManager.SaveGame(Player);
                GameManager.Instance.CheckForGameOver();
                // Autosave after successful travel

            }
            else
            {
                // Handle insufficient funds
                // (e.g., show warning or disable option in UI)
            }
        }

        public void LoadGoodsForCurrentPort()
        {
            var available = GoodsDatabase.GetCommonAndMidTier(6);
            Console.WriteLine($"GM Loading goods for port: {Player.CurrentPort.Name}");
            foreach (var good in available)
            {
                Console.WriteLine($"Available Good: {good.Name}");
            }
            Player.CurrentPort.AvailableGoods = available;
            GenerateMarketPrices();
        }

        public void GenerateMarketPrices()
        {
            Console.WriteLine($"GM Generating market prices for port: {Player.CurrentPort.Name}");
            Player.CurrentPort.CurrentPrices.Clear();
            
            foreach (var good in Player.CurrentPort.AvailableGoods)
            {
                // Allow +/- 30% price swing
                Console.WriteLine($"GM Generating market prices for : {good.Name}");
                float variance = 0.3f;
                float multiplier = 1f + RandomHelper.Range(-variance, variance);
                int newPrice = (int)MathF.Max(1, good.BasePrice * multiplier);

                Player.CurrentPort.CurrentPrices[good] = newPrice;
            }
        }

        public static class RandomHelper
        {
            private static Random rng = new();

            public static float Range(float min, float max)
            {
                return (float)(min + rng.NextDouble() * (max - min));
            }
        }

        public void SetGameState(GameState newState)
        {
            if (CurrentState != GameState.MainMenu)
                previousState = CurrentState;
            CurrentState = newState;
            if (Game1.ScreenManagerRef != null)
                Game1.ScreenManagerRef.SetActive(newState);
                // SaveLoadManager.SaveGame(Player);
        }

        private void TriggerRandomEvent()
        {
            var rng = new Random();
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
    }
}
