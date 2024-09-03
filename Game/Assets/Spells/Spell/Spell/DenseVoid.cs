
using MageAFK.Core;
using MageAFK.Management;
using MageAFK.Player;
using MageAFK.Stats;
using MageAFK.Tools;
using UnityEngine;

namespace MageAFK.Spells
{

  [CreateAssetMenu(fileName = "DenseVoid", menuName = "Spells/DenseVoid")]
  public class DenseVoid : Spell
  {

    public override void Activate()
    {
      GameObject instance = SpellSpawn(iD, PlayerController.Positions.SpellSpawn);

      var ePos = ServiceLocator.Get<EntityTracker>().ReturnBestTarget(Focus);
      Utility.FlipXSprite(PlayerController.Positions.SpellSpawn, ePos.Pivot, instance.transform);
      Utility.SetVelocity(instance, ePos.Pivot, ReturnStatValue(Stat.SpellSpeed), ReturnStatValue(Stat.AimVariance));
    }


  }

}
