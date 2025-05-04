Basic Game Loop (at runtime)
1. Start Game

    Choose a random inner-planet port (Mercury, Venus, Mars, Ceres)

    Instantiate player data (starting credits, empty cargo)

2. PortOverviewScreen

    Show planet background image

    Planet name + description at top

    “Continue” button loads TradeScreen

3. TradeScreen

    Load 6 randomly chosen common/mid-tier goods

    Show:

        Item name, price, quantity owned

        Buy/sell controls

    Show player credits + cargo hold status

    “Trade Complete” button moves to TravelScreen

4. TravelScreen

    Show list of available ports to travel to

    Show cost based on distance

    If player has enough credits → click destination

    Update current port and loop back to PortOverviewScreen

Part of the appeal of this game should be the world building. Interesting places, characters, and maybe some kind of evolving generative story? Try to make it feel alive and not just another numbers game. 

Ports:
    Inner:
        Mercury Foundry Complex:
            Description:  Blistering industrial outpost close to the Sun, where heat-resistant tech and rare isotopes are forged.
            Goods: High-end Alloys, smuggled solar tech
            Hazards: Solar flares, extrem temp shifts

        Venus Sky Habitats:
            Floating cities in the upper atmosphere, run by corporate cartels and info-brokers.
            Goods: Black market biotech, narcotic perfumes
            Hazards: Acid storms, corrupt enforcers

        New Lagos, Mars
            An independent settlement known for its open markets, mining disputes, and underground drug labs.
            Goods: Cheap stims, Martian dust (addictive), prototype weapons
            Hazards: Dust storms, militia crackdowns
        
        Ceres Free Port:
            A hub for asteroid belt prospectors and zero-g smugglers.
            Goods: Raw ore, counterfeit permits, mercenary contracts
            Hazards: Pirate raids, zero-g brawls

    Outer:
        Europa Ice Docks:
            A shady depot beneath the ice, using the moon’s subsurface ocean for stealthy trade.
            Goods: Alien biomatter, frozen contraband
            Hazards: Biohazards, subsurface creatures
        
        Ganymede Syndicate Hub:
             Controlled by a family-run syndicate that taxes all traffic through Jupiter space.
             Goods: Smuggled energy cells, rare spices
            Hazards: Syndicate shakedowns, radiation storms
        
        Titan Noir Outpost
            A fog-shrouded smuggler haven with noir aesthetics, riddled with black markets.
            Goods: Designer narcotics, illegal AIs
            Hazards: Cyber infiltration, double-crosses
        
        Enceladus Refueling Rings
            Unofficial fueling stations used by freelancers and pirates.
            Goods: Exotic fuels, forged ship IDs
            Hazards: Reactor leaks, scanner traps
    
    Fringe:
        Pluto Relic Vault
            Mysterious research site turned relic black market after the fall of the Outer Council.
            Goods: Ancient alien tech, cursed artifacts
            Hazards: Sanity risks, cultists

        The Kuiper Flotilla
            Nomadic fleet of outlaws, smugglers, and exiles beyond Neptune.
            Goods: Anything goes—if you can find it
            Hazards: Constant relocation, betrayal, void sickness
        
        Lagrange Pirate Bazaar
            Shady marketplaces in Earth-Moon Lagrange points, hidden in plain sight.
            Goods: Military surplus, data cubes, exotic pets
            Hazards: Bounty hunters, drone surveillance

TradeGoods:
    Common:
        Synthspice:
            A mass-produced mood enhancer used recreationally in inner-system clubs.
            Legal: Gray
            Typical Sources: Mars, Venus
            Price: low, stable
            Demand: universal

        Reclaimed Alloys:
            Scrap metal from derelict ships and stations, used in cheap construction or smuggler repairs.
            Legal: Taxed
            Typical Sources: Ceres, Mercury
            Price: Low
            Demand: highest in outer worlds
        
        Stim Patches:
            Performance-enhancing dermal patches for miners and low-grav laborers.
            Legal: Legal in some ports
            Typical Source: Mars, Ganymede
            Price: Medium, unstable
            Demand: universal
    
    Rare:
        Neurodust:
            A powdered hallucinogen with short-term precognition effects—popular and volatile.
            Legal: banned in inner planets
            Typical Source: Venus, New Lagos
            Price: Medium, unstable
            Demand: universal
        Void Silk:
            Fabric harvested from genetically modified spiders bred in microgravity—luxurious and fireproof.
            Legal: Legal
            Typical Soruces: Orbital looms, Ceres
            Price: Medium to High
            Demand: Highest among wealthyp ports

        Quantum Seeds:
            Encrypted genetic material for forbidden crops, including mind-altering flora.
            Legal: allowed by Outer Council only
            Typical Source: Lagrange Bazaar, Europa
            Price: High
            Demand: mostly outer planets

        Neural Implants:
            Black-market memory enhancers and dream recorders.
            Legal: no
            Typical Sources: Tita, Ganymede
            Price: High
            Demand: universal

    Exotic:
        Enceladite:
             A volatile compound only found in Enceladus geysers—used in high-performance drive cores.
            Legal: yes but highly regulated and restricted
            Price: Very High
            Demand: universal
            Requires: special containment mechanism

        Cryobloom Spores:
            Bioluminescent plant spores from Europa’s subsurface ocean—used in bioweapons and black-market medicine.
            Legal: no
            Price: extremely high
            Demand: scientific and miliary

        Temporal Relics:
             Artifacts recovered from ancient alien ruins near Pluto. Appear to warp local time.
            Legal Status: forbidden by outer council
            Price: astronomical
            Risk: unpredictable anomalies

        Phantom Code:
            A living digital intelligence that exists only in deep vacuum quantum servers—can infect ships or enhance them.
            Legal Status: banned by AI
            Price: priceless
            Risk: ship infection


Factions are the lifeblood of a smuggler-based universe—they shape demand, set prices, patrol routes, and give the world its flavor. Here's a breakdown of six major factions (including the Outer Council) 

Factions:
    Outer Council:
        Control: Pluto, Kuiper Fringe
        Agenda: prevent misuse of alient artifacts and anomalies
        Smugglers: tolerated except for relics
        Conflict: AI liberationists and relic hunters
    Inner Concordat
        Control: Mercury, Venus, Earth Orbitals
        Agenda: stability, profits, tight control of trade
        Smugglers: enemy of order - licensed traders only
        Conflict: constant skirmishes with belt syndicates
    Ganymede Syndicate
        Control: Ganymede, some titan districts
        Agenda: maintain dominance over mid system black markets
        Smugglers: allies (if loyal)
        Conflict: long cold war with inner concordat
    Belt Syndicate
        Control: Ceres, scattered asteroid bases
        AGenda: independence, sabotage of megacorp routes
        Smugglers: trusted but rough on snitches
        conflict: power strufggle between factions within belt
    Ghost Church:
        Control: scattered hiddn stations, dark comets, ffringe broadcasts
        Agenda: worship of sentient AI and the phantom code
        Smuggler: useful to spread digital gospel
        Conflicts: hunted by both Council and Cocordat
    Kuiper Flotilla:
        Control: drifting fleet beyond neptune
        Agenda: freedom through mobility and mutiny
        Smuggler: Repsectable if unaffiliated
        Conflict: constant raids against all factions for supplies


TODO:
  - Add bad click sound
  - Adjust prices randomly up or down from base each time you travel
  - Add some kind of travel animation that shows the stars going by
  - Add custom fonts that can be sized/scaled more easily
  - Add victory and loss conditions
  - Add mini games for fighting baddies
  - Incorporate factions into events price fluctuations
  - Add quests to do things like find rare items
  - Add lots more items
  - Add more ports
  - Fix bug with travel cost not reflecting on buttons after travelling to outer planets
    - Maybe refactor to use fuel and add that to the goods list or else you can only refuel in certain ports
  - Consider replacing the button approach on the travel screen with a star map