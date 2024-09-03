
using MageAFK.AI;
using MageAFK.Core;
using MageAFK.Management;
using UnityEngine;

namespace MageAFK.Spells
{

  [CreateAssetMenu(fileName = "TimberStrike", menuName = "Spells/TimberStrike")]
  public class TimberStrike : Spell
  {

    public override void Activate()
    {
      var target = ServiceLocator.Get<EntityTracker>().ReturnBestTarget(Focus);
      SpellSpawn(iD, target.Feet).GetComponent<TimberStrikeProjectile>().entity = target.Transform.GetComponent<NPEntity>();
    }

  }
}
