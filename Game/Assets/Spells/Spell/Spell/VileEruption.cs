
using MageAFK.Core;
using MageAFK.Management;
using UnityEngine;

namespace MageAFK.Spells
{

  [CreateAssetMenu(fileName = "VileEruption", menuName = "Spells/VileEruption")]
  public class VileEruption : Spell
  {

    public override void Activate()
    {
      var nonPlayerPos = ServiceLocator.Get<EntityTracker>().ReturnBestTarget(Focus);
      var spawn = nonPlayerPos.Pivot;
      spawn.y -= .02f;
      SpellSpawn(iD, spawn).GetComponent<VileEruptionProjectile>().target = nonPlayerPos.GetCollider(AI.NPEntityCollider.Body);
    }

  }
}
