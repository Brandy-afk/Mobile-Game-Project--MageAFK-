using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MageAFK.Core;
using MageAFK.Player;
using MageAFK.Pooling;
using MageAFK.Stats;
using MageAFK.UI;
using Sirenix.OdinInspector;
using MageAFK.Management;
using MageAFK.Tools;



namespace MageAFK.Spells
{


  public abstract class Spell : SerializedScriptableObject
  {

    public const int CurrentVersion = 0;

    #region Variables

    [TabGroup("Spell")] public SpellIdentification iD;
    [TabGroup("Spell"), ReadOnly] public string spellName;
    [TabGroup("Spell"), TextArea(10, 15)] public string desc = "Default Description.";
    [TabGroup("Spell")] public SpellType type;

    [Space(5), TabGroup("Spell"), Header("WARNING - Changing Status will change stats")]
    [TabGroup("Spell")] public StatusType effect;

    [Header("-----------------------")]
    [Space(10), TabGroup("Spell")] public SpellElement element;
    [TabGroup("Spell")] public SpellBook book;
    [TabGroup("Spell")] public SpellDamageType damageType;

    [Header("Color Info")]
    [TabGroup("Spell")] public ColorPair shaderColors = new(Color.white, Color.white);
    //For ui 
    [Header("For Name UI")]
    [TabGroup("Spell")] public Color topColor = Color.white;
    [TabGroup("Spell")] public Color bottomColor = Color.white;

    [Header("Make sure its UI Animator")]
    [TabGroup("Spell")] public RuntimeAnimatorController controller;

    [TabGroup("Spell"), PreviewField] public Sprite image;
    [TabGroup("Spell")] public GameObject prefab;

    [TabGroup("Spell")] public int cost = 5000;

    [Space(5)]
    [Header("If spell casts spell, leave null otherwise")]
    [TabGroup("Spell")] public PoolSpellClass[] spellsToPool = null;

    [Space(5)]
    [TabGroup("Spell"), Header("Leave as 'None' if target can be changed")]
    public FocusEntity forcedFocus = FocusEntity.None;


    #region  Stats variables
    [TabGroup("Stats"), Header("Stat to add")]
    public Stat statToAdd;
    [TabGroup("Stats"), Button("Add Stat")]
    public void Add()
    {
      if (!spellStats.ContainsKey(statToAdd))
      {
        var stat = statToAdd;
        spellStats[stat] = new SpellStat(stat)
        {
          statType = stat,
          upgradable = true
        };
      }
    }

    [TabGroup("Stats"), Button("Press to organize")]
    public void OrganizeSortedList()
    {
      if (spellStats == null || spellStats.Count < 1) return;
      sortedStats = spellStats.Values.Where(stat => !stat.hideStat).Select(stat => stat.statType)
       .OrderBy(e => e.ToString())
       .ToArray();
    }

    [TabGroup("Stats"), Title("Fields - Negative Field: -25 => -25% / Positive Field: 25 => 25%")] public Dictionary<Stat, SpellStat> spellStats;
    [TabGroup("Stats"), ReadOnly] public Stat[] sortedStats;
    #endregion

    [TabGroup("Records")]
    public Dictionary<SpellRecordID, float> recordDict = new Dictionary<SpellRecordID, float>
    {
      {SpellRecordID.Damage, 0},
      {SpellRecordID.Kills, 0},
      {SpellRecordID.Upgraded, 0},
      {SpellRecordID.Cast, 0}
    };


    //PRIVATE 
    private int level;
    private bool isUnlocked = true;
    private SpellSlotIndex index = SpellSlotIndex.None;
    private FocusEntity focus = FocusEntity.ClosestTarget;

    public int Level => level;
    public SpellSlotIndex SlotIndex => index;
    public bool IsUnlocked => isUnlocked;
    public FocusEntity Focus => forcedFocus == FocusEntity.None ? focus : forcedFocus;

    #endregion


    #region / Data Manipulation /

    #region LoadData
    public void LoadData(PermSpellData data)
    {
      // Update old data to current version
      while (data.version < CurrentVersion)
      {
        data = UpdateData(data);
      }
      // load data as normal...
      recordDict = new Dictionary<SpellRecordID, float>(data.recordDict);
    }


    public void LoadData(TempSpellData data)
    {
      while (data.version < CurrentVersion)
      {
        data = UpdateData(data);
      }
      // load data as normal...
      // Load data from SpellData into this Spell
      SetUnlock(data.isUnlocked);
      SetSlotIndex(data.index);
      SetLevel(data.spellLevel);
      LoadSpellStats(data.statData);
    }

    private void LoadSpellStats(List<SpellStatData> stats)
    {
      foreach (var data in stats)
      {
        if (spellStats.TryGetValue(data.stat, out SpellStat stat))
        {
          stat.runtimeValue = data.runtimeValue;
          stat.level = data.level;
        }
      }
    }

    #endregion

    #region Update Data
    private PermSpellData UpdateData(PermSpellData oldData)
    {
      // create a copy of the old data
      PermSpellData newData = new PermSpellData(oldData);
      // update version number
      newData.version++;
      // apply updates based on version
      switch (newData.version)
      {
        case 2:
          // Changes for version 2... Add more as needed...
          break;
          // add more cases as needed when you update your game
      }
      return newData;
    }

    private TempSpellData UpdateData(TempSpellData oldData)
    {
      // create a copy of the old data
      TempSpellData newData = new TempSpellData(oldData);
      // update version number
      newData.version++;
      // apply updates based on version
      switch (newData.version)
      {
        default:
          break;
      }
      return newData;
    }
    #endregion

    #region Save Data
    public PermSpellData SavePermData()
    {
      return new PermSpellData
      {
        version = CurrentVersion,
        iD = iD,

        recordDict = recordDict,
      };
    }

    public TempSpellData SaveTempData()
    {
      var data = new TempSpellData
      {
        version = CurrentVersion,
        iD = iD,
        isUnlocked = IsUnlocked,
        index = SlotIndex,
        spellLevel = Level,
        statData = new List<SpellStatData>()
      };

      foreach (var pair in spellStats)
        data.statData.Add(new SpellStatData(pair.Key, pair.Value.level, pair.Value.runtimeValue));

      return data;
    }

    #endregion

    #endregion

    #region Helpers

    public virtual void OnEnable() => ResetSpell();
    public virtual void ResetSpell()
    {

      SetLevel(0);

      if (spellStats != null)
      {
        foreach (var stat in spellStats)
        {
          stat.Value.runtimeValue = stat.Value.baseValue;
          stat.Value.level = 0;
        }
      }

      //TODO need to change later;
      if (recordDict != null && recordDict.Count > 0)
      {
        foreach (SpellRecordID recordID in Enum.GetValues(typeof(SpellRecordID)))
        {
          if (recordDict.ContainsKey(recordID))
          {
            recordDict[recordID] = 0;
          }
        }
      }
      //Testing
      SetSlotIndex(SpellSlotIndex.None);
      SetFocusEntitiy(FocusEntity.ClosestTarget);
      SetUnlock(true);
    }


    public void SetUnlock(bool state) => isUnlocked = state;
    public void SetSlotIndex(SpellSlotIndex index) => this.index = index;
    public void SetFocusEntitiy(FocusEntity focusEntity) => this.focus = focusEntity;


    public static void SpawnEffect(Vector2 pos, SpellEffectAnimation animation)
    {
      var target = SpellSpawn(SpellIdentification.SpellUtility_Effect, pos);
      if (target != null) target.GetComponent<Animator>().runtimeAnimatorController = GameResources.Spell.ReturnEffect(animation);
    }

    public static void SpawnEffect(Vector2 pos, SpellIdentification iD)
    {
      var target = SpellSpawn(SpellIdentification.SpellUtility_Effect, pos);
      if (target != null) target.GetComponent<Animator>().runtimeAnimatorController = GameResources.Spell.ReturnEffect(iD);
    }

    protected static GameObject SpellSpawn(SpellIdentification iD, Vector2 pos, bool state = true, SpellIdentification parent = SpellIdentification.None)
    {
      GameObject instance = ServiceLocator.Get<SpellPooler>().Get(iD, parent);

      if (instance != null)
      {
        instance.transform.position = pos;
        instance.SetActive(state);
      }

      return instance;
    }

    #endregion

    #region Overloads
    public static bool operator ==(Spell lhs, Spell rhs)
    {
      if (ReferenceEquals(lhs, null))
      {
        return ReferenceEquals(rhs, null);
      }
      return lhs.Equals(rhs);
    }

    public static bool operator !=(Spell lhs, Spell rhs)
    {
      return !(lhs == rhs);
    }

    public override bool Equals(object other)
    {
      Spell obj = other as Spell;
      if (other == null) return false;
      return iD == obj.iD;
    }
    public override int GetHashCode()
    {
      return iD.GetHashCode();
    }

    #endregion

    #region VirtualMethods
    public abstract void Activate();

    public virtual void OnCast()
    {
      AppendRecord(SpellRecordID.Cast, 1);
      ServiceLocator.Get<PlayerData>().AddStatValue(PlayerStatisticEnum.SpellCasts, 1);
    }

    //If needed
    public virtual void OnWaveOver() { }

    #endregion

    #region Records

    private event Action<SpellRecordID, float> OnRecordModififed;

    public void AppendRecord(SpellRecordID type, float quantity)
    {
      if (recordDict.ContainsKey(type))
      {
        recordDict[type] += quantity;
        OnRecordModififed?.Invoke(type, recordDict[type]);

        if (WaveHandler.WaveState == WaveState.None)
          SaveManager.Save(ServiceLocator.Get<SpellHandler>().SaveData(), DataType.SpellData);
      }
      else
      {
        Debug.Log($"Bad Spell Record Type : {type}");
      }
    }


    public void SubscribeToRecordEvent(Action<SpellRecordID, float> sub, bool state)
    {
      if (state)
        OnRecordModififed += sub;
      else
        OnRecordModififed -= sub;
    }

    #endregion

    #region Stats

    public SpellStat GetStat(Stat stat)
    {
      if (spellStats.ContainsKey(stat))
        return spellStats[stat];
      else
        Debug.Log("Stat not found " + stat.ToString());
      return null;
    }

    /// <summary>
    /// Returns the runtime value of the stat. Unless modify value = false, it will return the modified version. 
    /// </summary>
    /// <param name="stat">The stat to return.</param>
    /// <param name="modifyValue">If false it will return its own raw value.</param>
    /// <param name="modStat">Change if you want the stat to be modified by a different stat then itself.</param>
    /// <returns></returns>
    public virtual float ReturnStatValue(Stat stat, bool modifyValue = true, Stat modStat = Stat.None)
    {
      try
      {
        var runtimeValue = spellStats[stat].runtimeValue;
        modStat = modStat != Stat.None ? modStat : stat;

        if (modifyValue)
        {
          // #if DEBUG
          //           Debug.Log($"Returning {ServiceLocator.Get<PlayerStatHandler>().ReturnModifiedValue(modStat, runtimeValue)} for {stat}");
          // #endif
          return ServiceLocator.Get<PlayerStatHandler>().ReturnModifiedValue(modStat, runtimeValue);
        }
        else
        {
          // #if DEBUG
          //           Debug.Log($"Returning {runtimeValue} for {stat}");
          // #endif
          return runtimeValue;
        }
      }
      catch (KeyNotFoundException)
      {
        // #if DEBUG
        //         Debug.Log($"Stat not found : {stat}");
        // #endif
        return 0;
      }
    }

    #region Static
    private static readonly Dictionary<StatusType, Stat> effectPairs = new Dictionary<StatusType, Stat>{
      {StatusType.Slow, Stat.Slow},
      {StatusType.Burn, Stat.BurnDamage},
      {StatusType.Poison, Stat.Poison},
      {StatusType.Corrupt, Stat.CorruptionChance},
      {StatusType.Bleed, Stat.BleedDamage},
      {StatusType.Smite, Stat.SmiteDamage},
      {StatusType.Weaken, Stat.Weaken}
    };

    public static float ReturnEffectMagnitude(Spell spell)
    {
      try
      {
        return spell.ReturnStatValue(effectPairs[spell.effect]);
      }
      catch (KeyNotFoundException)
      {
        return 0;
      }
    }

    #endregion
    #endregion

    #region Level
    public event Action OnLevelUp;
    public void LeveUp()
    {
      level++;
      OnLevelUp?.Invoke();
    }

    public void SetLevel(int level) => this.level = level;

    public void SubscribeToLevelChanged(Action sub, bool state)
    {
      if (state)
      {
        OnLevelUp += sub;
        OnLevelUp?.Invoke();
      }
      else
      {
        OnLevelUp -= sub;
      }

    }


    #endregion

  }
}

