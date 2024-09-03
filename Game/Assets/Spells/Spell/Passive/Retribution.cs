
using MageAFK.AI;
using MageAFK.Combat;
using MageAFK.Core;
using MageAFK.Management;
using MageAFK.Player;
using MageAFK.Stats;
using MageAFK.TimeDate;
using MageAFK.Tools;
using UnityEngine;

namespace MageAFK.Spells
{

  [CreateAssetMenu(fileName = "Retribution", menuName = "Spells/Retribution")]
  public class Retribution : Spell, IPlayerCollisionEvent
  {
    [SerializeField] private Priority priority;
    private bool active = false;



    public override void Activate()
    {
      active = true;
      Proc();
      ServiceLocator.Get<TimeTaskHandler>().AddTimer(OnDurationOver, null, ReturnStatValue(Stat.SpellDuration));
    }

    public void OnDurationOver() => active = false;

    public void HandleCollision(NPEntity entity, ref float damage, ref float mod)
    {
      if (!active) return;
      if (Utility.RollChance(ReturnStatValue(Stat.SpecialChance)))
        Proc();
    }

    private void Proc()
    {
      SpawnEffect(PlayerController.Positions.Pivot, iD);
      SpellCastHandler.ReturnRandomActiveSpell()?.Activate();
    }

    public Priority ReturnPriority() => priority;
  }
}
