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
                new Port("mercury", "Mercury Foundry Complex", "Blistering factories melt metal near the sun.", PortZone.Inner, "Ports/mercury", "world_default"),
                new Port("venus", "Venus Sky Habitats", "Floating cities hover above acid clouds.", PortZone.Inner, "Ports/venus", "world_default"),
                new Port("mars", "New Lagos, Mars", "Martian frontier full of miners and misfits.", PortZone.Inner, "Ports/mars", "world_default"),
                new Port("ceres", "Ceres Free Port", "Neutral ground for traders and asteroid miners.", PortZone.Inner, "Ports/ceres", "world_default"),

                new Port("europa", "Europa Ice Docks", "Subsurface ocean labs and illicit biotrade.", PortZone.Outer, "Ports/europa", "world_default"),
                new Port("titan", "Titan Outpost", "Foggy noir smuggler den under the massive eye of Saturn.", PortZone.Outer, "Ports/titan", "world_default"),

                new Port("pluto", "Pluto Relic Vault", "Forbidden site of ancient tech discoveries.", PortZone.Fringe, "Ports/pluto", "world_default"),
                new Port("kuiper", "Kuiper Flotilla", "Nomadic pirate fleet beyond Neptune.", PortZone.Fringe, "Ports/kuiper", "world_default")
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
