using UnityEngine;
using MageAFK.Stats;
using MageAFK.Core;
using MageAFK.Tools;
using MageAFK.Management;
using MageAFK.Player;

namespace MageAFK.Spells
{
  [CreateAssetMenu(fileName = "New Skill", menuName = "Spells/WindBlades")]
  public class WindBlades : Spell
  {
    public override void Activate()
    {
      GameObject instance = SpellSpawn(iD, PlayerController.Positions.SpellSpawn);
      Utility.SetVelocity(
        Utility.SetRotation(
          instance, ServiceLocator.Get<EntityTracker>().ReturnBestTarget(Focus).Pivot, ReturnStatValue(Stat.AimVariance)),
          instance,
          ReturnStatValue(Stat.SpellSpeed));
    }



  }
}
