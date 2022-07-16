﻿using System.Collections.Generic;

namespace Bot.GameData;

internal static class Units {
    public const uint Colossus = 4;
    public const uint Techlab = 5;
    public const uint Reactor = 6;
    public const uint InfestorTerran = 7;
    public const uint BanelingCocoon = 8;
    public const uint Baneling = 9;
    public const uint Mothership = 10;
    public const uint PointDefenseDrone = 11;
    public const uint Changeling = 12;
    public const uint ChangelingZealot = 13;
    public const uint ChangelingMarineShield = 14;
    public const uint ChangelingMarine = 15;
    public const uint ChangelingZerglingWings = 16;
    public const uint ChangelingZergling = 17;
    public const uint CommandCenter = 18;
    public const uint SupplyDepot = 19;
    public const uint Refinery = 20;
    public const uint Barracks = 21;
    public const uint EngineeringBay = 22;
    public const uint MissileTurret = 23;
    public const uint Bunker = 24;
    public const uint SensorTower = 25;
    public const uint GhostAcademy = 26;
    public const uint Factory = 27;
    public const uint Starport = 28;
    public const uint Armory = 29;
    public const uint FusionCore = 30;
    public const uint AutoTurret = 31;
    public const uint SiegeTankSieged = 32;
    public const uint SiegeTank = 33;
    public const uint VikingAssault = 34;
    public const uint VikingFighter = 35;
    public const uint CommandCenterFlying = 36;
    public const uint BarracksTechlab = 37;
    public const uint BarracksReactor = 38;
    public const uint FactoryTechlab = 39;
    public const uint FactoryReactor = 40;
    public const uint StarportTechlab = 41;
    public const uint StarportReactor = 42;
    public const uint FactoryFlying = 43;
    public const uint StarportFlying = 44;
    public const uint Scv = 45;
    public const uint BarracksFlying = 46;
    public const uint SupplyDepotLowered = 47;
    public const uint Marine = 48;
    public const uint Reaper = 49;
    public const uint WidowMine = 498;
    public const uint WidowMineBurrowed = 500;
    public const uint Liberator = 689;
    public const uint Ghost = 50;
    public const uint Marauder = 51;
    public const uint Mule = 268;
    public const uint Thor = 52;
    public const uint Hellion = 53;
    public const uint Hellbat = 484;
    public const uint Cyclone = 692;
    public const uint Medivac = 54;
    public const uint Banshee = 55;
    public const uint Raven = 56;
    public const uint Battlecruiser = 57;
    public const uint Nuke = 58;
    public const uint Nexus = 59;
    public const uint Pylon = 60;
    public const uint Assimilator = 61;
    public const uint Gateway = 62;
    public const uint Forge = 63;
    public const uint FleetBeacon = 64;
    public const uint TwilightCounsel = 65;
    public const uint PhotonCannon = 66;
    public const uint Stargate = 67;
    public const uint TemplarArchive = 68;
    public const uint DarkShrine = 69;
    public const uint RoboticsBay = 70;
    public const uint RoboticsFacility = 71;
    public const uint CyberneticsCore = 72;
    public const uint Zealot = 73;
    public const uint Stalker = 74;
    public const uint Adept = 311;
    public const uint HighTemplar = 75;
    public const uint DarkTemplar = 76;
    public const uint Sentry = 77;
    public const uint Phoenix = 78;
    public const uint Carrier = 79;
    public const uint VoidRay = 80;
    public const uint WarpPrism = 81;
    public const uint Observer = 82;
    public const uint Immortal = 83;
    public const uint Probe = 84;
    public const uint Interceptor = 85;
    public const uint Hatchery = 86;
    public const uint CreepTumor = 87;
    public const uint Extractor = 88;
    public const uint SpawningPool = 89;
    public const uint EvolutionChamber = 90;
    public const uint HydraliskDen = 91;
    public const uint Spire = 92;
    public const uint UltraliskCavern = 93;
    public const uint InfestationPit = 94;
    public const uint NydusNetwork = 95;
    public const uint BanelingNest = 96;
    public const uint RoachWarren = 97;
    public const uint SpineCrawler = 98;
    public const uint SporeCrawler = 99;
    public const uint Lair = 100;
    public const uint Hive = 101;
    public const uint GreaterSpire = 102;
    public const uint Egg = 103;
    public const uint Drone = 104;
    public const uint Zergling = 105;
    public const uint Overlord = 106;
    public const uint Hydralisk = 107;
    public const uint Mutalisk = 108;
    public const uint Ultralisk = 109;
    public const uint Roach = 110;
    public const uint Infestor = 111;
    public const uint Corruptor = 112;
    public const uint BroodLordCocoon = 113;
    public const uint BroodLord = 114;
    public const uint Broodling = 289; // TODO GD Which one is it?
    public const uint BroodlingEscort = 143; // TODO GD Which one is it?
    public const uint BanelingBurrowed = 115;
    public const uint DroneBurrowed = 116;
    public const uint HydraliskBurrowed = 117;
    public const uint RoachBurrowed = 118;
    public const uint ZerglingBurrowed = 119;
    public const uint InfestorTerranBurrowed = 120;
    public const uint QueenBurrowed = 125;
    public const uint Queen = 126;
    public const uint InfestorBurrowed = 127;
    public const uint OverlordCocoon = 128;
    public const uint Overseer = 129;
    public const uint PlanetaryFortress = 130;
    public const uint UltraliskBurrowed = 131;
    public const uint OrbitalCommand = 132;
    public const uint WarpGate = 133;
    public const uint OrbitalCommandFlying = 134;
    public const uint ForceField = 135;
    public const uint WarpPrismPhasing = 136;
    public const uint CreepTumorBurrowed = 137;
    public const uint CreepTumorQueen = 138;
    public const uint SpineCrawlerUprooted = 139;
    public const uint SporeCrawlerUprooted = 140;
    public const uint Archon = 141;
    public const uint NydusWorm = 142;
    public const uint RichMineralField = 146;
    public const uint RichMineralField750 = 147;
    public const uint Ursadon = 148;
    public const uint XelNagaTower = 149;
    public const uint InfestedTerransEgg = 150;
    public const uint Larva = 151;
    public const uint MineralField = 341;
    public const uint VespeneGeyser = 342;
    public const uint SpacePlatformGeyser = 343;
    public const uint RichVespeneGeyser = 344;
    public const uint MineralField750 = 483;
    public const uint ProtossVespeneGeyser = 608;
    public const uint LabMineralField = 665;
    public const uint LabMineralField750 = 666;
    public const uint PurifierRichMineralField = 796;
    public const uint PurifierRichMineralField750 = 797;
    public const uint PurifierVespeneGeyser = 880;
    public const uint ShakurasVespeneGeyser = 881;
    public const uint PurifierMineralField = 884;
    public const uint PurifierMineralField750 = 885;
    public const uint BattleStationMineralField = 886;
    public const uint BattleStationMineralField750 = 887;
    public const uint SwarmHost = 494; // SwarmHostMP?
    public const uint SwarmHostBurrowed = 493; // SwarmHostBurrowedMP?
    public const uint Viper = 499;
    public const uint Lurker = 502; // LurkerMP?
    public const uint LurkerBurrowed = 503; // LurkerMPBurrowed?
    public const uint LurkerDen = 504; // LurkerDenMP?
    public const uint Ravager = 688;
    public const uint RavagerBurrowed = 690;

    public static readonly HashSet<uint> Structures = new HashSet<uint>
    {
        Armory,
        Assimilator,
        BanelingNest,
        Barracks,
        BarracksFlying,
        BarracksReactor,
        BarracksTechlab,
        Bunker,
        CommandCenter,
        CommandCenterFlying,
        CyberneticsCore,
        DarkShrine,
        EngineeringBay,
        EvolutionChamber,
        Extractor,
        Factory,
        FactoryFlying,
        FactoryReactor,
        FactoryTechlab,
        FleetBeacon,
        Forge,
        FusionCore,
        Gateway,
        GhostAcademy,
        GreaterSpire,
        Hatchery,
        Hive,
        HydraliskDen,
        InfestationPit,
        Lair,
        MissileTurret,
        Nexus,
        NydusNetwork,
        OrbitalCommand,
        OrbitalCommandFlying,
        PhotonCannon,
        PlanetaryFortress,
        Pylon,
        Reactor,
        Refinery,
        RoachWarren,
        RoboticsBay,
        RoboticsFacility,
        SensorTower,
        SpawningPool,
        SpineCrawler,
        SpineCrawlerUprooted,
        Spire,
        SporeCrawler,
        SporeCrawlerUprooted,
        Starport,
        Stargate,
        StarportFlying,
        StarportReactor,
        StarportTechlab,
        SupplyDepot,
        SupplyDepotLowered,
        Techlab,
        TemplarArchive,
        TwilightCounsel,
        UltraliskCavern,
        WarpGate
    };

    public static readonly HashSet<uint> Production = new HashSet<uint>
    {
        Armory,
        BanelingNest,
        Barracks,
        BarracksTechlab,
        CommandCenter,
        CyberneticsCore,
        EngineeringBay,
        EvolutionChamber,
        Factory,
        FactoryTechlab,
        FleetBeacon,
        Forge,
        FusionCore,
        Gateway,
        GhostAcademy,
        GreaterSpire,
        Hatchery,
        Hive,
        HydraliskDen,
        InfestationPit,
        Lair,
        Nexus,
        NydusNetwork,
        OrbitalCommand,
        PlanetaryFortress,
        RoachWarren,
        RoboticsBay,
        RoboticsFacility,
        SpawningPool,
        Spire,
        Starport,
        Stargate,
        StarportTechlab,
        Techlab,
        TemplarArchive,
        TwilightCounsel,
        UltraliskCavern,
        WarpGate
    };

    public static readonly HashSet<uint> TerranMilitary = new HashSet<uint>
    {
        Hellbat,
        Liberator,
        WidowMine,
        WidowMineBurrowed,
        Cyclone,
        AutoTurret,
        Banshee,
        Battlecruiser,
        Ghost,
        Hellion,
        VikingAssault,
        VikingFighter,
        Thor,
        Marauder,
        Marine,
        Medivac,
        Raven,
        Reaper,
        SiegeTank,
        SiegeTankSieged,
    };

    public static readonly HashSet<uint> ProtossMilitary = new HashSet<uint>
    {
        Adept,
        Archon,
        Carrier,
        Colossus,
        DarkTemplar,
        HighTemplar,
        Immortal,
        VoidRay,
        Zealot,
        Stalker,
        Sentry,
        Mothership,
        Phoenix,
    };

    public static readonly HashSet<uint> ZergMilitary = new HashSet<uint>
    {
        // Ground
        // Queen,
        // QueenBurrowed,
        Zergling,
        ZerglingBurrowed,
        Baneling,
        BanelingBurrowed,
        Roach,
        RoachBurrowed,
        Ravager,
        RavagerBurrowed,
        Hydralisk,
        HydraliskBurrowed,
        Lurker,
        LurkerBurrowed,
        SwarmHost,
        SwarmHostBurrowed,
        Infestor,
        InfestorBurrowed,
        InfestorTerran,
        InfestorTerranBurrowed,
        Ultralisk,
        UltraliskBurrowed,

        // Flying
        Mutalisk,
        Corruptor,
        BroodLord,
        Viper,
    };

    public static readonly HashSet<uint> ResourceCenters = new HashSet<uint>
    {
        CommandCenter,
        CommandCenterFlying,
        Hatchery,
        Lair,
        Hive,
        Nexus,
        OrbitalCommand,
        OrbitalCommandFlying,
        PlanetaryFortress
    };

    // Mineral field types seem to differ from map to map
    public static readonly HashSet<uint> MineralFields = new HashSet<uint>
    {
        RichMineralField,
        RichMineralField750,
        MineralField,
        MineralField750,
        LabMineralField,
        LabMineralField750,
        PurifierRichMineralField,
        PurifierRichMineralField750,
        PurifierMineralField,
        PurifierMineralField750,
        BattleStationMineralField,
        BattleStationMineralField750
    };

    public static readonly HashSet<uint> Extractors = new HashSet<uint>
    {
        Extractor,
        Refinery,
        Assimilator,
    };

    // Gas geyser types seem to differ from map to map
    public static readonly HashSet<uint> GasGeysers = new HashSet<uint>
    {
        VespeneGeyser,
        SpacePlatformGeyser,
        RichVespeneGeyser,
        ProtossVespeneGeyser,
        PurifierVespeneGeyser,
        ShakurasVespeneGeyser,
    };

    public static readonly HashSet<uint> Workers = new HashSet<uint>
    {
        Scv,
        Probe,
        Drone
    };

    public static readonly HashSet<uint> Mechanical = new HashSet<uint>
    {
        Hellbat,
        Banshee,
        Thor,
        SiegeTank,
        SiegeTankSieged,
        Battlecruiser,
        VikingAssault,
        VikingFighter,
        Hellion,
        Cyclone,
        WidowMine,
        WidowMineBurrowed,
        Liberator,
        Raven,
        Medivac
    };

    public static readonly HashSet<uint> Liftable = new HashSet<uint>
    {
        CommandCenter,
        OrbitalCommand,
        Barracks,
        Factory,
        Starport
    };

    public static readonly HashSet<uint> StaticAirDefense = new HashSet<uint>
    {
        PhotonCannon,
        MissileTurret,
        SporeCrawler,
        Bunker
    };

    public static readonly HashSet<uint> StaticGroundDefense = new HashSet<uint>
    {
        PhotonCannon,
        Bunker,
        SpineCrawler,
        PlanetaryFortress
    };

    public static readonly HashSet<uint> SiegeTanks = new HashSet<uint>
    {
        SiegeTank,
        SiegeTankSieged
    };

    public static readonly HashSet<uint> Vikings = new HashSet<uint>
    {
        VikingAssault,
        VikingFighter
    };

    public static readonly HashSet<uint> FromBarracks = new HashSet<uint>
    {
        Reaper,
        Marine,
        Marauder,
        Ghost
    };

    public static readonly HashSet<uint> FromFactory = new HashSet<uint>
    {
        Thor,
        Hellion,
        Hellbat,
        SiegeTank,
        Cyclone
    };

    public static readonly HashSet<uint> FromStarport = new HashSet<uint>
    {
        VikingFighter,
        Raven,
        Banshee,
        Battlecruiser,
        Liberator
    };

    public static readonly HashSet<uint> AddOns = new HashSet<uint>
    {
        Techlab,
        Reactor,
        BarracksReactor,
        BarracksTechlab,
        FactoryTechlab,
        FactoryReactor,
        StarportTechlab,
        StarportReactor
    };

    public static readonly HashSet<uint> SupplyDepots = new HashSet<uint>
    {
        SupplyDepot,
        SupplyDepotLowered
    };

    public static readonly Dictionary<uint, HashSet<uint>> Producers = new Dictionary<uint, HashSet<uint>>
    {
        { Drone,                             new HashSet<uint> { Larva }},
        { Corruptor,                         new HashSet<uint> { Larva }},
        { BroodLord,                         new HashSet<uint> { Corruptor }},
        { Hydralisk,                         new HashSet<uint> { Larva }},
        { Lurker,                            new HashSet<uint> { Hydralisk }},
        { Infestor,                          new HashSet<uint> { Larva }},
        { Mutalisk,                          new HashSet<uint> { Larva }},
        { Overlord,                          new HashSet<uint> { Larva }},
        { Overseer,                          new HashSet<uint> { Overlord }},
        { Queen,                             new HashSet<uint> { Hatchery, Lair, Hive }},
        { Roach,                             new HashSet<uint> { Larva }},
        { Ravager,                           new HashSet<uint> { Roach }},
        { Ultralisk,                         new HashSet<uint> { Larva }},
        { Zergling,                          new HashSet<uint> { Larva }},
        { SwarmHost,                         new HashSet<uint> { Larva }},
        { Viper,                             new HashSet<uint> { Larva }},
        { Baneling,                          new HashSet<uint> { Zergling }},
        { BanelingNest,                      new HashSet<uint> { Drone }},
        { EvolutionChamber,                  new HashSet<uint> { Drone }},
        { Extractor,                         new HashSet<uint> { Drone }},
        { Hatchery,                          new HashSet<uint> { Drone }},
        { Lair,                              new HashSet<uint> { Hatchery }},
        { Hive,                              new HashSet<uint> { Lair }},
        { HydraliskDen,                      new HashSet<uint> { Drone }},
        { LurkerDen,                         new HashSet<uint> { HydraliskDen }},
        { InfestationPit,                    new HashSet<uint> { Drone }},
        { NydusNetwork,                      new HashSet<uint> { Drone }},
        { RoachWarren,                       new HashSet<uint> { Drone }},
        { SpawningPool,                      new HashSet<uint> { Drone }},
        { SpineCrawler,                      new HashSet<uint> { Drone }},
        { Spire,                             new HashSet<uint> { Drone }},
        { GreaterSpire,                      new HashSet<uint> { Spire }},
        { SporeCrawler,                      new HashSet<uint> { Drone }},
        { UltraliskCavern,                   new HashSet<uint> { Drone }},
        { Upgrades.ZergMissileWeaponsLevel1, new HashSet<uint> { EvolutionChamber }},
        { Upgrades.GlialReconstitution,      new HashSet<uint> { RoachWarren }},
        { Upgrades.TunnelingClaws,           new HashSet<uint> { RoachWarren }},
        { Upgrades.Burrow,                   new HashSet<uint> { Hatchery }},
    };

    public static readonly Dictionary<uint, uint> MorphInto = new Dictionary<uint, uint>
    {
        { Drone,            Larva },
        { Corruptor,        Larva },
        { BroodLord,        Corruptor },
        { Hydralisk,        Larva },
        { Lurker,           Hydralisk },
        { Infestor,         Larva },
        { Mutalisk,         Larva },
        { Overlord,         Larva },
        { Overseer,         Overlord },
        { Roach,            Larva },
        { Ravager,          Roach },
        { Ultralisk,        Larva },
        { Zergling,         Larva },
        { SwarmHost,        Larva },
        { Viper,            Larva },
        { Baneling,         Zergling },
        { BanelingNest,     Drone },
        { EvolutionChamber, Drone },
        { Extractor,        Drone },
        { Hatchery,         Drone },
        { Lair,             Hatchery },
        { Hive,             Lair },
        { HydraliskDen,     Drone },
        { LurkerDen,        Drone },
        { InfestationPit,   Drone },
        { NydusNetwork,     Drone },
        { RoachWarren,      Drone },
        { SpawningPool,     Drone },
        { SpineCrawler,     Drone },
        { Spire,            Drone },
        { GreaterSpire,     Spire },
        { SporeCrawler,     Drone },
        { UltraliskCavern,  Drone },
    };

    public static readonly Dictionary<uint, HashSet<uint>> EquivalentTo = new Dictionary<uint, HashSet<uint>>
    {
        { Hatchery, new HashSet<uint> { Lair, Hive } },
        { Lair,     new HashSet<uint> { Hive } },
        { Spire,    new HashSet<uint> { GreaterSpire } },
    };

    public static readonly Dictionary<uint, uint> Prerequisites = new Dictionary<uint, uint>
    {
        { EvolutionChamber,                  Hatchery },
        { SpawningPool,                      Hatchery },
        { Zergling,                          SpawningPool },
        { Queen,                             SpawningPool },
        { SpineCrawler,                      SpawningPool },
        { SporeCrawler,                      SpawningPool },
        { BanelingNest,                      SpawningPool },
        { Baneling,                          BanelingNest },
        { RoachWarren,                       SpawningPool },
        { Roach,                             RoachWarren },
        { Ravager,                           RoachWarren },
        { Lair,                              SpawningPool },
        { Overseer,                          Lair },
        { NydusNetwork,                      Lair },
        { NydusWorm,                         NydusNetwork },
        { InfestationPit,                    Lair },
        { Infestor,                          InfestationPit },
        { SwarmHost,                         InfestationPit },
        { Spire,                             Lair },
        { Corruptor,                         Spire },
        { Mutalisk,                          Spire },
        { HydraliskDen,                      Lair },
        { Hydralisk,                         HydraliskDen },
        { LurkerDen,                         HydraliskDen },
        { Lurker,                            LurkerDen },
        { Hive,                              Lair },
        { Viper,                             Hive },
        { GreaterSpire,                      Hive },
        { BroodLord,                         GreaterSpire },
        { UltraliskCavern,                   Hive },
        { Ultralisk,                         UltraliskCavern },

        { Upgrades.ZergMissileWeaponsLevel1, EvolutionChamber },
        { Upgrades.GlialReconstitution,      RoachWarren },
        { Upgrades.TunnelingClaws,           RoachWarren },
        { Upgrades.Burrow,                   Hatchery },
    };
}