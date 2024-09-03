
using MageAFK.Core;
using MageAFK.Management;
using MageAFK.Player;
using MageAFK.Tools;
using UnityEngine;

namespace MageAFK.Spells
{

  [CreateAssetMenu(fileName = "SeaStrikes", menuName = "Spells/SeaStrikes")]
  public class SeaStrikes : Spell
  {
    public override void Activate()
    {
      var nonPlayerPos = ServiceLocator.Get<EntityTracker>().ReturnBestTarget(Focus);
      var instance = SpellSpawn(iD, nonPlayerPos.Body);
      Utility.FlipXSprite(PlayerController.Positions.Pivot, nonPlayerPos.Pivot, instance.transform.GetChild(0));
      var projectile = instance.GetComponentInChildren<SeaStrikesProjectile>();
      projectile.target = nonPlayerPos.GetCollider(AI.NPEntityCollider.Body);
      projectile.entityPosition = nonPlayerPos;
    }
  }
}
