
using MageAFK.Core;
using MageAFK.Management;
using MageAFK.Player;
using MageAFK.Stats;
using MageAFK.Tools;
using UnityEngine;

namespace MageAFK.Spells
{

  [CreateAssetMenu(fileName = "Voidshade", menuName = "Spells/Voidshade")]
  public class Voidshade : Spell
  {
    public override void Activate()
    {
      GameObject instance = SpellSpawn(iD, PlayerController.Positions.SpellSpawn, false);
      var ePos = ServiceLocator.Get<EntityTracker>().ReturnBestTarget(Focus);
      var direction = Utility.SetRotation(instance, ePos.Body);
      Utility.SetVelocity(direction, instance, ReturnStatValue(Stat.SpellSpeed));
      Utility.FlipYSprite(Vector3.zero, Vector3.zero, instance.transform, direction.x < 0);
      instance.SetActive(true);
    }


  }
}
