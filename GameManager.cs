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
        private GameState? previousState;
        private GameManager() { }
        public int JumpsSinceLastUpdate => Player.JumpsSinceLastUpdate;

        // Check if the game is over based on player's credits and cargo
        // If player has less than 50 credits and cannot sell cargo for enough, game is over
        public void CheckForGameOver()
        {
            var player = Player;
            var port = player.CurrentPort;
            var currentPrices = player.CurrentPrices[port.Id];

            if (player.Credits >= 50)
                return; // Player can still travel

            // Try to find if cargo is sellable for enough to earn 50
            int potentialRevenue = 0;

            foreach (var item in player.CargoHold)
            {
                if (port.AvailableItems.Contains(item.Key))
                {
                    var marketPrice = currentPrices[item.Key.Id];
                    potentialRevenue += item.Value * marketPrice; // Number of items * market price
                    Console.WriteLine($"Checking item: {item.Key.Name}, Quantity: {item.Value}, Market Price: {marketPrice}, Potential Revenue: {potentialRevenue}");
                    if (potentialRevenue >= 50)
                        return; // Can sell enough to continue
                }
            }

            // No way to travel or make money → game over
            SetGameState(GameState.GameOver);
        }

        // Get the travel cost between two ports based on their zones and a base cost
        public int GetTravelCost(Port from, Port to)
        {
            if (from == to)
                return 0;

            int baseCost = 10;

            // Add cost based on zone difference
            int zoneDiff = Math.Abs((int)from.Zone - (int)to.Zone);
            int cost = baseCost + zoneDiff * 5;

            if (zoneDiff >= 2)
                cost += 10;

            return cost;
        }

        // Determine the markup based on the item's rarity and the port's zone. 
        // Common items are cheaper in the inner ports, while exotic items are more expensive.
        public float GetItemMarkup(Item item, Port port)
        {
            switch (item.Rarity)
            {
                case ItemRarity.Common:
                    if (port.Zone == PortZone.Fringe)
                        return 2;
                    else if (port.Zone == PortZone.Outer)
                        return 0.5f;
                    else
                        return 0;
                case ItemRarity.MidTier:
                    if (port.Zone == PortZone.Fringe)
                        return 1;
                    else if (port.Zone == PortZone.Outer)
                        return 0;
                    else
                        return 0.5f;
                case ItemRarity.Exotic:
                    if (port.Zone == PortZone.Fringe)
                        return 0;
                    else if (port.Zone == PortZone.Outer)
                        return 0.5f;
                    else
                        return 2f;
                default:
                    return 0;
            }
        }

        // Load game data from save file or start a new game if no save exists
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

        // Start a new game with a fresh player and random starting port
        public void StartNewGame()
        {
            var startingPort = PortsDatabase.GetRandomInnerPort();
            Player = new PlayerData(startingCredits: 500, cargoLimit: 30);
            Player.CurrentPort = startingPort;
            UpdatePrices(PortsDatabase.AllPorts, ItemsDatabase.AllItems);
            LoadGoodsForCurrentPort();
            SetGameState(GameState.PortOverview); 
        }

        // Travel to a specified port, deducting the travel cost and updating the player's state
        public void TravelToPort(Port destination, int cost)
        {
            if (Player.Credits >= cost)
            {
                Console.WriteLine($"Current port: {Player.CurrentPort.Name}");
                // Deduct travel cost
                Player.Credits -= cost;
                // Set player's current port to the destination
                Player.CurrentPort = destination;
                Console.WriteLine($"Destination port: {Player.CurrentPort.Name}");
                // Load the available goods for the new port
                LoadGoodsForCurrentPort();
                // Update the number of jumps since last market update
                Player.JumpsSinceLastUpdate++;
                // Update prices (if condition met)
                UpdatePrices(PortsDatabase.AllPorts, ItemsDatabase.AllItems);
                // Change the game state to the port overview
                SetGameState(GameState.PortOverview);
                // Trigger a random event after travel (25% chance)
                TriggerRandomEvent();
                // Save the game state after travel
                SaveLoadManager.SaveGame(Player);
                // Check for game over conditions after travel
                Console.WriteLine($"Checking for game over conditions after travel to {Player.CurrentPort.Name}");
                Instance.CheckForGameOver();

            }
            else
            {
                // Handle insufficient funds
                // (e.g., show warning or disable option in UI)
            }
        }

        // Load goods available at the current port and print them to the console
        public void LoadGoodsForCurrentPort()
        {
            var available = ItemsDatabase.GetCommonAndMidTier(6);
            Console.WriteLine($"Loading goods for port: {Player.CurrentPort.Name}");
            foreach (var good in available)
            {
                Console.WriteLine($"Available Good: {good.Name}");
            }
            Player.CurrentPort.AvailableItems = available;
            Player.CurrentPort.Prices = CurrentPrices[Player.CurrentPort.Id];
        }

        // Set the current game state and update the screen manager if available
        public void SetGameState(GameState newState)
        {
            if (CurrentState != GameState.MainMenu)
                previousState = CurrentState;
            CurrentState = newState;
            if (Game1.ScreenManagerRef != null)
                Game1.ScreenManagerRef.SetActive(newState);
        }

        // Trigger a random event with a 25% chance, executing the event if it occurs
        private void TriggerRandomEvent()
        {
            var rng = new Random();
            rng.Next(1, 100);
            if (rng.Next(1, 100) >= 30) {// 30% chance to trigger an event
                Player.CurrentEvent = null; // No event triggered
                return; // No event triggered
            }
            Player.CurrentEvent = EventDatabase.AllEvents[rng.Next(EventDatabase.AllEvents.Count)];
            Player.CurrentEvent.Execute(Player);
        }
        
        // Get the last triggered event
        // This can be used to display event details in the UI or log
        public GameEvent GetLastEvent()
        {
            return Player.CurrentEvent;
        }

        // Check if there is a previous game state to return to
        public bool HasPreviousState()
        {
            return previousState.HasValue;
        }

        // Return to the previous game state if it exists
        // This is useful for going back to the last screen or state before a transition
        public void ReturnToPreviousState()
        {
            if (previousState.HasValue)
            {
                SetGameState(previousState.Value);
                previousState = null;
            }
        }

        // Update prices for all ports based on the number of jumps since the last update
        // Prices are updated based on a random multiplier applied to the base price of each item
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
                        // The multiplier combines a random variance and a markup additively.
                        // This design choice ensures that prices fluctuate dynamically while incorporating
                        // a consistent markup based on the item's characteristics and the port's conditions.
                        float variance = 0.3f;
                        float markup = GetItemMarkup(item, port);
                        float multiplier = 1f + ((float)(rng.NextDouble() * 2 - 1) * variance) + markup;
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
