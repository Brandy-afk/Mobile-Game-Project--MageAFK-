
using System;
using System.Collections.Generic;
using MageAFK.AI;
using MageAFK.Management;
using MageAFK.Stats;
using UnityEngine;

namespace MageAFK.Spells
{

  [CreateAssetMenu(fileName = "Gloomgazer", menuName = "Spells/Gloomgazer")]
  public class Gloomgazer : Spell
  {

    public override void Activate()
    {
      var target = ReturnHighestHP(ServiceLocator.Get<EntityDataManager>().ReturnActiveNPEntities());

      if (target != null)
      {
        SpellSpawn(iD, target.position);
      }
    }

    private Transform ReturnHighestHP(List<NPEntity> entities)
    {
      var sortedList = entities.ToArray();
      Array.Sort(sortedList, (a, b) => a.data.GetStats(AIDataType.Altered)[Stat.Health].CompareTo(b.data.GetStats(AIDataType.Altered)[Stat.Health]));
      return sortedList[sortedList.Length - 1].transform;

    }
  }
}
