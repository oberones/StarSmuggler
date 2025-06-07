using System.Collections.Generic;
using System.Linq;
using StarSmuggler.Events;

namespace StarSmuggler {
    public class PlayerData
    {
        public int Credits { get; set; }
        public Dictionary<Item, int> CargoHold { get; set; }
        public int CargoLimit { get; set; }
        public Port CurrentPort { get; set; }
        public Dictionary<string, Dictionary<string, int>> CurrentPrices { get; set; }
        public int JumpsSinceLastUpdate { get; set; } = 0;
        public GameEvent CurrentEvent { get; set; }

        public PlayerData(int startingCredits, int cargoLimit)
        {
            Credits = startingCredits;
            CargoLimit = cargoLimit;
            CargoHold = new Dictionary<Item, int>();
            CurrentPrices = new Dictionary<string, Dictionary<string, int>>();
            CurrentPort = null; // Will be set when player visits a port
            CurrentEvent = null; // No event active at start
        }

        public int GetCurrentCargoLoad()
        {
            return CargoHold.Values.Sum();
        }

        public bool CanAddCargo(int quantity)
        {
            return (GetCurrentCargoLoad() + quantity) <= CargoLimit;
        }
    }
}