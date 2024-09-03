
using MageAFK.Core;
using MageAFK.Management;
using UnityEngine;

namespace MageAFK.Spells
{

  [CreateAssetMenu(fileName = "HellishAura", menuName = "Spells/HellishAura")]
  public class HellishAura : Spell
  {

    public override void Activate()
    {
      SpellSpawn(iD, ServiceLocator.Get<EntityTracker>().ReturnBestTarget(FocusEntity.Random).Pivot);
    }


  }
}
