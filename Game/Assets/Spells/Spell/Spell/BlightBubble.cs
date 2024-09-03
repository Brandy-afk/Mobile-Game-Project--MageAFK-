using MageAFK.Core;
using MageAFK.Management;
using MageAFK.Player;
using MageAFK.Tools;
using UnityEngine;

namespace MageAFK.Spells
{

  [CreateAssetMenu(fileName = "BlightBubble", menuName = "Spells/BlightBubble")]
  public class BlightBubble : Spell
  {

    public override void Activate()
    {
      GameObject instance = SpellSpawn(iD, PlayerController.Positions.SpellSpawn);
      var ePos = ServiceLocator.Get<EntityTracker>().ReturnBestTarget(Focus);
      Utility.FlipXSprite(PlayerController.Positions.SpellSpawn, ePos.Pivot, instance.transform);
      Utility.SetVelocity(instance, ePos.Body, ReturnStatValue(Stats.Stat.SpellSpeed), ReturnStatValue(Stats.Stat.AimVariance));
    }


  }
}
