
using MageAFK.Core;
using MageAFK.Management;
using UnityEngine;

namespace MageAFK.Spells
{

  [CreateAssetMenu(fileName = "Terrorcut", menuName = "Spells/Terrorcut")]
  public class Terrorcut : Spell
  {

    public override void Activate()
    {
      var entityPos = ServiceLocator.Get<EntityTracker>().ReturnBestTarget(Focus);
      var instance = SpellSpawn(iD, entityPos.Body);
      instance.GetComponent<SingleTargetController>().target = entityPos.GetCollider(AI.NPEntityCollider.Body);
      instance.GetComponent<Animator>().Play(Random.Range(0, 2) == 0 ? "Attack1" : "Attack2");
    }


  }
}
