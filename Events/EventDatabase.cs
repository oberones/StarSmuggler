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
                "Local vendors are striking! Prices for goods have doubled temporarily.",
                (player, port) => {
                    foreach (var g in port.AvailableGoods)
                        g.BasePrice *= 2;
                }),

            new GameEvent(
                "Market Glut",
                "A recent cargo drop flooded the market. Prices for one item have plummeted.",
                (player, port) => {
                    if (port.AvailableGoods.Count > 0)
                    {
                        var rng = new Random();
                        var index = rng.Next(port.AvailableGoods.Count);
                        port.AvailableGoods[index].BasePrice /= 2;
                    }
                }),

            new GameEvent(
                "Lost Cargo",
                "A smuggler accidentally opened your cargo bay. You lose 1 item at random.",
                (player, port) => {
                    var keys = new List<Good>(player.CargoHold.Keys);
                    if (keys.Count > 0)
                    {
                        var rng = new Random();
                        var randomGood = keys[rng.Next(keys.Count)];
                        player.CargoHold[randomGood] = Math.Max(0, player.CargoHold[randomGood] - 1);
                        if (player.CargoHold[randomGood] == 0)
                            player.CargoHold.Remove(randomGood);
                    }
                }),
        };
    }
}
