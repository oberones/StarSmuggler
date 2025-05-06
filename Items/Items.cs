namespace StarSmuggler {

    public enum ItemRarity
    {
        Common,
        MidTier,
        Exotic
    }
    public class Item
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ItemRarity Rarity { get; set; }
        public int BasePrice { get; set; }

        public Item(string name, string description, ItemRarity rarity, int basePrice)
        {
            Name = name;
            Description = description;
            Rarity = rarity;
            BasePrice = basePrice;
        }
    }
}