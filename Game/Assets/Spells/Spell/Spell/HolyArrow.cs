using MageAFK.Core;
using MageAFK.Management;
using MageAFK.Player;
using MageAFK.Stats;
using MageAFK.Tools;
using UnityEngine;

namespace MageAFK.Spells
{

  [CreateAssetMenu(fileName = "HolyArrow", menuName = "Spells/HolyArrow")]
  public class HolyArrow : Spell
  {

    public override void Activate()
    {
      GameObject instance = SpellSpawn(iD, PlayerController.Positions.SpellSpawn); ;

      var ePos = ServiceLocator.Get<EntityTracker>().GetFurthestTarget(ServiceLocator.Get<EntityTracker>().entities);
      instance.GetComponent<HolyArrowProjectile>().target = ePos.Transform;

      Utility.SetVelocity(
      Utility.SetRotation(
        instance, ePos.Body, ReturnStatValue(Stat.AimVariance)),
        instance,
        ReturnStatValue(Stat.SpellSpeed));
    }



  }

}
