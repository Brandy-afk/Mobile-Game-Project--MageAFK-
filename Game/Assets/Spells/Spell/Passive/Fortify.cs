
using MageAFK.Management;
using MageAFK.Player;
using MageAFK.Stats;
using MageAFK.TimeDate;
using UnityEngine;

namespace MageAFK.Spells
{

  [CreateAssetMenu(fileName = "Fortify", menuName = "Spells/Fortify")]
  public class Fortify : Spell
  {
    private float cachedValue;
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
        cachedValue = ReturnStatValue(Stat.Armour, false);

      float value = state ? cachedValue : -cachedValue;

      ServiceLocator.Get<PlayerStatHandler>().ModifyStat(Stat.Armour, value, false);

      if (!state)
        cachedValue = 0;
    }


  }
}
