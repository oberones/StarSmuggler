// Import the libraries we need to work with dicitionaries and lists
using System.Collections.Generic;

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
    }
}
