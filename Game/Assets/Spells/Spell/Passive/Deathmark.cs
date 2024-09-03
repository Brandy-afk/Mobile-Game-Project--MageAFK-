using MageAFK.AI;
using MageAFK.Combat;
using MageAFK.Core;
using MageAFK.Management;
using MageAFK.Stats;
using MageAFK.TimeDate;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MageAFK.Spells
{

  [CreateAssetMenu(fileName = "Deathmark", menuName = "Spells/Deathmark")]
  public class Deathmark : Spell, ISpellCollisionEvent
  {
    [BoxGroup("CollisionEffect"), Header("Decides when it will effect damage")]
    public Priority priority;

    private bool active = false;



    public override void Activate()
    {
      active = true;
      // PlayerController.Instance.ReturnPlayerShaderController().SetShader(deathShader);
      ServiceLocator.Get<TimeTaskHandler>().AddTimer(OnDurationOver, null, ReturnStatValue(Stat.SpellDuration));
    }

    public void OnDurationOver()
    {
      ServiceLocator.Get<PlayerShaderController>().SetShader(null);
      active = false;
    }

    public Priority ReturnPriority() => priority;

    public void HandleCollision(Spell spell, NPEntity entity, ref float damage, ref float mod, bool isCrit, bool isPierce, bool isEffect)
    {
      if (active == false) return;

      float threshold = entity.data.GetStats(AIDataType.Altered)[Stat.Health] * (ReturnStatValue(Stat.ExecutionThreshold, false) / 100);
      if (entity.runtimeStats[Stat.Health] < threshold)
      {
        var Instance = SpellSpawn(iD, entity.Body).GetComponent<DeathmarkProjectile>();
        Instance.target = entity;
        Instance.proc = spell;
      }
    }
  }
}
