
using MageAFK.Core;
using MageAFK.Management;
using MageAFK.Player;
using MageAFK.Tools;
using UnityEngine;

namespace MageAFK.Spells
{

  [CreateAssetMenu(fileName = "Boulder", menuName = "Spells/Boulder")]
  public class Boulder : Spell
  {

    public override void Activate()
    {
      GameObject instance = SpellSpawn(iD, PlayerController.Positions.SpellSpawn);
      var ePos = ServiceLocator.Get<EntityTracker>().ReturnBestTarget(Focus);
      Utility.SetVelocity(instance, ePos.Body, ReturnStatValue(Stats.Stat.SpellSpeed), ReturnStatValue(Stats.Stat.AimVariance));
      Utility.FlipXSprite(PlayerController.Positions.SpellSpawn, ePos.Pivot, instance.transform);
      instance.GetComponent<BoulderProjectile>().SetUp();
    }


  }
}
