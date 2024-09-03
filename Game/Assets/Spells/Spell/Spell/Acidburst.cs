
using MageAFK.Core;
using MageAFK.Management;
using MageAFK.Player;
using MageAFK.Tools;
using UnityEngine;

namespace MageAFK.Spells
{

  [CreateAssetMenu(fileName = "Acidburst", menuName = "Spells/Acidburst")]
  public class Acidburst : Spell
  {

    public override void Activate()
    {
      GameObject instance = SpellSpawn(iD, PlayerController.Positions.SpellSpawn);
      Utility.SetRotation(instance, ServiceLocator.Get<EntityTracker>().ReturnBestTarget(FocusEntity.ClosestTarget).Body);
    }

  }
}
