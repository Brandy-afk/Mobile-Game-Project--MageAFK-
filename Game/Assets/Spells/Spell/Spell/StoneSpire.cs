
using MageAFK.AI;
using MageAFK.Core;
using MageAFK.Management;
using MageAFK.Player;
using MageAFK.Tools;
using UnityEngine;

namespace MageAFK.Spells
{

  [CreateAssetMenu(fileName = "StoneSpire", menuName = "Spells/StoneSpire")]
  public class StoneSpire : Spell
  {

    public override void Activate()
    {
      SpawnSpire(ServiceLocator.Get<EntityTracker>().ReturnBestTarget(Focus), procCount: (int)ReturnStatValue(Stats.Stat.MaxTriggers) + 1);
    }

    public void SpawnSpire(INonPlayerPosition target, int procCount, Transform orgin = null)
    {
      procCount--;
      var instance = SpellSpawn(iD, target.Feet);
      var main = orgin ? (Vector2)orgin.position : PlayerController.Positions.Pivot;
      Utility.FlipXSprite(main, target.Feet, instance.transform);
      StoneSpireProjectile projectile = instance.GetComponent<StoneSpireProjectile>();
      projectile.target = target.GetCollider(NPEntityCollider.Body);
      projectile.procs = procCount;
    }

  }
}
