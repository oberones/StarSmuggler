using System.Collections.Generic;

namespace StarSmuggler
{
    public static class ItemsDatabase
    {
        public static List<Item> AllItems { get; private set; }

        static ItemsDatabase()
        {
            AllItems = new List<Item>
            {
                new Item("Synthspice", "Mass-produced mood enhancer popular in clubs.", ItemRarity.Common, 40),
                new Item("Reclaimed Alloys", "Scrap metal salvaged from derelict ships.", ItemRarity.Common, 30),
                new Item("Stimpatches", "Performance-enhancing dermal patches.", ItemRarity.Common, 55),
                new Item("Neurodust", "Hallucinogenic powder with precognition effects.", ItemRarity.MidTier, 150),
                new Item("Void Silk", "Luxury zero-G fabric harvested from orbital spiders.", ItemRarity.MidTier, 220),
                new Item("Quantum Seeds", "Encrypted gene seeds for forbidden crops.", ItemRarity.MidTier, 280),
                new Item("Neural Implants", "Black-market dream recorders and enhancers.", ItemRarity.MidTier, 320),
                new Item("Enceladite", "Highly unstable energy ore from Enceladus.", ItemRarity.Exotic, 800),
                new Item("Cryobloom Spores", "Bioluminescent Europa spores with medicinal uses.", ItemRarity.Exotic, 950),
                new Item("Temporal Relics", "Artifacts with rumored time-warping properties.", ItemRarity.Exotic, 1200),
                new Item("Phantom Code", "Living software AI harvested in the Kuiper Belt.", ItemRarity.Exotic, 1600)
            };
        }

        public static List<Item> GetItemsByRarity(ItemRarity rarity)
        {
            return AllItems.FindAll(g => g.Rarity == rarity);
        }

        public static List<Item> GetCommonAndMidTier(int count)
        {
            var validItems = AllItems.FindAll(g => g.Rarity == ItemRarity.Common || g.Rarity == ItemRarity.MidTier);
            var rng = new System.Random();
            var selected = new List<Item>();

            while (selected.Count < count && validItems.Count > 0)
            {
                int index = rng.Next(validItems.Count);
                selected.Add(validItems[index]);
                validItems.RemoveAt(index);
            }

            return selected;
        }
    }
}
