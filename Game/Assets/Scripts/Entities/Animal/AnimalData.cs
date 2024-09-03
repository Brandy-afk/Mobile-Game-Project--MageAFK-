using System.Collections.Generic;
using MageAFK.Stats;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MageAFK.AI
{
    [CreateAssetMenu(fileName = "New Animal Data", menuName = "AIData/Animal")]
  public class AnimalData : AIData
  {

    [SerializeField, ShowInInspector, ReadOnly]
    protected override Dictionary<Stat, float> baseStats { get; set; } = new Dictionary<Stat, float>{
       {Stat.Health, 0},
      {Stat.Armour, 0},
      {Stat.MovementSpeed, 0},
      {Stat.EnemySpawnRate, 0},
    };


    [SerializeField, ShowInInspector, ReadOnly] protected override Dictionary<Stat, float> runtimeStats { get; set; }
    [SerializeField, ShowInInspector, ReadOnly] protected override Dictionary<Stat, float> alteredStats { get; set; }

    public override void ScaleStats(float mod, float minSpawnCap)
    {
      base.ScaleStats(mod, minSpawnCap);
      runtimeStats[Stat.EnemySpawnRate] =
        Mathf.Max((baseStats[Stat.EnemySpawnRate] * minSpawnCap),
        baseStats[Stat.EnemySpawnRate] - (baseStats[Stat.EnemySpawnRate] * mod));
    }

    public override void RecalculateStat(Stat stat, float minValueCap)
    {
      throw new System.NotImplementedException();
    }

    public override void RecalculateStats(float minValueCap)
    {
      throw new System.NotImplementedException();
    }
  }
}
