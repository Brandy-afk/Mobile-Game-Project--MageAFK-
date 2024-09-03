
using MageAFK.Core;
using MageAFK.Management;
using UnityEngine;

namespace MageAFK.Spells
{

  [CreateAssetMenu(fileName = "New Skill", menuName = "Spells/DemonicPortal")]
  public class DemonicPortal : Spell
  {
    public override void Activate()
    {
      SpellSpawn(iD, ServiceLocator.Get<EntityTracker>().ReturnBestTarget(Focus).Feet);
    }
  }

}
