using MageAFK.Combat;
using MageAFK.Core;
using MageAFK.Management;
using MageAFK.Stats;
using UnityEngine;

namespace MageAFK.Spells
{

  [CreateAssetMenu(fileName = "GhastlyAura", menuName = "Spells/GhastlyAura")]
  public class GhastlyAura : Spell
  {

    public override void Activate()
    {
      StatusHandler enemyStatusHandler = ServiceLocator.Get<EntityTracker>().ReturnBestTarget(FocusEntity.Random).Transform.GetComponent<StatusHandler>();
      SpawnEffect(enemyStatusHandler.transform.position, iD);
      enemyStatusHandler.CreateEffect(OrginType.Spell,
                                              StatusType.Fear,
                                              0,
                                              ReturnStatValue(Stat.StatusDuration),
                                              (int)iD);
    }
  }
}
