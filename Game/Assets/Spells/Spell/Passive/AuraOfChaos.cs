using MageAFK.Core;
using MageAFK.Management;
using UnityEngine;

namespace MageAFK.Spells
{

  [CreateAssetMenu(fileName = "AuraOfChaos", menuName = "Spells/Passives/AuraOfChaos")]
  public class AuraOfChaos : Spell
  {
    public override void Activate() => SpellSpawn(iD, ServiceLocator.Get<EntityTracker>().ReturnBestTarget(FocusEntity.Random).Pivot);
  }

}
