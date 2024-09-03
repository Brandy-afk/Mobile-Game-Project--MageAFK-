
using System.Collections.Generic;
using UnityEngine;
using MageAFK.Management;
using System;
using MageAFK.UI;
using Sirenix.OdinInspector;
using UnityEditor;
using System.Linq;
using MageAFK.Core;

namespace MageAFK.Spells
{

  public class SpellHandler : SerializedMonoBehaviour, IData<PermSpellDataCollection>, ITempData<TempSpellData[]>
  {

    [SerializeField] private Dictionary<SpellIdentification, Spell> spells = new();
    [SerializeField, Tooltip("Has to do with players level (during waves)\nDo not alter unlocked")] private Dictionary<SpellType, (int req, bool unlocked)> unlockReqs;
    [SerializeField] private SpellSlotHandler spellSlotTabGroup;
    [SerializeField] private SpellTypeGroup spellTypeGroup;

    public SpellIdentification defaultSpell = SpellIdentification.Voidshade;

    [Button("Save all changes on scriptables")] //FOR TESTING
    public void SaveAllChanges()
    {
      foreach (var slot in spells)
      {
        EditorUtility.SetDirty(slot.Value);
        if (slot.Value.prefab != null)
          EditorUtility.SetDirty(slot.Value.prefab);
      }

      AssetDatabase.Refresh(); // Refresh the AssetDatabase to show the new script in Unity Editor
      AssetDatabase.SaveAssets();
    }




    #region Save/Load
    public void InitializeData(PermSpellDataCollection data)
    {
      foreach (var entry in data.data)
      {
        if (spells.ContainsKey(entry.iD))
          spells[entry.iD].LoadData(entry);
      }
    }

    public PermSpellDataCollection SaveData()
    {
      List<PermSpellData> dataList = new();
      foreach (KeyValuePair<SpellIdentification, Spell> entry in spells)
      {
        PermSpellData data = entry.Value.SavePermData();
        dataList.Add(data);
      }
      return new PermSpellDataCollection(dataList);
    }

    public void InitializeTempData(TempSpellData[] data)
    {
      foreach (var tempData in data)
      {
        if (spells.TryGetValue(tempData.iD, out Spell spell))
        {
          spell.LoadData(tempData);
        }
        else
          Debug.Log($" Spell ID not found : {tempData.iD}");
      }
    }

    public TempSpellData[] SaveTempData() => spells.Select(pair => pair.Value.SaveTempData()).ToArray();

    #endregion
    private void Awake()
    {
      ServiceLocator.RegisterService<IData<PermSpellDataCollection>>(this);
      ServiceLocator.RegisterService<ITempData<TempSpellData[]>>(this);
      ServiceLocator.RegisterService(this);
      ConfigureSpellsSiegeEnd();
    }

    private void Start()
    {
      WaveHandler.SubToSiegeEvent(OnSiegeEvent, true);
      ServiceLocator.Get<LevelHandler>().SubscribeToLevelChanged(OnLevelChanged, true);
    }

    #region Events
    private void OnSiegeEvent(Status state)
    {
      if (state == Status.Start)
        ConfigureSpellsForSiege();
      else if (state == Status.End_CleanUp)
        ConfigureSpellsSiegeEnd();
    }

    private void OnLevelChanged(int level)
    {
      foreach (SpellType type in Enum.GetValues(typeof(SpellType)))
      {
        try
        {
          var values = unlockReqs[type];

          if (values.unlocked == true) continue;
          if (level >= values.req || WaveHandler.WaveState == WaveState.None)
          {
            spellTypeGroup.ToggleTab(true, type);
            unlockReqs[type] = (values.req, true);
            if (!GameManager.IsLoad && WaveHandler.WaveState != WaveState.None)
              ServiceLocator.Get<IndicatorHandler>().SetUIChain(UIPanel.Book_Spell);
          }
        }
        catch (KeyNotFoundException)
        {
          continue;
        }
      }
    }

    #endregion


    #region  Spell Functions

    public Spell GetSpellData(SpellIdentification key)
    {
      if (spells.ContainsKey(key))
        return spells[key];
      else
      {
        Debug.Log("Spell Data Not Found");
        return null;
      }
    }

    public Dictionary<SpellIdentification, Spell> ReturnSpellDict() => spells;

    #endregion

    #region  Spell Configuration
    public void ConfigureSpellsForSiege()
    {
      foreach (var item in spells)
      {
        Spell spell = item.Value;
        if (spell.iD == defaultSpell && !GameManager.IsLoad)
        {
          spell.SetUnlock(true);
          spellSlotTabGroup.EquipSpell(spell, SpellSlotIndex.Spell1, spell.type);
        }
        else if (spell.SlotIndex == SpellSlotIndex.None)
        {
          spell.SetUnlock(false);
        }
      }

      foreach (SpellType type in Enum.GetValues(typeof(SpellType)))
      {
        try
        {
          unlockReqs[type] = (unlockReqs[type].req, false);
          spellTypeGroup.ToggleTab(false, type);
        }
        catch (KeyNotFoundException)
        {
          continue;
        }
      }

    }

    public void ConfigureSpellsSiegeEnd()
    {
      foreach (var item in spells)
      {
        Spell spell = item.Value;

        if (spell.SlotIndex != SpellSlotIndex.None)
          spellSlotTabGroup.EquipSpell(null, spell.SlotIndex, spell.type);

        spell.ResetSpell();
      }
    }


    #endregion


  }

  [Serializable]
  public class PermSpellData
  {
    //Data version
    public SpellIdentification iD;
    public int version;

    //Data from SpellRecords
    public Dictionary<SpellRecordID, float> recordDict;

    public PermSpellData(PermSpellData source)
    {
      iD = source.iD;
      version = source.version;
      recordDict = new Dictionary<SpellRecordID, float>(source.recordDict);
    }

    public PermSpellData() { }

  }

  [Serializable]
  public class TempSpellData
  {
    //Data version
    public SpellIdentification iD;
    public int version;
    //Data from SpellInformation
    public bool isUnlocked;
    public SpellSlotIndex index;
    //Data from SpellStatClass
    public List<SpellStatData> statData;

    //Data from SpellUpgradeInfo
    public int spellLevel;

    public TempSpellData(TempSpellData source)
    {
      iD = source.iD;
      version = source.version;
      isUnlocked = source.isUnlocked;
      index = source.index;
      spellLevel = source.spellLevel;

      statData = new List<SpellStatData>();
      foreach (var data in source.statData)
        statData.Add(new SpellStatData(data.stat, data.level, data.runtimeValue));
    }

    public TempSpellData()
    {

    }
  }



  [Serializable]
  public class PermSpellDataCollection
  {
    public List<PermSpellData> data;

    public PermSpellDataCollection(List<PermSpellData> data)
    {
      this.data = data;
    }
  }

}