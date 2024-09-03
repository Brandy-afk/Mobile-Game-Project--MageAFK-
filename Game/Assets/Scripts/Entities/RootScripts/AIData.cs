
using System.Collections.Generic;
using MageAFK.Core;
using MageAFK.Spells;
using MageAFK.Stats;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MageAFK.AI
{
  public abstract class AIData : SerializedScriptableObject
  {
    public EntityIdentification iD;
    [SerializeField] protected StatusType[] immunities;
    protected abstract Dictionary<Stat, float> baseStats { get; set; }
    protected abstract Dictionary<Stat, float> runtimeStats { get; set; }
    protected abstract Dictionary<Stat, float> alteredStats { get; set; }

    public virtual void ResetStats()
    {
      if (runtimeStats == null) { runtimeStats = new Dictionary<Stat, float>(); }
      if (alteredStats == null) { alteredStats = new Dictionary<Stat, float>(); }
      foreach (var key in baseStats.Keys)
      {
        runtimeStats[key] = baseStats[key];
        alteredStats[key] = baseStats[key];
      }
    }

    public virtual void ScaleStats(float mod, float minSpawnCap)
    {
      runtimeStats[Stat.Health] = baseStats[Stat.Health] + (baseStats[Stat.Health] * mod);
    }

    public abstract void RecalculateStats(float minValueCap);

    public abstract void RecalculateStat(Stat stat, float minValueCap);

    public Dictionary<Stat, float> GetStats(AIDataType type)
    {
      switch (type)
      {
        case AIDataType.Base:
          return baseStats;
        case AIDataType.Runtime:
          return runtimeStats;
        case AIDataType.Altered:
          return alteredStats;
      }
      Debug.Log($"Returning null - Error");
      return null;
    }

    public StatusType[] GetImmunities() => immunities;
  }



  public enum AIDataType
  {
    Base,
    Runtime,
    Altered
  }


}