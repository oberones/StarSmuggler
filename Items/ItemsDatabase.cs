using System.Collections.Generic;
using System;   

namespace StarSmuggler
{
    public static class ItemsDatabase
    {
        public static List<Item> AllItems { get; private set; }

        static ItemsDatabase()
        {
            AllItems = new List<Item>
            {
                // Common items
                new Item("synthspice", "Synthspice", "Mass-produced mood enhancer popular in clubs.", ItemRarity.Common, 40),
                new Item("alloy", "Reclaimed Alloys", "Scrap metal salvaged from derelict ships.", ItemRarity.Common, 30),
                new Item("stimpatches", "Stimpatches", "Performance-enhancing dermal patches.", ItemRarity.Common, 55),
                new Item("holochips", "Holochips", "Cheap holographic entertainment modules.", ItemRarity.Common, 25),
                new Item("plasteel_scraps", "Plasteel Scraps", "Lightweight and durable construction material.", ItemRarity.Common, 35),
                new Item("medigel", "Medigel", "Basic medical gel for treating minor injuries.", ItemRarity.Common, 50),

                // Mid-tier items
                new Item("neurodust", "Neurodust", "Hallucinogenic powder with precognition effects.", ItemRarity.MidTier, 150),
                new Item("void_silk", "Void Silk", "Luxury zero-G fabric harvested from orbital spiders.", ItemRarity.MidTier, 220),
                new Item("quantum_seeds", "Quantum Seeds", "Encrypted gene seeds for forbidden crops.", ItemRarity.MidTier, 280),
                new Item("neural_implants", "Neural Implants", "Black-market dream recorders and enhancers.", ItemRarity.MidTier, 320),
                new Item("grav_crystals", "Grav Crystals", "Crystals used in advanced anti-gravity tech.", ItemRarity.MidTier, 200),
                new Item("biofoam", "Biofoam", "Advanced healing foam for emergency surgeries.", ItemRarity.MidTier, 260),

                // Exotic items
                new Item("enceladite", "Enceladite", "Highly unstable energy ore from Enceladus.", ItemRarity.Exotic, 800),
                new Item("cryobloom_spores", "Cryobloom Spores", "Bioluminescent Europa spores with medicinal uses.", ItemRarity.Exotic, 950),
                new Item("temporal_relics", "Temporal Relics", "Artifacts with rumored time-warping properties.", ItemRarity.Exotic, 1200),
                new Item("darkmatter_shards", "Darkmatter Shards", "Fragments of condensed dark matter with unusual properties.", ItemRarity.Exotic, 1400),
                new Item("starlight_essence", "Starlight Essence", "Rare liquid light extracted from dying micro-stars.", ItemRarity.Exotic, 1800),
                new Item("aetherium_core", "Aetherium Core", "Energy core rumored to power ancient alien tech.", ItemRarity.Exotic, 4000),

                // Special items
                // TODO: enable the phantom code item when the game is ready for it
                // new Item("phantom_code", "Phantom Code", "Living software AI harvested in the Kuiper Belt.", ItemRarity.Exotic, 5000),

            };
        }

        // Returns a list of all items with the specified rarity
        public static List<Item> GetItemsByRarity(ItemRarity rarity, int count = 0)
        {
            if (count <= 0 || count > AllItems.Count)
            {
                return AllItems.FindAll(g => g.Rarity == rarity);
            }

            var validItems = AllItems.FindAll(g => g.Rarity == rarity);
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

        // Returns a list of items available in the specified port zone
        public static List<Item> GetZoneItems(PortZone zone, int count)
        {
            switch (zone)
            {
                case PortZone.Inner:
                    return GetItemsByRarity(ItemRarity.Common, count);
                case PortZone.Outer:
                    return GetItemsByRarity(ItemRarity.MidTier, count);
                case PortZone.Fringe:
                    return GetItemsByRarity(ItemRarity.Exotic, count);
                default:
                    return GetItemsByRarity(ItemRarity.Common, count);
            }
        }

        // Returns a list of items avilable at a zone other than the specified zone
        public static List<Item> GetOtherZoneItems(PortZone excludedZone, int count)
        {
            var allItems = new List<Item>();
            foreach (PortZone zone in Enum.GetValues(typeof(PortZone)))
            {
                if (zone != excludedZone)
                {
                    allItems.AddRange(GetZoneItems(zone, count));
                }
            }

            // Shuffle the items and return the first 'count' items
            var rng = new System.Random();
            for (int i = allItems.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                var temp = allItems[i];
                allItems[i] = allItems[j];
                allItems[j] = temp;
            }

            return allItems.GetRange(0, Math.Min(count, allItems.Count));
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
