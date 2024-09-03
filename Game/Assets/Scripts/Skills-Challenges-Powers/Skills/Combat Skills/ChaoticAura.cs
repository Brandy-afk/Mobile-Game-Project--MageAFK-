
using MageAFK.Combat;
using MageAFK.Core;
using MageAFK.Management;
using UnityEngine;

namespace MageAFK.Skills
{
  [CreateAssetMenu(fileName = "ChaoticAura", menuName = "Skill/Combat/ChaoticAura")]
  public class ChaoticAura : Skill, IWaveUpdateAction
  {

    //Default values originally
    [Space(10)]
    [Header("ChaoticAura Fields")]
    [SerializeField] private StatusBlueprint[] effectBps;

    public float ReturnTimeBetweenUpdates() => upgradeValues[currentRank - 1];

    public void UpdateAction()
    {
      StatusHandler enemyStatusHandler = ServiceLocator.Get<EntityTracker>().ReturnBestTarget(FocusEntity.Random).Transform.GetComponent<StatusHandler>();
      if (enemyStatusHandler == null) { return; }
      if (effectBps.Length < 1) { return; }
      int roll = Random.Range(0, effectBps.Length);

      var bp = effectBps[roll];
      enemyStatusHandler.CreateEffect(OrginType.Other, bp.status, bp.magnitude, bp.duration, (int)StatusIdentifiers.ChaoticAura);
    }
  }

}