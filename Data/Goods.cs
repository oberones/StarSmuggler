// Define the Good class and its properties. What does it MEAN to be a good?

namespace StarSmuggler {
    
    // Define global references to the rarity properties of goods
    public enum GoodRarity
    {
        Common,
        MidTier,
        Exotic
    }
    public class Good
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public GoodRarity Rarity { get; set; }
        public int BasePrice { get; set; }

        // Optional: Could include volatility, legality flags, etc.

        public Good(string name, string description, GoodRarity rarity, int basePrice)
        {
            Name = name;
            Description = description;
            Rarity = rarity;
            BasePrice = basePrice;
        }
    }
}