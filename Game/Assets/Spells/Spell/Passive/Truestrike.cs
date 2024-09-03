
using MageAFK.Management;
using MageAFK.Player;
using MageAFK.Stats;
using MageAFK.TimeDate;
using UnityEngine;

namespace MageAFK.Spells
{

  [CreateAssetMenu(fileName = "Truestrike", menuName = "Spells/Truestrike")]
  public class Truestrike : Spell
  {

    private float cachedValue = 0;
    public override void Activate()
    {
      TogglePassive(true);

      SpawnEffect(PlayerController.Positions.Pivot, iD);

      ServiceLocator.Get<TimeTaskHandler>().AddTimer(OnDurationOver, null, ReturnStatValue(Stat.SpellDuration));
    }

    public void OnDurationOver()
    {
      TogglePassive(false);
    }

    private void TogglePassive(bool state)
    {
      if (state)
        cachedValue = ReturnStatValue(Stat.AimVariance, false);

      float value = state ? cachedValue : -cachedValue;

      ServiceLocator.Get<PlayerStatHandler>().ModifyStat(Stat.AimVariance, value, false);

      if (!state)
        cachedValue = 0;
    }


  }
}
