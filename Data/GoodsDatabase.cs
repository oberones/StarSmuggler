using System.Collections.Generic;

namespace StarSmuggler
{
    public static class GoodsDatabase
    {
        public static List<Good> AllGoods { get; private set; }

        static GoodsDatabase()
        {
            AllGoods = new List<Good>
            {
                new Good("Synthspice", "Mass-produced mood enhancer popular in clubs.", GoodRarity.Common, 40),
                new Good("Reclaimed Alloys", "Scrap metal salvaged from derelict ships.", GoodRarity.Common, 30),
                new Good("Stimpatches", "Performance-enhancing dermal patches.", GoodRarity.Common, 55),
                new Good("Neurodust", "Hallucinogenic powder with precognition effects.", GoodRarity.MidTier, 150),
                new Good("Void Silk", "Luxury zero-G fabric harvested from orbital spiders.", GoodRarity.MidTier, 220),
                new Good("Quantum Seeds", "Encrypted gene seeds for forbidden crops.", GoodRarity.MidTier, 280),
                new Good("Subconscious Implants", "Black-market dream recorders and enhancers.", GoodRarity.MidTier, 320),
                new Good("Enceladite", "Highly unstable energy ore from Enceladus.", GoodRarity.Exotic, 800),
                new Good("Cryobloom Spores", "Bioluminescent Europa spores with medicinal uses.", GoodRarity.Exotic, 950),
                new Good("Temporal Relics", "Artifacts with rumored time-warping properties.", GoodRarity.Exotic, 1200),
                new Good("Phantom Code", "Living software AI harvested in the Kuiper Belt.", GoodRarity.Exotic, 1600)
            };
        }

        public static List<Good> GetGoodsByRarity(GoodRarity rarity)
        {
            return AllGoods.FindAll(g => g.Rarity == rarity);
        }

        public static List<Good> GetCommonAndMidTier(int count)
        {
            var validGoods = AllGoods.FindAll(g => g.Rarity == GoodRarity.Common || g.Rarity == GoodRarity.MidTier);
            var rng = new System.Random();
            var selected = new List<Good>();

            while (selected.Count < count && validGoods.Count > 0)
            {
                int index = rng.Next(validGoods.Count);
                selected.Add(validGoods[index]);
                validGoods.RemoveAt(index);
            }

            return selected;
        }
    }
}
