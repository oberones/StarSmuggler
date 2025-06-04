using System.Collections.Generic;

namespace StarSmuggler
{
    public class SaveData
    {
        public string CurrentPortName { get; set; }
        public Dictionary<string, Dictionary<string, int>> Prices { get; set; } = new();
        public int Credits { get; set; }
        public int CargoLimit { get; set; }
        public Dictionary<string, int> CargoHold { get; set; } = new();
    }
}
