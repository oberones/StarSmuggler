using System;
using System.Collections.Generic;

namespace StarSmuggler.Events
{
    public static class EventDatabase
    {
        public static List<GameEvent> AllEvents = new List<GameEvent>
        {
            new GameEvent(
                // Name of the event
                "Customs Shake-Down",

                // Description of the event - will be updated dynamically
                "Local security stops you for a 'random inspection' and demands a bribe.",

                // Effect of the event
                player=> {
                    var rng = new Random();
                    int baseBribe = rng.Next(25, 101);
                    // Scale bribe based on player wealth (5-15% of credits), minimum is base amount
                    float percentage = (float)(rng.NextDouble() * 0.10 + 0.05); // 5-15%
                    int scaledBribe = Math.Max(baseBribe, (int)(player.Credits * percentage));
                    
                    // Update description with actual amount
                    var currentEvent = player.CurrentEvent;
                    if (currentEvent != null)
                        currentEvent.Description = $"Local security stops you for a 'random inspection' and demands a bribe of {scaledBribe} credits.";
                    
                    player.Credits = Math.Max(0, player.Credits - scaledBribe);
                    Console.WriteLine($"Customs bribe: {scaledBribe} credits ({percentage:P1} of wealth, min {baseBribe})");
                }),

            new GameEvent(
                "Merchant Strike",
                "Local vendors are striking! Prices for all items have doubled temporarily.",
                player => {
                    foreach (var g in player.CurrentPort.AvailableItems)
                        player.CurrentPrices[player.CurrentPort.Id][g.Id] *= 2;
                    Console.WriteLine($"Prices doubled at {player.CurrentPort.Name} due to Merchant Strike.");
                }),

            new GameEvent(
                "Market Glut",
                "A recent cargo drop flooded the market. Prices for one item have plummeted.",
                player => {
                    if (player.CurrentPort.AvailableItems.Count > 0)
                    {
                        var rng = new Random();
                        var index = rng.Next(player.CurrentPort.AvailableItems.Count);
                        var item = player.CurrentPort.AvailableItems[index];
                        player.CurrentPrices[player.CurrentPort.Id][item.Id] = Math.Max(1, player.CurrentPrices[player.CurrentPort.Id][item.Id] / 2);
                        Console.WriteLine($"Market glut for {item.Name} at {player.CurrentPort.Name}. New price: {player.CurrentPrices[player.CurrentPort.Id][item.Id]}");
                    }
                }),

            new GameEvent(
                "Lost Cargo",
                "A member of the crew accidentally opened your cargo bay. You lose 1 item at random.",
                player => {
                    var keys = new List<Item>(player.CargoHold.Keys);
                    if (keys.Count > 0)
                    {
                        var rng = new Random();
                        var randomItem = keys[rng.Next(keys.Count)];
                        player.CargoHold[randomItem] = Math.Max(0, player.CargoHold[randomItem] - 1);
                        if (player.CargoHold[randomItem] == 0)
                            player.CargoHold.Remove(randomItem);
                    }
                }),

            new GameEvent(
                "Pirate Ambush",
                "Pirates attack your ship! You lose some credits to avoid destruction.",
                player => {
                    var rng = new Random();
                    int baseLoss = rng.Next(50, 201);
                    // Scale loss based on player wealth (10-25% of credits), minimum is base amount
                    float percentage = (float)(rng.NextDouble() * 0.15 + 0.10); // 10-25%
                    int scaledLoss = Math.Max(baseLoss, (int)(player.Credits * percentage));
                    
                    // Update description with actual amount
                    var currentEvent = player.CurrentEvent;
                    if (currentEvent != null)
                        currentEvent.Description = $"Pirates attack your ship! You lose {scaledLoss} credits to avoid destruction.";
                    
                    player.Credits = Math.Max(0, player.Credits - scaledLoss);
                    Console.WriteLine($"Pirate ambush loss: {scaledLoss} credits ({percentage:P1} of wealth, min {baseLoss})");
                }),

            new GameEvent(
                "Engine Malfunction",
                "Your ship's engine malfunctions, requiring repairs. You lose some credits.",
                player => {
                    var rng = new Random();
                    int baseRepairCost = rng.Next(100, 301);
                    // Scale repair cost based on player wealth (8-20% of credits), minimum is base amount
                    float percentage = (float)(rng.NextDouble() * 0.12 + 0.08); // 8-20%
                    int scaledRepairCost = Math.Max(baseRepairCost, (int)(player.Credits * percentage));
                    
                    // Update description with actual amount
                    var currentEvent = player.CurrentEvent;
                    if (currentEvent != null)
                        currentEvent.Description = $"Your ship's engine malfunctions, requiring repairs costing {scaledRepairCost} credits.";
                    
                    player.Credits = Math.Max(0, player.Credits - scaledRepairCost);
                    Console.WriteLine($"Engine repair cost: {scaledRepairCost} credits ({percentage:P1} of wealth, min {baseRepairCost})");
                }),

            // new GameEvent(
            //     "Black Market Opportunity",
            //     "A shady dealer offers you rare goods at a discount.",
            //     player => {
            //         var rng = new Random();
            //         if (player.CurrentPort.AvailableItems.Count > 0)
            //         {
            //             var index = rng.Next(port.AvailableItems.Count);
            //             var item = port.AvailableItems[index];
            //             port.Prices[item.Id] = Math.Max(1, port.Prices[item.Id] / 2);
            //         }
            //     }),

            new GameEvent(
                "Crew Mutiny",
                "Your crew demands higher wages. You lose some credits to appease them.",
                player => {
                    var rng = new Random();
                    int baseWageIncrease = rng.Next(50, 151);
                    // Scale wage increase based on player wealth (6-18% of credits), minimum is base amount
                    float percentage = (float)(rng.NextDouble() * 0.12 + 0.06); // 6-18%
                    int scaledWageIncrease = Math.Max(baseWageIncrease, (int)(player.Credits * percentage));
                    
                    // Update description with actual amount
                    var currentEvent = player.CurrentEvent;
                    if (currentEvent != null)
                        currentEvent.Description = $"Your crew demands higher wages. You pay {scaledWageIncrease} credits to appease them.";
                    
                    player.Credits = Math.Max(0, player.Credits - scaledWageIncrease);
                    Console.WriteLine($"Crew wage increase: {scaledWageIncrease} credits ({percentage:P1} of wealth, min {baseWageIncrease})");
                }),
        };
    }
}
