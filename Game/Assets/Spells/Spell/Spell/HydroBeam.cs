
using MageAFK.Core;
using MageAFK.Management;
using MageAFK.Player;
using MageAFK.Tools;
using UnityEngine;

namespace MageAFK.Spells
{

  [CreateAssetMenu(fileName = "HydroBeam", menuName = "Spells/HydroBeam")]
  public class HydroBeam : Spell
  {

    public override void Activate()
    {
      SpellSpawn(iD, PlayerController.Positions.SpellSpawn);
    }

    public void OnUpdate(GameObject instance)
    {
      Utility.SetRotation(instance, ServiceLocator.Get<EntityTracker>().ReturnBestTarget(FocusEntity.ClosestTarget).Body);
      instance.transform.position = PlayerController.Positions.SpellSpawn;
    }
  }
}
