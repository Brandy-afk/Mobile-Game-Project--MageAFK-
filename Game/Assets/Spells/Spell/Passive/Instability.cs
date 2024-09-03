
using System.Collections.Generic;
using MageAFK.Stats;
using MageAFK.Tools;
using UnityEngine;

namespace MageAFK.Spells
{

    [CreateAssetMenu(fileName = "Instability", menuName = "Spells/Instability")]
  public class Instability : Spell
  {

    private LinkedList<InstabilityProjectile> que = new();
    public override void Activate()
    {
      if (que.Count >= ReturnStatValue(Stat.SpawnCap))
      {
        var removedSpawn = que.First.Value;
        removedSpawn.InitialDisable();
      }

      que.AddLast(SpellSpawn(iD, Utility.GetRandomMapPosition()).GetComponent<InstabilityProjectile>());
    }

    public void Dequeue(InstabilityProjectile spawn) => que.Remove(spawn);

    public override void OnWaveOver()
    {
      if (que.Count > 0)
        que.Clear();
    }

  }
}
