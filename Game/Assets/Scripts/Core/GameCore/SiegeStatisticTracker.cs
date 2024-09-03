
using System.Collections.Generic;
using MageAFK.Player;
using System.Linq;
using System;
using MageAFK.Management;
using MageAFK.Spells;
using UnityEngine;

namespace MageAFK.Core
{

  public class SiegeStatisticTracker : IData<SiegeStatistic>
  {
    private WaveStatistics waveStatistics;
    private SiegeStatistic siegeStatistics;

    #region Init / Cycle

    //Load
    public void InitializeData(SiegeStatistic data) => siegeStatistics = new SiegeStatistic(data);
    //Save
    public SiegeStatistic SaveData() => siegeStatistics;
    //Constructor
    public SiegeStatisticTracker() => WaveHandler.SubToSiegeEvent(OnSiegeEvent, true, Priority.First);

    private void OnSiegeEvent(Status state)
    {
      if (state == Status.Start)
      {
        if (!GameManager.IsLoad) CreateSiegeStats();
        WaveHandler.SubToWaveState(OnWaveStateChanged, true, Priority.First);
      }
      else
        WaveHandler.SubToWaveState(OnWaveStateChanged, false);
    }

    private void OnWaveStateChanged(WaveState state)
    {
      if (state == WaveState.Wave)
        CreateWaveStats();
    }



    #endregion



    #region StatClassBuilder
    private void CreateSiegeStats() => siegeStatistics = new SiegeStatistic();
    private void CreateWaveStats() => waveStatistics = new WaveStatistics();

    public SiegeStatistic BuildSiegeStats()
    {
      siegeStatistics.Build();
      return siegeStatistics;
    }
    public WaveStatistics BuildWaveStats()
    {
      waveStatistics.Build();
      return waveStatistics;
    }

    #endregion

    public void ModifiyMetric(PlayerStatisticEnum metric, int intAmount = 0)
    {
      if (siegeStatistics == null || waveStatistics == null) { Debug.Log("Objects are null... "); return; }

      siegeStatistics.intMetrics.TryAdd(metric, 0);
      siegeStatistics.intMetrics[metric] += intAmount;

      waveStatistics.intMetrics.TryAdd(metric, 0);
      waveStatistics.intMetrics[metric] += intAmount;
    }


    public void ModifySpellMetric(SpellIdentification spellKey, float damage)
    {
      if (siegeStatistics == null || waveStatistics == null) { Debug.Log("Objects are null... "); return; }
      UpdateDamageMetrics(waveStatistics.spellDamage, spellKey, damage);
      UpdateDamageMetrics(siegeStatistics.spellDamage, spellKey, damage);
    }

    public void ModifyEnemyMetric(EntityIdentification enemyID, float damage)
    {
      if (siegeStatistics == null || waveStatistics == null) { Debug.Log("Objects are null... "); return; }
      UpdateDamageMetrics(waveStatistics.enemyDamage, enemyID, damage);
      UpdateDamageMetrics(siegeStatistics.enemyDamage, enemyID, damage);
    }

    private void UpdateDamageMetrics<T>(Dictionary<T, float> metrics, T id, float damage)
    {
      if (metrics == null) metrics = new();
      if (!metrics.ContainsKey(id))
        metrics[id] = damage;
      else
        metrics[id] += damage;

    }

    public void ModifyDamage(float damage)
    {
      if (siegeStatistics == null || waveStatistics == null) { Debug.Log("Objects are null... "); return; }

      siegeStatistics.totalDamageDone += damage;
    }


  }

  #region Classes
  [Serializable]
  public class WaveStatistics
  {
    public Dictionary<SpellIdentification, float> spellDamage;
    //Damage to player.
    public Dictionary<EntityIdentification, float> enemyDamage;

    public Dictionary<PlayerStatisticEnum, int> intMetrics = new Dictionary<PlayerStatisticEnum, int>{
      {PlayerStatisticEnum.Kills, 0},
      {PlayerStatisticEnum.Experience, 0},
      {PlayerStatisticEnum.Silver, 0},
      {PlayerStatisticEnum.Gems, 0},
      {PlayerStatisticEnum.ItemsGained, 0}
    };

    public string wave;
    public (SpellIdentification, float)[] topSpells;
    public KeyValuePair<EntityIdentification, float> nemesisPair = new(EntityIdentification.None, 0);

    public void Build()
    {
      wave = $"Wave {WaveHandler.Wave}";

      if (enemyDamage.Count > 0)
      {
        nemesisPair = enemyDamage.Aggregate((x, y) => x.Value > y.Value ? x : y);
      }

      topSpells = GetTopThreeSpellDamages();

      spellDamage = null;
      enemyDamage = null;
    }

    private (SpellIdentification, float)[] GetTopThreeSpellDamages()
    {
      // Ensure the dictionary is not null
      if (spellDamage == null)
      {
        return new (SpellIdentification, float)[0];  // or throw an exception, depending on your requirement.
      }

      // Sort and take top 3
      var topThree = spellDamage.OrderByDescending(entry => entry.Value)
                                .Take(3)
                                .Select(entry => (entry.Key, entry.Value))
                                .ToArray();

      return topThree;
    }
    public WaveStatistics()
    {
      spellDamage = new Dictionary<SpellIdentification, float>();
      enemyDamage = new Dictionary<EntityIdentification, float>();
    }

  }

  [Serializable]
  public class SiegeStatistic
  {

    public Dictionary<SpellIdentification, float> spellDamage;
    //Damage to player.
    public Dictionary<EntityIdentification, float> enemyDamage;
    public Dictionary<PlayerStatisticEnum, int> intMetrics = new Dictionary<PlayerStatisticEnum, int>{
      {PlayerStatisticEnum.Kills, 0},
      {PlayerStatisticEnum.Silver, 0},
      {PlayerStatisticEnum.Gems, 0},
      {PlayerStatisticEnum.Gold, 0},
      {PlayerStatisticEnum.ItemsGained, 0},
      {PlayerStatisticEnum.BossesKilled, 0}
    };

    public string wave;
    public bool isBestWave;
    public Location location;
    public string dateFinished;
    public float totalDamageDone = 0;
    public string level;
    public (EntityIdentification, float) mobPair;
    public (SpellIdentification, float) spellPair;

    public void Build()
    {
      var waveHandler = ServiceLocator.Get<WaveHandler>();
      var locationHandler = ServiceLocator.Get<LocationHandler>();

      //Get wave died on.
      wave = $"Wave {WaveHandler.Wave}";
      level = ServiceLocator.Get<LevelHandler>().ReturnCurrentLevel().ToString();

      //Get top damage spell and arch nemesis
      KeyValuePair<EntityIdentification, float> archPair = default;
      if (enemyDamage.Any())
      {
        archPair = enemyDamage.Count == 1 ? enemyDamage.First() :
                   enemyDamage.Aggregate((x, y) => x.Value > y.Value ? x : y);
      }

      KeyValuePair<SpellIdentification, float> topSpell = default;
      if (spellDamage.Any())
      {
        topSpell = spellDamage.Count == 1 ? spellDamage.First() :
                   spellDamage.Aggregate((x, y) => x.Value > y.Value ? x : y);
      }

      //Package details
      mobPair.Item2 = archPair.Value;
      mobPair.Item1 = archPair.Key;
      //Mob that killed the player


      spellPair.Item2 = topSpell.Value;
      spellPair.Item1 = topSpell.Key;

      //Time of death. 
      DateTime now = DateTime.Now;
      dateFinished = now.ToString("MM/dd/yyyy HH:mm:ss");

      //Deteremine if best wave
      isBestWave = locationHandler.ReturnIfBestWave(WaveHandler.Wave);

      //Get current Location
      location = locationHandler.ReturnCurrentLocation();

      spellDamage = null;
      enemyDamage = null;
    }

    public SiegeStatistic()
    {
      spellDamage = new Dictionary<SpellIdentification, float>();
      enemyDamage = new Dictionary<EntityIdentification, float>();
    }

    public SiegeStatistic(SiegeStatistic source)
    {
      // Copying value types and immutable reference types (strings, DateTime)
      wave = source.wave;
      isBestWave = source.isBestWave;
      location = source.location; // Assuming Location is an enum or immutable
      dateFinished = source.dateFinished;
      totalDamageDone = source.totalDamageDone;
      level = source.level;
      mobPair = source.mobPair; // Tuples are value types, but check if EntityIdentification is a value type
      spellPair = source.spellPair; // Similarly, check SpellIdentification


      // For dictionaries, create new instances and copy each key-value pair
      intMetrics = new Dictionary<PlayerStatisticEnum, int>(source.intMetrics);
      // Assuming enemyDamage and spellDamage are declared somewhere within SiegeStatistics
      spellDamage = source.spellDamage != null ?
          new Dictionary<SpellIdentification, float>(source.spellDamage) : null;
      enemyDamage = source.enemyDamage != null ?
          new Dictionary<EntityIdentification, float>(source.enemyDamage) : null;
    }

  }


  #endregion
}