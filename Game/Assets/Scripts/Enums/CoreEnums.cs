namespace MageAFK.Core
{
  public enum Entities
  {
    All,
    Ore,
    Plant,
    Enemy,
    Animal
  }

  public enum EntityIdentification
  {
    None = 0,
    Frostfang = -1,
    Howler = -2,
    Pyroclaw = -3,
    Lycan = -4,
    Shadow = -5,
    Wolf = -6,
    Wraithwolf = -7,
    Shaman = -8,
    Malevolith = -9,
    Clawfiend = -10,
    IronOre = -11,
    CopperOre = -12,
    TinOre = -13,
    GoldOre = -14,
    AngelicOre = -15,
    PineTree = -16,
    BirchTree = -17,
    MapleTree = -18,
    OakTree = -19,
    Chicken = -20,
    Pig = -21,
    Lamb = -22,
    Cow = -23,
    BrownSpider = -24,
    Toxibite = -25,
    Crimsonstalker = -26,
    NightCrawler = -27,
    Voidfiend = -28,
    Voidbane = -30,
    Mutant = -31,
    Warlock = -35,
    BlackHorn = -36,
    Ogre = -37,
    Husk = -38,
    Reaver = -39,
    Hoplite = -40,
    Abysswing = -41,
    Necrowing = -42,
    Warlord = -43,
    Shieldbearer = -44,
    Starweaver = -45,
    Swiftstrider = -46,
    Eldarion = -47,
    Shadowshot = -48,
    Shadowcaster = -49,
    Twinblade = -50,
    Sniper = -51,
    Harvester = -52,
    Bonemen = -53,
    Necroaxe = -54,
    Lich = -55,
    Reaper = -56,
    Stormrider = -57,
    Ironclad = -58,
    Lancer = -59,
    Hurler = -60,
    Berserker = -61,
    Runemaster = -62,
    Marksmen = -63,
    Warden = -64,
    Vanguard = -65,
    Ranger = -66,
    Noble = -67,
    Sentinel = -68,
    Sharpshot = -69,
    Bloodaxe = -70,
    Raider = -71,
    Brute = -72,
    Voidseer = -73,
    Tamer = -74,
    Titan = -75,
    Slender = -76
  }



  public enum EnemyRace
  {
    Humanoid,
    Elemental,
    Creature,
    Orc,
    Undead,
    Demon,
    None
  }


  public enum FocusEntity
  {
    ClosestTarget = 5,
    Enemy1 = 0,
    Enemy2 = 1,
    Enemy3 = 2,
    Random = 4,
    None = -1,
    FurthestTarget = 6
  }

  //For later, its important to understand these titles only point torwards enemies, all other entities are "All"
  public enum MobGrade
  {
    Soldier,
    Captain,
    General,
    Boss,
    All
  }

  public enum EnemyType
  {
    Melee,
    Ranged,
    Magic
  }

  public enum Priority
  {
    First = 0,
    Middle = 1,
    Last = 2
  }

  public enum StatusIdentifiers
  {
    ChaoticAura = -100,
  }


}