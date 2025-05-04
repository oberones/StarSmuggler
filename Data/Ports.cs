// Import the libraries we need to work with dicitionaries and lists
using System.Collections.Generic;
using System;

namespace StarSmuggler {

    public enum PortZone
    {
        Inner,
        Outer,
        Fringe
    }

    public class Port
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string BackgroundImagePath { get; set; }
        public string MusicTrackName { get; set; } // e.g. "venus_theme"
        public PortZone Zone { get; set; }
        
        public Dictionary<Good, int> CurrentPrices { get; set; } = new();
        // List of goods available at this port for the current visit
        public List<Good> AvailableGoods { get; set; }
        

        public Port(string name, string description, PortZone zone, string backgroundImagePath, string musicTrackName = null)
        {
            Name = name;
            Description = description;
            Zone = zone;
            BackgroundImagePath = backgroundImagePath;
            MusicTrackName = musicTrackName;
            AvailableGoods = new List<Good>();
        }

        // // Randomize market prices for goods at the current port
        // public void GenerateMarketPrices()
        // {
        //     Console.WriteLine($"Generating market prices");
        //     CurrentPrices.Clear();
            
        //     foreach (var good in AvailableGoods)
        //     {
        //         // Allow +/- 30% price swing
        //         Console.WriteLine($"Generating market prices for : {good.Name}");
        //         float variance = 0.3f;
        //         float multiplier = 1f + RandomHelper.Range(-variance, variance);
        //         int newPrice = (int)MathF.Max(1, good.BasePrice * multiplier);

        //         CurrentPrices[good] = newPrice;
        //     }
        // }

        //         // Helper class to generate random numbers
        // public static class RandomHelper
        // {
        //     private static Random rng = new();

        //     public static float Range(float min, float max)
        //     {
        //         return (float)(min + rng.NextDouble() * (max - min));
        //     }
        // }
    }
}
