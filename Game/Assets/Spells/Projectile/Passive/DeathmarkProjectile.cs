

using MageAFK.AI;
using MageAFK.Combat;
using MageAFK.Stats;
using MageAFK.UI;
using UnityEngine;

namespace MageAFK.Spells
{

    public class DeathmarkProjectile : SpellProjectile
  {
    [HideInInspector]
    public NPEntity target;
    [HideInInspector]
    public Spell proc;

    public void DoDamage()
    {
      HandleDamage(target);
    }

    protected override CollisionInformation HandleDamage(NPEntity entity, bool forceCrit = false, bool forceStatus = false, bool forcePierce = false, float baseDamage = .01f)
    {

      CollisionInformation information = new CollisionInformation(entity.runtimeStats[Stat.Health], false, false, false);
      if (information.damage <= 0) return information;

      proc.AppendRecord(SpellRecordID.Kills, 1);
      entity.DoDamage(information.damage, spell, TextInfoType.Corrupt);

      return information;
    }

    public override void Disable()
    {
      gameObject.SetActive(false);
      target = null;
      spell = null;
    }

  }

}
