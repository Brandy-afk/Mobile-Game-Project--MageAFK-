using MageAFK.AI;
using MageAFK.Combat;
using MageAFK.Core;
using MageAFK.Spells;
using MageAFK.Tools;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MageAFK.Skills
{
  [CreateAssetMenu(fileName = "Insurmountable", menuName = "Skill/Combat/Insurmountable")]
  public class Insurmountable : Skill, ISpellCollisionEvent
  {
    [BoxGroup("CollisionEffect"), Header("Decides when it will effect damage")]
    public Priority priority;


    public void HandleCollision(Spell spell, NPEntity entity, ref float damage, ref float mod, bool isCrit, bool isPierce, bool isEffect)
    {
      if (Utility.RollChance(upgradeValues[currentRank - 1]))
        damage *= 2;
    }

    public Priority ReturnPriority() => priority;
  }
}