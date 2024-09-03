
using System.Collections.Generic;
using MageAFK.Management;
using MageAFK.Stats;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MageAFK.AI
{

  [CreateAssetMenu(fileName = "New Enemy Data", menuName = "AIData/Enemy Data")]
  public class EnemyData : AIData
  {


    [SerializeField, ShowInInspector]
    protected override Dictionary<Stat, float> baseStats { get; set; } = new Dictionary<Stat, float>{
    {Stat.Health, 0},
      {Stat.Armour, 0},
      {Stat.MovementSpeed, 0},
      {Stat.AttackRange, 0},
      {Stat.Cooldown, 0},
      {Stat.EnemySpawnRate, 0},
      {Stat.Damage, 0}
    };

    [SerializeField, ShowInInspector, ReadOnly] protected override Dictionary<Stat, float> runtimeStats { get; set; }
    [SerializeField, ShowInInspector] protected override Dictionary<Stat, float> alteredStats { get; set; }


    public override void ScaleStats(float mod, float minSpawnCap)
    {
      base.ScaleStats(mod, minSpawnCap);

      runtimeStats[Stat.Damage] = baseStats[Stat.Damage] + (baseStats[Stat.Damage] * mod);
      runtimeStats[Stat.EnemySpawnRate] =
        Mathf.Max(baseStats[Stat.EnemySpawnRate] * minSpawnCap,
        baseStats[Stat.EnemySpawnRate] - (baseStats[Stat.EnemySpawnRate] * mod));
    }

    public override void RecalculateStats(float minValueCap)
    {
      foreach (var key in baseStats.Keys)
      {
        RecalculateStat(key, minValueCap);
      }
    }

    public override void RecalculateStat(Stat stat, float minValueCap)
    {
      if (stat == Stat.Armour)
      {
        alteredStats[stat] = Mathf.Max(ServiceLocator.Get<EnemyStatHandler>().ReturnModifiedValue(stat, runtimeStats[stat]), 0);
        return;
      }

      alteredStats[stat] = Mathf.Max
        (ServiceLocator.Get<EnemyStatHandler>().ReturnModifiedValue(stat, runtimeStats[stat])
        , baseStats[stat] * minValueCap);

    }



  }


}