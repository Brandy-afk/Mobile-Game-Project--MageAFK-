using MageAFK.Management;
using MageAFK.Player;
using MageAFK.Stats;
using MageAFK.TimeDate;
using UnityEngine;

namespace MageAFK.Spells
{

  [CreateAssetMenu(fileName = "Overflow", menuName = "Spells/Overflow")]
  public class Overflow : Spell
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
        cachedValue = ReturnStatValue(Stat.ExperienceDrops, false);

      float value = state ? cachedValue : -cachedValue;

      ServiceLocator.Get<PlayerStatHandler>().ModifyStat(Stat.ExperienceDrops, value, false);

      if (!state)
        cachedValue = 0;
    }



  }
}
