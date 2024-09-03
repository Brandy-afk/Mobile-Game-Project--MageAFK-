using MageAFK.Core;
using MageAFK.Management;
using UnityEngine;

namespace MageAFK.Spells
{

  [CreateAssetMenu(fileName = "IceCrash", menuName = "Spells/IceCrash")]
  public class IceCrash : Spell
  {

    public override void Activate()
    {
      var targets = ServiceLocator.Get<EntityTracker>().GetMultipleRandomTargets(2);
      if (targets == null || targets.Length == 0)
        return;


      var spell1 = SpellSpawn(iD, targets[0].Feet);
      spell1.GetComponent<SingleTargetController>().target = targets[0].GetCollider(AI.NPEntityCollider.Body);

      if (targets.Length > 1)
      {
        var spell2 = SpellSpawn(iD, targets[1].Feet);
        spell2.GetComponent<SingleTargetController>().target = targets[1].GetCollider(AI.NPEntityCollider.Body);
      }
    }


  }
}
