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
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string BackgroundImagePath { get; set; }
        public string PreviewImagePath { get; set; } // For the travel screen
        public string MusicTrackName { get; set; } // e.g. "venus_theme"
        public PortZone Zone { get; set; }
        
        public Dictionary<string, int> Prices { get; set; }
        
        // List of goods available at this port for the current visit
        public List<Item> AvailableItems { get; set; }
        
        public Port(string id, string name, string description, PortZone zone, string backgroundImagePath, string previewImagePath, string musicTrackName = null)
        {
            Id = id;
            Name = name;
            Description = description;
            Zone = zone;
            BackgroundImagePath = backgroundImagePath;
            PreviewImagePath = previewImagePath;
            MusicTrackName = musicTrackName;

            // A list of items available at this port for the current visit
            AvailableItems = new List<Item>();

            // A list of items and their prices at this port for the current visit
            Prices = new Dictionary<string, int>();
        }
    }
}
