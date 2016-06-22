using System;

public enum TraitType
{
    [StrRefAttr(0x39, "UI")]
    Abadar = 100,
    [StrRefAttr(0x3a, "UI")]
    Aberration = 150,
    [StrRefAttr(0x3b, "UI")]
    Accessory = 200,
    [StrRefAttr(60, "UI")]
    Achaekek = 300,
    [StrRefAttr(0x3d, "UI")]
    Acid = 400,
    [StrRefAttr(0x3e, "UI")]
    Admiral = 450,
    [StrRefAttr(0x3f, "UI")]
    Alchemical = 500,
    [StrRefAttr(0x40, "UI")]
    Alchemist = 600,
    [StrRefAttr(0x41, "UI")]
    Angel = 700,
    [StrRefAttr(0x42, "UI")]
    Animal = 800,
    [StrRefAttr(0x1e2, "UI")]
    Ape = 0x36b,
    [StrRefAttr(0x43, "UI")]
    Aquatic = 900,
    [StrRefAttr(0x44, "UI")]
    Arcane = 0x3e8,
    [StrRefAttr(0x45, "UI")]
    Aristocrat = 0x44c,
    [StrRefAttr(70, "UI")]
    Asmodeus = 0x4b0,
    [StrRefAttr(0x47, "UI")]
    Assassin = 0x514,
    [StrRefAttr(0x48, "UI")]
    Attack = 0x578,
    [StrRefAttr(0x49, "UI")]
    Automaton = 0x5dc,
    [StrRefAttr(0x4a, "UI")]
    Axe = 0x640,
    [StrRefAttr(0x25b, "UI")]
    Azlanti = 0x65e,
    [StrRefAttr(0x4b, "UI")]
    Bandit = 0x672,
    [StrRefAttr(0x4c, "UI")]
    Barbarian = 0x68b,
    [StrRefAttr(0x4d, "UI")]
    Bard = 0x6a4,
    [StrRefAttr(0x4e, "UI")]
    Basic = 0x708,
    [StrRefAttr(0x4f, "UI")]
    Besmara = 0x76c,
    [StrRefAttr(0x1e1, "UI")]
    Blacksmith = 0x7b2,
    [StrRefAttr(80, "UI")]
    Bludgeoning = 0x7d0,
    [StrRefAttr(0x51, "UI")]
    Book = 0x834,
    [StrRefAttr(0x52, "UI")]
    Bow = 0x898,
    [StrRefAttr(0x53, "UI")]
    Bugbear = 0x8b1,
    [StrRefAttr(0x54, "UI")]
    Bunyip = 0x8ca,
    [StrRefAttr(0x55, "UI")]
    Cache = 0x8e3,
    [StrRefAttr(0x56, "UI")]
    Calistria = 0x8fc,
    [StrRefAttr(0x57, "UI")]
    Cannibal = 0x92e,
    [StrRefAttr(0x58, "UI")]
    Captain = 0x960,
    [StrRefAttr(0x59, "UI")]
    CaydenCailean = 0x9c4,
    [StrRefAttr(90, "UI")]
    Chain = 0xa28,
    [StrRefAttr(0x5b, "UI")]
    Cleric = 0xa8c,
    [StrRefAttr(0x5c, "UI")]
    Clothing = 0xaf0,
    [StrRefAttr(0x5d, "UI")]
    Club = 0xb54,
    [StrRefAttr(0x5e, "UI")]
    Cold = 0xbb8,
    [StrRefAttr(0x5f, "UI")]
    Combat = 0xbbd,
    [StrRefAttr(0x60, "UI")]
    Conjurer = 0xbc2,
    [StrRefAttr(0x61, "UI")]
    Construct = 0xbcc,
    [StrRefAttr(0x21b, "UI")]
    Corrupted = 0xbc7,
    [StrRefAttr(0x62, "UI")]
    Cougar = 0xbd6,
    [StrRefAttr(0x63, "UI")]
    Cultist = 0xbe0,
    [StrRefAttr(100, "UI")]
    Curse = 0xbea,
    [StrRefAttr(0x65, "UI")]
    Cyclops = 0xbf4,
    [StrRefAttr(0x66, "UI")]
    Dart = 0xc1c,
    [StrRefAttr(0x67, "UI")]
    Demon = 0xc80,
    [StrRefAttr(0x68, "UI")]
    Desna = 0xce4,
    [StrRefAttr(0x69, "UI")]
    Devil = 0xd16,
    [StrRefAttr(0x6a, "UI")]
    Divine = 0xd48,
    [StrRefAttr(0x6b, "UI")]
    Dragon = 0xdac,
    [StrRefAttr(0x6c, "UI")]
    Druid = 0xe10,
    [StrRefAttr(0x6d, "UI")]
    Dwarf = 0xe42,
    [StrRefAttr(110, "UI")]
    Electricity = 0xe74,
    [StrRefAttr(0x6f, "UI")]
    Elemental = 0xea6,
    [StrRefAttr(0x70, "UI")]
    Elf = 0xed8,
    [StrRefAttr(0x71, "UI")]
    Elite = 0xf3c,
    [StrRefAttr(0x72, "UI")]
    Enshrouded = 0xfa0,
    [StrRefAttr(0x73, "UI")]
    Erastil = 0x1004,
    [StrRefAttr(0x74, "UI")]
    Evoker = 0x1081,
    [StrRefAttr(0x75, "UI")]
    Female = 0x109a,
    [StrRefAttr(0x76, "UI")]
    Fey = 0x10b3,
    [StrRefAttr(0x77, "UI")]
    Fighter = 0x1068,
    [StrRefAttr(120, "UI")]
    Finesse = 0x10cc,
    [StrRefAttr(0x79, "UI")]
    Fire = 0x1130,
    [StrRefAttr(0x7a, "UI")]
    Firearm = 0x1194,
    [StrRefAttr(0x7b, "UI")]
    Force = 0x11f8,
    [StrRefAttr(0x7c, "UI")]
    Gambling = 0x125c,
    [StrRefAttr(0x7d, "UI")]
    Gargoyle = 0x128e,
    [StrRefAttr(0x7e, "UI")]
    Genie = 0x12c0,
    [StrRefAttr(0x7f, "UI")]
    Geryon = 0x1324,
    [StrRefAttr(0x80, "UI")]
    Ghast = 0x133d,
    [StrRefAttr(0x81, "UI")]
    Ghost = 0x1356,
    [StrRefAttr(130, "UI")]
    Ghoul = 0x136f,
    [StrRefAttr(0x83, "UI")]
    Giant = 0x1388,
    [StrRefAttr(0x84, "UI")]
    Gnome = 0x13ec,
    [StrRefAttr(0x85, "UI")]
    Goblin = 0x1450,
    [StrRefAttr(0x86, "UI")]
    Golem = 0x1482,
    [StrRefAttr(0x87, "UI")]
    Gorum = 0x14b4,
    [StrRefAttr(0x88, "UI")]
    Gozreh = 0x1518,
    [StrRefAttr(0x89, "UI")]
    Gunslinger = 0x1536,
    [StrRefAttr(0x8a, "UI")]
    Hag = 0x155e,
    [StrRefAttr(0x8b, "UI")]
    HalfElf = 0x157c,
    [StrRefAttr(0x8d, "UI")]
    Halfling = 0x1644,
    [StrRefAttr(140, "UI")]
    HalfOrc = 0x15e0,
    [StrRefAttr(0x8e, "UI")]
    Hammer = 0x16a8,
    [StrRefAttr(0x8f, "UI")]
    Harpy = 0x16da,
    [StrRefAttr(0x90, "UI")]
    Healing = 0x170c,
    [StrRefAttr(0x91, "UI")]
    HeavyArmor = 0x1770,
    [StrRefAttr(0x92, "UI")]
    Hellknight = 0x17a2,
    [StrRefAttr(0x93, "UI")]
    Hireling = 0x17d4,
    [StrRefAttr(0x94, "UI")]
    Hobgoblin = 0x1806,
    [StrRefAttr(0x95, "UI")]
    Hshurha = 0x1838,
    [StrRefAttr(150, "UI")]
    Human = 0x189c,
    [StrRefAttr(0x97, "UI")]
    Illusionist = 0x18ba,
    [StrRefAttr(0x98, "UI")]
    Incorporeal = 0x18e2,
    [StrRefAttr(0x99, "UI")]
    Iomedae = 0x1900,
    [StrRefAttr(0x9a, "UI")]
    Irori = 0x1964,
    [StrRefAttr(0x9b, "UI")]
    Kelizandri = 0x19c8,
    [StrRefAttr(0x9c, "UI")]
    Knife = 0x1a2c,
    [StrRefAttr(0x9d, "UI")]
    Kobold = 0x1a5e,
    [StrRefAttr(0x9e, "UI")]
    Lamashtu = 0x1a90,
    [StrRefAttr(0x9f, "UI")]
    Lamia = 0x1aae,
    [StrRefAttr(160, "UI")]
    Lich = 0x1ad6,
    [StrRefAttr(0xa1, "UI")]
    LightArmor = 0x1af4,
    [StrRefAttr(0xa2, "UI")]
    Liquid = 0x1b58,
    [StrRefAttr(0xa3, "UI")]
    Lock = 0x1b8a,
    [StrRefAttr(0xa4, "UI")]
    Lycanthrope = 0x1bbc,
    [StrRefAttr(0xa5, "UI")]
    Mace = 0x1c20,
    [StrRefAttr(0xa6, "UI")]
    Magic = 0x1c84,
    [StrRefAttr(0xa7, "UI")]
    Manticore = 0x1cb6,
    [StrRefAttr(0xa8, "UI")]
    Marid = 0x1ce8,
    [StrRefAttr(0xa9, "UI")]
    Mayor = 0x1d4c,
    [StrRefAttr(170, "UI")]
    Melee = 0x1db0,
    [StrRefAttr(0xab, "UI")]
    Mental = 0x1e14,
    [StrRefAttr(0xac, "UI")]
    Merfolk = 0x1e46,
    [StrRefAttr(0xad, "UI")]
    Milani = 0x1e78,
    [StrRefAttr(0xae, "UI")]
    Monk = 0x1e82,
    [StrRefAttr(0xaf, "UI")]
    Mummy = 0x1e8c,
    [StrRefAttr(0xb0, "UI")]
    Mutant = 0x1e96,
    [StrRefAttr(0xb1, "UI")]
    Naga = 0x1ea0,
    [StrRefAttr(0xb2, "UI")]
    Necromancer = 0x1eaa,
    [StrRefAttr(0xb3, "UI")]
    Nereid = 0x1edc,
    [StrRefAttr(0x1dc, "UI")]
    Nethys = 0x1ee6,
    [StrRefAttr(180, "UI")]
    Noble = 0x1f40,
    [StrRefAttr(0, "UI")]
    None = 0,
    [StrRefAttr(0xb5, "UI")]
    Norgorber = 0x1fa4,
    [StrRefAttr(0xb6, "UI")]
    Nymph = 0x2008,
    [StrRefAttr(0xb7, "UI")]
    Object = 0x206c,
    [StrRefAttr(0xb8, "UI")]
    Obstacle = 0x209e,
    [StrRefAttr(0xb9, "UI")]
    Offhand = 0x20d0,
    [StrRefAttr(0xba, "UI")]
    Ogre = 0x20ee,
    [StrRefAttr(0xbb, "UI")]
    Ogrekin = 0x2116,
    [StrRefAttr(0xbc, "UI")]
    Oni = 0x2134,
    [StrRefAttr(0xbd, "UI")]
    Ooze = 0x2152,
    [StrRefAttr(190, "UI")]
    Oracle = 0x217a,
    [StrRefAttr(0x25f, "UI")]
    Orc = 0x217f,
    [StrRefAttr(0xbf, "UI")]
    Outsider = 0x2198,
    [StrRefAttr(0x25a, "UI")]
    Paladin = 0x21ca,
    [StrRefAttr(0xc0, "UI")]
    Pharasma = 0x21fc,
    [StrRefAttr(0xc1, "UI")]
    Pick = 0x22c4,
    [StrRefAttr(0xc2, "UI")]
    Piercing = 0x2328,
    [StrRefAttr(0xc3, "UI")]
    Pirate = 0x238c,
    [StrRefAttr(0xc4, "UI")]
    Pixie = 0x23f0,
    [StrRefAttr(0xc5, "UI")]
    Plant = 0x2422,
    [StrRefAttr(0xc6, "UI")]
    Poison = 0x2454,
    [StrRefAttr(0xc7, "UI")]
    Polearm = 0x24b8,
    [StrRefAttr(200, "UI")]
    Ranged = 0x251c,
    [StrRefAttr(0xc9, "UI")]
    Ranger = 0x2580,
    [StrRefAttr(0xca, "UI")]
    Reliable = 0x25e4,
    [StrRefAttr(0xcb, "UI")]
    Revenant = 0x2616,
    [StrRefAttr(0xcc, "UI")]
    Rogue = 0x2648,
    [StrRefAttr(0xcd, "UI")]
    Sage = 0x26ac,
    [StrRefAttr(0xce, "UI")]
    Sarenrae = 0x2710,
    [StrRefAttr(0xcf, "UI")]
    Scout = 0x2774,
    [StrRefAttr(0xd0, "UI")]
    Scythe = 0x27d8,
    [StrRefAttr(0xd1, "UI")]
    Shelyn = 0x283c,
    [StrRefAttr(210, "UI")]
    Shield = 0x28a0,
    [StrRefAttr(0xd3, "UI")]
    Shipbuilder = 0x2904,
    [StrRefAttr(0xd4, "UI")]
    Shopkeeper = 0x2968,
    [StrRefAttr(0xd5, "UI")]
    Sihedron = 0x29cc,
    [StrRefAttr(0xd6, "UI")]
    Siren = 0x29fe,
    [StrRefAttr(0xd7, "UI")]
    Sivanah = 0x2a30,
    [StrRefAttr(0xd8, "UI")]
    Skeleton = 0x2a4e,
    [StrRefAttr(0xd9, "UI")]
    Skirmish = 0x2a6c,
    [StrRefAttr(0xda, "UI")]
    Slashing = 0x2a94,
    [StrRefAttr(0xdb, "UI")]
    Sling = 0x2af8,
    [StrRefAttr(220, "UI")]
    Smuggler = 0x2b5c,
    [StrRefAttr(0xdd, "UI")]
    Sorcerer = 0x2bc0,
    [StrRefAttr(0xde, "UI")]
    Spear = 0x2c24,
    [StrRefAttr(0xdf, "UI")]
    Special = 0x2c88,
    [StrRefAttr(0xe0, "UI")]
    Spider = 0x2cba,
    [StrRefAttr(0xe1, "UI")]
    Spy = 0x2cec,
    [StrRefAttr(0xe2, "UI")]
    Spymaster = 0x2d1e,
    [StrRefAttr(0xe3, "UI")]
    Staff = 0x2d50,
    [StrRefAttr(0xe4, "UI")]
    Surgeon = 0x2db4,
    [StrRefAttr(0xe5, "UI")]
    Swarm = 0x2de6,
    [StrRefAttr(230, "UI")]
    Swashbuckling = 0x2e18,
    [StrRefAttr(0xe7, "UI")]
    Sword = 0x2e7c,
    [StrRefAttr(0xe8, "UI")]
    Task = 0x2eae,
    [StrRefAttr(0xe9, "UI")]
    Tengu = 0x2ee0,
    [StrRefAttr(0xea, "UI")]
    Thug = 0x2f44,
    [StrRefAttr(0xeb, "UI")]
    Tool = 0x2fa8,
    [StrRefAttr(0xec, "UI")]
    Torag = 0x300c,
    [StrRefAttr(0xed, "UI")]
    Transmuter = 0x303e,
    [StrRefAttr(0xee, "UI")]
    Trap = 0x3070,
    [StrRefAttr(0xef, "UI")]
    Treant = 0x308e,
    [StrRefAttr(240, "UI")]
    Troll = 0x30b6,
    [StrRefAttr(0xf1, "UI")]
    TwoHanded = 0x30d4,
    [StrRefAttr(0xf2, "UI")]
    Undead = 0x30ed,
    [StrRefAttr(0xf3, "UI")]
    Vampire = 0x3106,
    [StrRefAttr(0xf4, "UI")]
    Veteran = 0x3138,
    [StrRefAttr(0xf5, "UI")]
    Wand = 0x319c,
    [StrRefAttr(0xf6, "UI")]
    Warrior = 0x3200,
    [StrRefAttr(0xf7, "UI")]
    Weather = 0x326e,
    [StrRefAttr(0xf8, "UI")]
    Wererat = 0x3278,
    [StrRefAttr(0xf9, "UI")]
    Wight = 0x3282,
    [StrRefAttr(250, "UI")]
    Witch = 0x328c,
    [StrRefAttr(0xfb, "UI")]
    Wizard = 0x3296,
    [StrRefAttr(0xfc, "UI")]
    Worm = 0x32a0,
    [StrRefAttr(0xfd, "UI")]
    Xulgath = 0x32aa,
    [StrRefAttr(0xfe, "UI")]
    Yeti = 0x32b4,
    [StrRefAttr(0xff, "UI")]
    Zogmugot = 0x32c8,
    [StrRefAttr(0x100, "UI")]
    Zombie = 0x332c,
    [StrRefAttr(480, "UI")]
    Zorangel = 0x335e
}

