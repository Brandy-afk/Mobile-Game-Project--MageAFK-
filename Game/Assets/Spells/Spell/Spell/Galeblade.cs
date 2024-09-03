
using MageAFK.Core;
using MageAFK.Management;
using MageAFK.Player;
using MageAFK.Stats;
using MageAFK.Tools;
using UnityEngine;

namespace MageAFK.Spells
{

  [CreateAssetMenu(fileName = "Galeblade", menuName = "Spells/Galeblade")]
  public class Galeblade : Spell
  {

    public override void Activate()
    {
      GameObject instance = SpellSpawn(iD, PlayerController.Positions.SpellSpawn);
      var ePos = ServiceLocator.Get<EntityTracker>().ReturnBestTarget(Focus);
      Utility.FlipXSprite(instance.transform.position, ePos.Pivot, instance.transform);
      Utility.SetVelocity(instance, ePos.Feet, ReturnStatValue(Stat.SpellSpeed), ReturnStatValue(Stat.AimVariance));
    }


  }
}
