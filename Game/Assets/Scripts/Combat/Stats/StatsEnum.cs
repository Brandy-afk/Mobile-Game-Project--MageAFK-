namespace MageAFK.Stats
{
  // MOD - intended for modification of other values

  // VALUE - being used as a value to alter gameplay. Can be used as a modification.

  public enum Stat
  {
    None = 0,
    Armour = 1, //value
    Health = 2, // mod
    AttackRange = 3, // mod
    SoulDamage = 4, // mod
    DemonicDamage = 5, // mod
    LifeDamage = 6, // mod
    ChaosDamage = 7, // mod
    Cooldown = 10, // mod
    Damage = 11, // mod

    PotionDuration = 12, // mod
    PotionEffect = 13, // mod

    //FOR ALL DAMAGE REDUCTION STATS, MODIFICATION NEEDS
    //TO BE POSITIVE AND LATER MADE NEGATIVE WHEN COMPUTING
    DamageReduction = 14, // mod
    SilverDrops = 19, // mod
    GemDrops = 20, // mod
    ExperienceDrops = 21, // mod

    MovementSpeed = 22, // mod
    EnemySpawnRate = 23, //value

    AimVariance = 24, // value

    //Spell Stats
    SpellDuration = 25, // value
    CritChance = 26, // value
    CritDamage = 27, // value
    PierceChance = 28, // value

    HealVamp = 29, // value

    SpellSpeed = 30, // value
    KnockBack = 31, // value
    StatusChance = 32, // value
    StatusDuration = 33, // value

    //Elemental Effects
    BurnDamage = 34, // value
    Slow = 35, // mod
    CorruptionChance = 36, // value
    SmiteDamage = 38, // value
    BleedDamage = 39, // value
    Weaken = 40, // mod

    //More Effects
    CraftingTime = 41, // mod
    SpellCost = 42, // mod
    CraftingCost = 43, // mod
    CraftingItemCostMod = 45, // mod

    ThornsDamage = 46, // value

    HealthRegen = 47, // value

    Poison = 48,
    ExecutionThreshold = 49,

    FireSpellDamage = 50,
    SpawnCap = 52,
    AfterEffectDuration = 53,
    SpecialChance = 54,
    ProcInterval = 55,
    Range = 56,
    AccelerationRate = 57,

    AreaOfEffectDamage = 58,
    DamageIncrease = 59,

    MaxTargets = 60,
    AttackCooldown = 61,

    CooldownModifier = 62,
    Frequency = 63,
    Amplitude = 64,
    DecisionInterval = 65,

    UltimateCooldownMod = 67, // Remember modifiers are not to be called to in the ///playerstathandler///, only base stats like damage instead of damage mod
                              // this is essentially just for altering of forms as in the case where we want to increase damage by a percentage 
                              // rather then a numeric value.
    ItemUpgradeCostMod = 68,
    DodgeChance = 69, /* Value */
    StormDamage = 70,
    Armour_Modifier = 71,
    MaxTriggers = 72,
    TargetsPerTrigger = 73

  }
}



