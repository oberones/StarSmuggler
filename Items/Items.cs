namespace StarSmuggler {

    public enum ItemRarity
    {
        Common,
        MidTier,
        Exotic
    }
    public class Item
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ItemRarity Rarity { get; set; }
        public int BasePrice { get; set; }

        public Item(string id, string name, string description, ItemRarity rarity, int basePrice)
        {
            Id = id;
            Name = name;
            Description = description;
            Rarity = rarity;
            BasePrice = basePrice;
        }
    }
}