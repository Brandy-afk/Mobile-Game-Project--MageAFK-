using MageAFK.Core;
using MageAFK.Management;
using MageAFK.Player;
using MageAFK.Tools;
using UnityEngine;

namespace MageAFK.Spells
{

  [CreateAssetMenu(fileName = "Pyrocuts", menuName = "Spells/Pyrocuts")]
  public class Pyrocuts : Spell
  {
    private readonly string[] attacks = new string[] {
    "Cleave",
    "Uppercut",
    "Claw"
    };

    public override void Activate()
    {
      var attack = attacks[Random.Range(0, attacks.Length)];

      var npPos = ServiceLocator.Get<EntityTracker>().ReturnBestTarget(Focus);
      var instance = SpellSpawn(iD, npPos.Body);
      Utility.FlipXSprite(PlayerController.Positions.Pivot, npPos.Pivot, instance.transform.GetChild(0));
      instance.GetComponentInChildren<SingleTargetController>().target = npPos.GetCollider(AI.NPEntityCollider.Body);
      instance.GetComponentInChildren<Animator>().Play(attack.ToString());
    }


  }
}
