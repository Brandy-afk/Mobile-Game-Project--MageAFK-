
using System;
using UnityEngine;
using MageAFK.Stats;
using Sirenix.OdinInspector;
using static MageAFK.Core.SpellUpgradeHandler;
using MageAFK.Core;

namespace MageAFK.Spells
{
  public enum StatusType
  {
    None = 0,
    Slow = 1, // Only Ice / Slow
    Burn = 2, // Only fire / DoT
    Corrupt = 4, // only void / chance to execute
    Confuse = 5, // Storm / Confuses enemy
    Smite = 6, // Holy / Smites enemy for percentage of max hp
    Poison = 10, // Acid / Increases damage taken
    Bleed = 7,
    Weaken = 8,
    Root = 9,
    Stun = 3,
    Fear = 11,
    Multiple = 12

  }

  public enum SpellElement
  {
    None = 0,
    Fire = 1,
    Storm = 2,
    Ice = 3,
    Void = 4,
    Holy = 6,
    Corrosive = 7,
    Air = 8,
    Water = 9,
    Earth = 10,
    Blood = 11
  }
  public enum SpellBook
  {
    All,
    Life,
    Demonic,
    Soul,
    Chaos
  }

  public enum SpellDamageType
  {
    None,
    AoE,
    Single,
    Buff,
    Debuff,
    Summon,
    Random,
    Multiple

  }

  public enum SpellType
  {
    Spell = 0,
    Passive = 1,
    Ultimate = 2

  }

  public enum SpellIdentification
  {
    None = 0,
    WindBlades = 1,
    FireBrand = 2,
    EtherealSpike = 3,
    DenseVoid = 4,
    PillarOfChaos = 5,
    Acidburst = 6,
    Acidlash = 7,
    BlightBubble = 8,
    Frogspit = 9,
    Growth = 10,
    VileEruption = 11,
    InfernalSeal = 12,
    IonLagoon = 13,
    Overflow = 14,
    Retribution = 15,
    Fortify = 16,
    Veil = 17,
    Truestrike = 18,
    Quickstrike = 19,
    Doomstrike = 20,
    Deathmark = 21,
    Voidshade = 22,
    Instability = 23,
    Shadowhand = 24,
    Gloomgazer = 25,
    Soulgale = 26,
    HolyArrow = 27,
    GhastlyAura = 28,
    TerraGuardian = 29,
    StoneSpire = 30,
    Boulder = 31,
    DemonicShield = 32,
    HellishAura = 33,
    Pyrocuts = 34,
    Holix = 35,
    Thunderang = 36,
    IonTrap = 37,
    GlacialDecoy = 38,
    FrostBomb = 39,
    IceCrash = 40,
    Soulken = 41,
    HydroBeam = 42,
    SeaCyclone = 43,
    SeaStrikes = 44,
    Skewer = 45,
    NaturesFury = 46,
    TimberStrike = 47,
    Galeblade = 48,
    Tornado = 49,
    Terrorcut = 50,
    DemonicShield_Fireball = 51,
    IonLagoon_Fish = 52,
    SpellUtility_Range = 53,
    Soulken_Explosion = 55,
    SpellUtility_Effect = 56,
    SeaCyclone_WaterBlast = 57,
    TerraGuardian_Mine = 58
  }

  public enum SpellRecordID
  {
    Kills,
    Damage,
    Upgraded,
    Cast,
    GuardiansSummoned,
    MinesSet,
    WaterBlasts,
    VortexesConjured,
    CumulativeIncrease = -1,

  }

  public enum SpellEffectAnimation
  {
    None = -1,
    Thunder_1,
    SmiteEffect,
    Water_1,
    Wood_1,
    RootEffect,
    FireHitEffect_1,
    Thunder_2,
    Earth_1
  }





  [Serializable]
  public class SpellStat
  {

    [TabGroup("Values"), Header("All Percentages need to be in TRADITIONAL form (Ex: 50%)")]
    public float baseValue;
    [TabGroup("Values"), Header("Caution when editing")]
    public float runtimeValue;

    [TabGroup("Functionality")]
    public Stat statType;

    [TabGroup("Functionality")]
    public bool upgradable = true;
    [TabGroup("Functionality")]
    public bool hideStat = false;
    [TabGroup("Functionality")]
    public bool isPercentage = false;

    [TabGroup("Functionality"), Header("-1 if none for this stat")]
    public int maxLevel = -1;

    [TabGroup("Functionality")]
    public StatUpgradeRule rule = null;


    [ReadOnly, TabGroup("Functionality")]
    public int level = 0;

    public SpellStat(Stat type)
    {
      statType = type;
      baseValue = -1;
    }

    public bool IsMaxed() => maxLevel == -1 || level >= maxLevel || !upgradable || WaveHandler.WaveState == WaveState.None;
  }

  [Serializable]
  public class SpellStatData
  {
    public Stat stat;
    public int level;
    public float runtimeValue;

    public SpellStatData(Stat stat, int level, float runtime)
    {
      this.stat = stat;
      this.level = level;
      runtimeValue = runtime;
    }
  }

}


