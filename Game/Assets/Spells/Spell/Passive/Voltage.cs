using MageAFK.Core;
using MageAFK.Management;
using MageAFK.Player;
using MageAFK.Stats;
using MageAFK.TimeDate;
using MageAFK.Tools;
using UnityEngine;

namespace MageAFK.Spells
{

  [CreateAssetMenu(fileName = "Voltage", menuName = "Spells/Voltage")]
  public class Voltage : Spell
  {

    private float timer = 0;
    private GameObject rangeVisual;

    public override void Activate()
    {
      //Logic to spawn or start attack range visual.
      timer = ReturnStatValue(Stat.ProcInterval, false);
      ServiceLocator.Get<TimeTaskHandler>().AddTimer(OnDurationOver, RollAttack, ReturnStatValue(Stat.SpellDuration));
      rangeVisual = SpellSpawn(SpellIdentification.SpellUtility_Range, PlayerController.Positions.Pivot, true, iD);
      rangeVisual.GetComponent<SpellRangeVisualizer>().SetUpVisualizer(ReturnStatValue(Stat.Range));

    }

    public void RollAttack()
    {
      if (timer > 0)
      {
        timer -= Time.deltaTime;
        return;
      }

      timer = ReturnStatValue(Stat.ProcInterval, false);

      if (Utility.RollChance(ReturnStatValue(Stat.SpecialChance)))
      {
        var entityPositions = ServiceLocator.Get<EntityTracker>().ReturnRandomEnemyInRange(ReturnStatValue(Stat.Range), PlayerController.Positions.Transform);
        if (entityPositions != null)
        {
          var instance = SpellSpawn(iD, new Vector2(entityPositions.Pivot.x, entityPositions.Pivot.y - .02f), true);
          Utility.FlipXSprite(PlayerController.Positions.Pivot, entityPositions.Pivot, instance.transform);
          instance.GetComponent<SingleTargetController>().target = entityPositions.GetCollider(AI.NPEntityCollider.Feet);
        }
      }

    }

    public void OnDurationOver()
    {
      if (rangeVisual != null) rangeVisual.gameObject.SetActive(false);
      rangeVisual = null;
    }
  }
}
