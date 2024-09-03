using MageAFK.AI;
using MageAFK.Core;
using MageAFK.Management;
using MageAFK.Player;
using MageAFK.Stats;
using MageAFK.Tools;
using UnityEngine;

namespace MageAFK.Spells
{

  [CreateAssetMenu(fileName = "Soulken", menuName = "Spells/Soulken")]
  public class Soulken : Spell
  {

    public override void Activate()
    {
      GameObject instance = SpellSpawn(iD, PlayerController.Positions.SpellSpawn);
      var ePos = ServiceLocator.Get<EntityTracker>().ReturnBestTarget(Focus);
      Utility.FlipXSprite(instance.transform.position, ePos.Pivot, instance.transform);
      Utility.SetVelocity(instance, ePos.Body, ReturnStatValue(Stat.SpellSpeed), ReturnStatValue(Stat.AimVariance));
    }

    public void SpawnExplosion(IEntityPosition ePos)
    {
      SpellSpawn(SpellIdentification.Soulken_Explosion, ePos.Feet, true, SpellIdentification.Soulken);
    }


  }
}
