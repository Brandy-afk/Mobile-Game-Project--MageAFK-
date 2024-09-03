using MageAFK.Core;
using MageAFK.Management;
using MageAFK.Player;
using MageAFK.Stats;
using MageAFK.Tools;
using UnityEngine;

namespace MageAFK.Spells
{

  [CreateAssetMenu(fileName = "Acidlash", menuName = "Spells/Acidlash")]
  public class Acidlash : Spell
  {

    public override void Activate()
    {
      var ePos = ServiceLocator.Get<EntityTracker>().ReturnBestTarget(Focus);
      var spawn = ePos.Pivot;
      spawn.y -= .02f;
      GameObject instance = SpellSpawn(iD, spawn);
      AcidlashProjectile obj = instance.GetComponent<AcidlashProjectile>();
      obj.target = ePos.GetCollider(AI.NPEntityCollider.Body);

      Utility.FlipXSprite(PlayerController.Positions.Pivot, ePos.Pivot, instance.transform);
      var animation = Utility.RollChance(ReturnStatValue(Stat.SpecialChance)) ? "Burst" : "Smack";
      obj.animator.Play(animation);
    }

  }
}
