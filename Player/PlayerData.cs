using System.Collections.Generic;
using System.Linq;

namespace StarSmuggler {
    public class PlayerData
    {
        public int Credits { get; set; }
        public Dictionary<Good, int> CargoHold { get; set; }
        public int CargoLimit { get; set; }
        public Port CurrentPort { get; set; }

        public PlayerData(int startingCredits, int cargoLimit)
        {
            Credits = startingCredits;
            CargoLimit = cargoLimit;
            CargoHold = new Dictionary<Good, int>();
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