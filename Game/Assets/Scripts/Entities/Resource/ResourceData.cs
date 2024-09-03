using System.Collections.Generic;
using MageAFK.Stats;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MageAFK.AI
{
    [CreateAssetMenu(fileName = "New Resource Data", menuName = "AIData/Resource")]
  public class ResourceData : AIData
  {


    [SerializeField, ShowInInspector, ReadOnly]
    protected override Dictionary<Stat, float> baseStats { get; set; } = new Dictionary<Stat, float>{
      {Stat.Health, 0},
      {Stat.Armour, 0},
      {Stat.EnemySpawnRate, 0}
    };

    [SerializeField, ShowInInspector, ReadOnly] protected override Dictionary<Stat, float> runtimeStats { get; set; }
    [SerializeField, ShowInInspector, ReadOnly] protected override Dictionary<Stat, float> alteredStats { get; set; }

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