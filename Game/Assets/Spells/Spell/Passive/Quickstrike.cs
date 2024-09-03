
using MageAFK.Management;
using MageAFK.Player;
using MageAFK.Stats;
using MageAFK.TimeDate;
using UnityEngine;

namespace MageAFK.Spells
{

  [CreateAssetMenu(fileName = "Quickstrike", menuName = "Spells/Quickstrike")]
  public class Quickstrike : Spell
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
        cachedValue = ReturnStatValue(Stat.CooldownModifier, false);

      float value = state ? cachedValue : -cachedValue;

      ServiceLocator.Get<PlayerStatHandler>().ModifyStat(Stat.Cooldown, value, false);

      if (!state)
        cachedValue = 0;
    }

    public override float ReturnStatValue(Stat stat, bool modifyValue = true, Stat modStat = Stat.None)
    {
      modifyValue = stat == Stat.Cooldown ? false : true;
      return base.ReturnStatValue(stat, modifyValue, modStat);
    }


  }
}
