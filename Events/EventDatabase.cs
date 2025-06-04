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

                // Description of the event
                "Local security stops you for a 'random inspection' and demands a bribe.",

                // Effect of the event
                (player, port) => {
                    var rng = new Random();
                    int bribe = rng.Next(25, 101);
                    player.Credits = Math.Max(0, player.Credits - bribe);

                }),

            new GameEvent(
                "Merchant Strike",
                "Local vendors are striking! Prices for all items have doubled temporarily.",
                (player, port) => {
                    foreach (var g in port.AvailableItems)
                        port.Prices[g.Id] *= 2;
                }),

            new GameEvent(
                "Market Glut",
                "A recent cargo drop flooded the market. Prices for one item have plummeted.",
                (player, port) => {
                    if (port.AvailableItems.Count > 0)
                    {
                        var rng = new Random();
                        var index = rng.Next(port.AvailableItems.Count);
                        port.Prices[port.AvailableItems[index].Id] /= 2;
                    }
                }),

            new GameEvent(
                "Lost Cargo",
                "A member of the crew accidentally opened your cargo bay. You lose 1 item at random.",
                (player, port) => {
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
                (player, port) => {
                    var rng = new Random();
                    int loss = rng.Next(50, 201);
                    player.Credits = Math.Max(0, player.Credits - loss);
                }),

            new GameEvent(
                "Engine Malfunction",
                "Your ship's engine malfunctions, requiring repairs. You lose some credits.",
                (player, port) => {
                    var rng = new Random();
                    int repairCost = rng.Next(100, 301);
                    player.Credits = Math.Max(0, player.Credits - repairCost);
                }),

            new GameEvent(
                "Black Market Opportunity",
                "A shady dealer offers you rare goods at a discount.",
                (player, port) => {
                    var rng = new Random();
                    if (port.AvailableItems.Count > 0)
                    {
                        var index = rng.Next(port.AvailableItems.Count);
                        var item = port.AvailableItems[index];
                        port.Prices[item.Id] = Math.Max(1, port.Prices[item.Id] / 2);
                    }
                }),

            new GameEvent(
                "Crew Mutiny",
                "Your crew demands higher wages. You lose some credits to appease them.",
                (player, port) => {
                    var rng = new Random();
                    int wageIncrease = rng.Next(50, 151);
                    player.Credits = Math.Max(0, player.Credits - wageIncrease);
                }),
        };
    }
}
