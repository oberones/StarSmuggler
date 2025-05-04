using System.Collections.Generic;

namespace StarSmuggler
{
    public static class PortsDatabase
    {
        public static List<Port> AllPorts { get; private set; }

        static PortsDatabase()
        {
            AllPorts = new List<Port>
            {
                new Port("Mercury Foundry Complex", "Blistering factories melt metal near the sun.", PortZone.Inner, "Ports/mercury"),
                new Port("Venus Sky Habitats", "Floating cities hover above acid clouds.", PortZone.Inner, "Ports/venus"),
                new Port("New Lagos, Mars", "Martian frontier full of miners and misfits.", PortZone.Inner, "Ports/mars"),
                new Port("Ceres Free Port", "Neutral ground for traders and asteroid miners.", PortZone.Inner, "Ports/ceres"),

                new Port("Europa Ice Docks", "Subsurface ocean labs and illicit biotrade.", PortZone.Outer, "Ports/europa"),
                new Port("Titan Outpost", "Foggy noir smuggler den under the massive eye of Saturn.", PortZone.Outer, "Ports/titan"),

                new Port("Pluto Relic Vault", "Forbidden site of ancient tech discoveries.", PortZone.Fringe, "Ports/pluto"),
                new Port("Kuiper Flotilla", "Nomadic pirate fleet beyond Neptune.", PortZone.Fringe, "Ports/kuiper")
            };
        }

        public static List<Port> GetPortsByZone(PortZone zone)
        {
            return AllPorts.FindAll(p => p.Zone == zone);
        }

        public static Port GetRandomInnerPort()
        {
            var innerPorts = GetPortsByZone(PortZone.Inner);
            var rng = new System.Random();
            return innerPorts[rng.Next(innerPorts.Count)];
        }
    }
}
