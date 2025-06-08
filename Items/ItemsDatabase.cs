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
                new Item("synthspice", "Synthspice", "Mass-produced mood enhancer popular in clubs.", ItemRarity.Common, 40),
                new Item("alloy", "Reclaimed Alloys", "Scrap metal salvaged from derelict ships.", ItemRarity.Common, 30),
                new Item("stimpatches", "Stimpatches", "Performance-enhancing dermal patches.", ItemRarity.Common, 55),
                new Item("holochips", "Holochips", "Cheap holographic entertainment modules.", ItemRarity.Common, 25),
                new Item("plasteel_scraps", "Plasteel Scraps", "Lightweight and durable construction material.", ItemRarity.Common, 35),
                new Item("medigel", "Medigel", "Basic medical gel for treating minor injuries.", ItemRarity.Common, 50),
                
                new Item("neurodust", "Neurodust", "Hallucinogenic powder with precognition effects.", ItemRarity.MidTier, 150),
                new Item("void_silk", "Void Silk", "Luxury zero-G fabric harvested from orbital spiders.", ItemRarity.MidTier, 220),
                new Item("quantum_seeds", "Quantum Seeds", "Encrypted gene seeds for forbidden crops.", ItemRarity.MidTier, 280),
                new Item("neural_implants", "Neural Implants", "Black-market dream recorders and enhancers.", ItemRarity.MidTier, 320),
                new Item("grav_crystals", "Grav Crystals", "Crystals used in advanced anti-gravity tech.", ItemRarity.MidTier, 200),
                new Item("biofoam", "Biofoam", "Advanced healing foam for emergency surgeries.", ItemRarity.MidTier, 260),
                
                new Item("enceladite", "Enceladite", "Highly unstable energy ore from Enceladus.", ItemRarity.Exotic, 800),
                new Item("cryobloom_spores", "Cryobloom Spores", "Bioluminescent Europa spores with medicinal uses.", ItemRarity.Exotic, 950),
                new Item("temporal_relics", "Temporal Relics", "Artifacts with rumored time-warping properties.", ItemRarity.Exotic, 1200),
                new Item("phantom_code", "Phantom Code", "Living software AI harvested in the Kuiper Belt.", ItemRarity.Exotic, 1600),
                // new Item("darkmatter_shards", "Darkmatter Shards", "Fragments of condensed dark matter with unknown properties.", ItemRarity.Exotic, 1400),
                // new Item("starlight_essence", "Starlight Essence", "Rare liquid light extracted from dying stars.", ItemRarity.Exotic, 1800),
                // new Item("aetherium_core", "Aetherium Core", "Energy core rumored to power ancient alien tech.", ItemRarity.Exotic, 2000)
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
