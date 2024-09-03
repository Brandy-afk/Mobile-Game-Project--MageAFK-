
using MageAFK.Management;
using MageAFK.Player;
using MageAFK.Stats;
using MageAFK.TimeDate;
using UnityEngine;

namespace MageAFK.Spells
{

  [CreateAssetMenu(fileName = "Doomstrike", menuName = "Spells/Doomstrike")]
  public class Doomstrike : Ultimate
  {


    public override void Activate()
    {
      TogglePassive(true);

      SpawnEffect(PlayerController.Positions.Pivot, iD);

      AppendRecord(SpellRecordID.CumulativeIncrease, ReturnStatValue(Stat.DamageIncrease, false));
      ServiceLocator.Get<TimeTaskHandler>().AddTimer(OnDurationOver, null, ReturnStatValue(Stat.SpellDuration));
    }

    public void OnDurationOver()
    {
      TogglePassive(false);
    }

    private void TogglePassive(bool state)
    {
      var value = (state ? ReturnStatValue(Stat.DamageIncrease, false) : -ReturnStatValue(Stat.DamageIncrease, false)) / 100;
      ServiceLocator.Get<PlayerStatHandler>().ModifyStat(Stat.Damage, value, false);
    }

  }
}
