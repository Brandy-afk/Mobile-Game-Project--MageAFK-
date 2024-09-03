
using MageAFK.Core;
using MageAFK.Management;
using MageAFK.Player;
using MageAFK.Tools;
using UnityEngine;

namespace MageAFK.Spells
{

  [CreateAssetMenu(fileName = "Shadowhand", menuName = "Spells/Shadowhand")]
  public class Shadowhand : Spell
  {

    public override void Activate()
    {
      var npPos = ServiceLocator.Get<EntityTracker>().ReturnBestTarget(Focus);
      var instance = SpellSpawn(iD, npPos.Body);
      Utility.FlipXSprite(PlayerController.Positions.Pivot, npPos.Pivot, instance.transform);
      instance.GetComponent<SingleTargetController>().target = npPos.GetCollider(AI.NPEntityCollider.Body);
    }

  }
}
