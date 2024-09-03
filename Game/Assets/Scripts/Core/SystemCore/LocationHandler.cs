using System.Collections.Generic;
using MageAFK.UI;
using MageAFK.Management;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using MageAFK.Player;
using System.Linq;

namespace MageAFK.Core
{
  public class LocationHandler : SerializedMonoBehaviour, IData<LocationData>
  {

    [SerializeField] private Dictionary<Location, LocInfo> locationDict = new();
    [SerializeField] private LocationUI locationUI;


    public static Location currentLocation { get; private set; } = Location.Woods;

    #region Initializing - Load / Save

    private void Awake()
    {
      ServiceLocator.RegisterService(this);
      ServiceLocator.RegisterService<IData<LocationData>>(this);
    }

    private void Start() => SaveManager.OnSaveDataLoaded += () =>
  { ServiceLocator.Get<PlayerData>().SubscribeToStatAltered(PlayerStatisticEnum.MilestonesComplete, CheckIfRequirementsMet, true); };


    public void InitializeData(LocationData data)
    {
      foreach (var block in data.locDataBlocks)
      {
        if (locationDict.TryGetValue(block.Location, out LocInfo info))
        {
          info.LoadData(block);
          if (info.location != Location.Woods && block.isUnlocked)
          {
            locationUI.UnlockLocation(block.Location);
          }
        }
      }

    }

    public LocationData SaveData() => new LocationData(locationDict.Select(pair => pair.Value.SaveData()).ToArray());


    #endregion

    #region Location Map Handling
    //Called every time the player levels up to ensure that all correct locations are unlocked.
    public void CheckIfRequirementsMet()
    {
      int counter = 0;
      foreach (KeyValuePair<Location, LocInfo> entry in locationDict)
      {
        //Base level (no need to check) or checck if already added
        if (entry.Value.unlocked) { counter++; continue; }

        int unlockedAmount = (int)ServiceLocator.Get<PlayerData>().GetStatValue(PlayerStatisticEnum.MilestonesComplete);

        if (entry.Value.milestoneRequirement <= unlockedAmount)
        {
          locationUI.UnlockLocation(entry.Key);
          ServiceLocator.Get<IndicatorHandler>().SetUIChain(UIPanel.Map);
          entry.Value.unlocked = true;
          counter++;
        }
      }

      if (counter == locationDict.Count)
      {
        ServiceLocator.Get<PlayerData>().SubscribeToStatAltered(PlayerStatisticEnum.MilestonesComplete, CheckIfRequirementsMet, false);
      }
    }

    #endregion

    #region Events and Getters
    public static void SetLocation(Location location)
    {
      if (WaveHandler.WaveState != WaveState.None) return;
      currentLocation = location;
    }
    public bool ReturnIfBestWave(int wave)
    {
      if (locationDict[currentLocation].bestWave < wave)
      {
        locationDict[currentLocation].bestWave = wave;
        return true;
      }
      return false;
    }

    public float ReturnCurrencyModifier()
    {
      return locationDict[currentLocation].silverMod;
    }

    public float ReturnXPModifier()
    {
      return locationDict[currentLocation].xpMod;
    }

    public LocInfo ReturnLocationData(Location location = Location.None)
    {
      var loc = location == Location.None ? currentLocation : location;
      return locationDict[loc];
    }

    public Location ReturnCurrentLocation()
    {
      return currentLocation;
    }

    public EntityIdentification[] ReturnEnemyList(Location loc = Location.None)
    {
      loc = loc != Location.None ? loc : currentLocation;
      return locationDict[loc].enemies;
    }

    public List<EntityIdentification> ReturnEntityList(Location loc = Location.None)
    {
      loc = loc != Location.None ? loc : currentLocation;

      List<EntityIdentification> entities = new();

      entities.AddRange(locationDict[loc].enemies);
      entities.AddRange(locationDict[loc].animals);
      entities.AddRange(locationDict[loc].resources);

      return entities;
    }


    #endregion
  }


  [Serializable]
  public class LocationData
  {
    public LocationDataBlock[] locDataBlocks;

    public LocationData(LocationDataBlock[] blocks)
    {
      locDataBlocks = blocks;
    }
  }

  [Serializable]
  public class LocationDataBlock
  {
    public Location Location;
    public float currencyPercentage;
    public float xpPercentage;
    public int bestWave;


    public bool isUnlocked;
    public LocationDataBlock(Location location, float currencyPercentage, float xpPercentage, int bestWave, bool isUnlocked
    )
    {
      this.Location = location;
      this.currencyPercentage = currencyPercentage;
      this.xpPercentage = xpPercentage;
      this.bestWave = bestWave;
      this.isUnlocked = isUnlocked;
    }

  }




  [Serializable]
  public class LocInfo
  {
    public Location location;

    [Header("Novice, Adept, Elite")]
    public string difficulty;
    public Sprite image;

    [Space(5)]
    [Header("Amount of milestone to complete to unlock location")]
    public int milestoneRequirement;

    [Space(5)]
    [Header("All Enemies, Nodes, and Animals will be")]
    [Header("organized on start of siege")]
    [Header("-----------")]
    public EntityIdentification[] enemies;
    public EntityIdentification[] animals;
    public EntityIdentification[] resources;

    [Space(5)]
    [Header("percentage increase to scaling / DECIMAL FORM")]
    public float scalingMod;


    [Header("data / DECIMAL FORM")]
    public float silverMod;
    public float xpMod;
    [HideInInspector] public int bestWave;
    [Header("Only edit if milestone requirement is 0")] public bool unlocked = false;


    public void LoadData(LocationDataBlock block)
    {
      silverMod = block.currencyPercentage;
      xpMod = block.xpPercentage;
      bestWave = block.bestWave;
      unlocked = block.isUnlocked;
    }

    public LocationDataBlock SaveData() => new LocationDataBlock(location, silverMod, xpMod, bestWave, unlocked);

  }

  public enum Location
  {
    Woods = 0,
    Deadlands = 1,
    Ridge = 2,
    None = 3,
    All = 4,
    Town
  }
}