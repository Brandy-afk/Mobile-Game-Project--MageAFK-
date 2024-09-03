using MageAFK.AI;
using MageAFK.Combat;
using MageAFK.Core;
using MageAFK.Spells;
using MageAFK.Stats;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MageAFK.Skills
{
  [CreateAssetMenu(fileName = "Introduction", menuName = "Skill/Combat/Introductions")]
  public class Introductions : Skill, ISpellCollisionEvent
  {
    [BoxGroup("CollisionEffect"), Header("Decides when it will effect damage")]
    public Priority priority;

    public void HandleCollision(Spell spell, NPEntity entity, ref float damage, ref float mod, bool isCrit, bool isPierce, bool isEffect)
    {
      if (entity.runtimeStats[Stat.Health] >= entity.data.GetStats(AIDataType.Altered)[Stat.Health])
        mod = +upgradeValues[currentRank - 1];
    }

    public Priority ReturnPriority() => priority;
  }
}