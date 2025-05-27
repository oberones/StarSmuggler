using System;
using System.Collections.Generic;

namespace StarSmuggler.Events
{
    public static class EventDatabase
    {
        public static List<GameEvent> AllEvents = new List<GameEvent>
        {
            new GameEvent(
                "Customs Shake-Down",
                "Local security stops you for a 'random inspection' and demands a bribe.",
                (player, port) => {
                    int fine = 100;
                    player.Credits = Math.Max(0, player.Credits - fine);
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
                "A smuggler accidentally opened your cargo bay. You lose 1 item at random.",
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
        };
    }
}
