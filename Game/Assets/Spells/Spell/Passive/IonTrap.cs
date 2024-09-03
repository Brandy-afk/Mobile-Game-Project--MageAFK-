using System.Collections.Generic;
using MageAFK.Stats;
using MageAFK.Tools;
using UnityEngine;

namespace MageAFK.Spells
{

  [CreateAssetMenu(fileName = "IonTrap", menuName = "Spells/IonTrap")]
  public class IonTrap : Spell
  {
    private LinkedList<IonTrapProjectile> que = new();
    public override void Activate()
    {
      if (que.Count >= ReturnStatValue(Stat.SpawnCap))
      {
        var removedSpawn = que.First.Value;
        removedSpawn.Disable();
      }

      que.AddLast(SpellSpawn(iD, Utility.GetRandomMapPosition()).GetComponent<IonTrapProjectile>());
    }

    public void Dequeue(IonTrapProjectile spawn) => que.Remove(spawn);

    public override void OnWaveOver()
    {
      if (que.Count > 0)
        que.Clear();
    }
  }
}
