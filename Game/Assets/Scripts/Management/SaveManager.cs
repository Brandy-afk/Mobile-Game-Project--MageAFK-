using MageAFK.AI;
using MageAFK.Core;
using MageAFK.Items;
using MageAFK.Player;
using MageAFK.Skills;
using MageAFK.Spells;
using MageAFK.Stats;
using MageAFK.UI;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace MageAFK.Management
{
  public static class SaveManager
  {
    public delegate void SaveDataLoadedHandler();
    public static event SaveDataLoadedHandler OnSaveDataLoaded;
    private static ConcurrentDictionary<DataType, Task> activeTasks = new();


    static SaveManager()
    {
      WaveHandler.SubToWaveState(SaveOnWaveStart, true, Priority.Last);
      WaveHandler.SubToSiegeEvent(SaveOnSiegeEnd, true, Priority.Last);
    }

    #region Project Related Functions

    /// <summary>
    /// Called on game launch. [ONLY]
    /// </summary>
    public static void LoadAllData()
    {
      Debug.Log(Application.persistentDataPath);

      Load<int, CraftingHandler>(DataType.CraftingData);
      Load<CurrencyData, CurrencyHandler>(DataType.CurrencyData);
      Load<(float, int), LevelProgressHandler>(DataType.LevelUpgradeData);
      Load(UIController.Instance.ReturnDataObject<SiegeHistoryData>(DataType.SiegeHistoryData), DataType.SiegeHistoryData);
      Load<PlayerDataCollection, PlayerData>(DataType.PlayerData); // Assuming PlayerDataHandler exists
      Load<StatData, PlayerStatHandler>(DataType.PermPlayerStat);
      Load<StatData, EnemyStatHandler>(DataType.PermEnemyStat);
      Load<PermInventoryData, InventoryHandler>(DataType.PermInventoryData);


      //<---------------Monos---------->
      Load<PowerData>(DataType.ElixirData);
      Load<RecipeShopData>(DataType.RecipeShopData);

      Load<RecipeData>(DataType.RecipeData);
      Load<PermSpellDataCollection>(DataType.SpellData);

      Load<SkillData>(DataType.SkillData);
      Load<MilestoneData>(DataType.MilestoneData);
      Load<GradeStatData>(DataType.GradeData);
      Load<LocationData>(DataType.LocationData);
      Load(new WaveSaveHandler(), DataType.WaveSaveData, false);

      // Notify listeners that the data has been loaded
      if (OnSaveDataLoaded != null)
      {
        OnSaveDataLoaded.Invoke();
      }
      OnSaveDataLoaded = null;
    }

    /// <summary>
    /// Saves all data related to wave behaviour. Called on new wave start.
    /// </summary>
    private static void SaveOnWaveStart(WaveState state)
    {
      if (!GameManager.IsLoad && state == WaveState.Wave)
      {
        if (WaveHandler.Wave == 1) Save(ServiceLocator.Get<PowerHandler>().SaveData(), DataType.ElixirData);
        SaveDependentPermanents();
        Save(new WaveSaveData(true), DataType.WaveSaveData);
      }
    }

    /// <summary>
    /// Saves all data related to end of siege. Called on siege end.
    /// </summary>
    private static void SaveOnSiegeEnd(Status state)
    {
      SaveDependentPermanents();
      Delete(DataType.WaveSaveData);
    }

    private static void SaveDependentPermanents()
    {
      Save(ServiceLocator.Get<PlayerData>().SaveData(), DataType.PlayerData);
      Save(ServiceLocator.Get<PlayerStatHandler>().SaveData(), DataType.PermPlayerStat);
      Save(ServiceLocator.Get<EnemyStatHandler>().SaveData(), DataType.PermEnemyStat);
      Save(ServiceLocator.Get<CurrencyHandler>().SaveData(), DataType.CurrencyData);
      Save(ServiceLocator.Get<SkillTreeHandler>().SaveData(), DataType.SkillData);
      Save(ServiceLocator.Get<MilestoneHandler>().SaveData(), DataType.MilestoneData);
      Save(ServiceLocator.Get<GradeStatHandler>().SaveData(), DataType.GradeData);
      Save(ServiceLocator.Get<LocationHandler>().SaveData(), DataType.LocationData);
      Save(ServiceLocator.Get<LevelProgressHandler>().SaveData(), DataType.LevelUpgradeData);
      Save(ServiceLocator.Get<CraftingHandler>().SaveData(), DataType.CraftingData);
      Save(ServiceLocator.Get<RecipeHandler>().SaveData(), DataType.RecipeData);
      Save(ServiceLocator.Get<SpellHandler>().SaveData(), DataType.SpellData);
      Save(ServiceLocator.Get<InventoryHandler>().SaveData(), DataType.PermInventoryData);
    }

    #endregion

    #region Class Functions

    #region Loading


    /// <summary>
    /// Loades data based on script. Will use service locator to get Script. (Monos should not call this)
    /// </summary>
    /// <typeparam name="Data"></typeparam>
    /// <typeparam name="Script"></typeparam>
    /// <param name="type"></param>
    /// <param name="saveIfNotFound"></param>
    public static void Load<Data, Script>(DataType type, bool saveIfNotFound = true)
  where Script : class, IData<Data>, new() =>
  Load(ServiceLocator.Get<Script>(), type, saveIfNotFound);

    /// <summary>
    /// Loads data based on data type. Will use service locator to get IData interface. 
    /// </summary>
    /// <typeparam name="Data"></typeparam>
    /// <param name="type"></param>
    /// <param name="saveIfNotFound"></param>
    public static void Load<Data>(DataType type, bool saveIfNotFound = true) =>
     Load(ServiceLocator.Get<IData<Data>>(), type, saveIfNotFound);
    /// <summary>
    /// Loads data based on interface. Initializes data if found otherwise it will save current data.
    /// </summary>
    /// <typeparam name="Data"></typeparam>
    /// <param name="iData"></param>
    /// <param name="type"></param>
    /// <param name="saveIfNotFound"></param>
    public static void Load<Data>(IData<Data> iData, DataType type, bool saveIfNotFound = true)
    {
      string saveFilePath = ReturnFilePath(type);

      if (File.Exists(saveFilePath))
      {
        string jsonString = File.ReadAllText(saveFilePath);
        var loadedData = JsonConvert.DeserializeObject<Data>(jsonString);
        iData.InitializeData(loadedData);
      }
      else if (saveIfNotFound)
      {
        Save(iData.SaveData(), type);
      }
    }

    #endregion

    #region Saving

    /// <summary>
    /// Saves data to file based on type. [ASYNC]
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <param name="type"></param>
    public static async void Save<T>(T data, DataType type)
    {
      string filePath = ReturnFilePath(type);

      if (activeTasks.TryAdd(type, Task.CompletedTask)) // Placeholder Task to indicate work in progress
      {
        try
        {
          string jsonString = JsonConvert.SerializeObject(data, Formatting.Indented);
          await File.WriteAllTextAsync(filePath, jsonString);
        }
        finally
        {
          activeTasks.TryRemove(type, out var _);
        }
      }
    }

    #endregion

    #region Deleting

    /// <summary>
    /// Deletes data based on type.
    /// </summary>
    /// <param name="type"></param>
    public static async void Delete(DataType type)
    {
      if (activeTasks.TryGetValue(type, out var task))
        await task;

      if (activeTasks.TryAdd(type, Task.CompletedTask))
      {
        try
        {
          string filePath = ReturnFilePath(type);
          if (File.Exists(filePath))
          {
            File.Delete(filePath);
          }
        }
        finally
        {
          activeTasks.TryRemove(type, out var _);
        }
      }
    }
    #endregion

    #region Helpers
    private static string ReturnFilePath(DataType type) => Path.Combine(Application.persistentDataPath, $"{type}.txt");

    #endregion

    #endregion

  }

  public enum DataType
  {
    WaveSaveData,
    ElixirData,
    RecipeData,
    SpellData,
    CraftingData,
    CurrencyData,
    SkillData,
    MilestoneData,
    GradeData,
    LocationData,
    LevelUpgradeData,
    PlayerData,
    PermPlayerStat,
    PermEnemyStat,
    SiegeHistoryData,
    PermInventoryData,
    RecipeShopData
    //etc
  }

  public interface IData<TData>
  {
    void InitializeData(TData data);
    TData SaveData();
  }

  public interface ITempData<TData>
  {
    void InitializeTempData(TData data);
    TData SaveTempData();
  }
}