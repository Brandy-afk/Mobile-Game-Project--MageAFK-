
using MageAFK.Core;
using MageAFK.Management;
using MageAFK.Player;
using MageAFK.Stats;
using MageAFK.Tools;
using UnityEngine;

namespace MageAFK.Spells
{

  [CreateAssetMenu(fileName = "Holix", menuName = "Spells/Holix")]
  public class Holix : Spell
  {

    public override void Activate()
    {
      GameObject instance = SpellSpawn(iD, PlayerController.Positions.SpellSpawn); ;

      var targetPos = ServiceLocator.Get<EntityTracker>().ReturnBestTarget(Focus).Pivot;
      var direction = Utility.SetRotation(instance, targetPos, ReturnStatValue(Stat.AimVariance));
      Utility.FlipYSprite(PlayerController.Positions.SpellSpawn, targetPos, instance.transform);
      Utility.SetVelocity(direction, instance, ReturnStatValue(Stat.SpellSpeed));

    }


  }
}
