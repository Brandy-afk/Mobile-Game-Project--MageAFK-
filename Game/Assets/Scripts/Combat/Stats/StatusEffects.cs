
using System.Collections.Generic;
using System.Linq;
using MageAFK.AI;
using MageAFK.Core;
using MageAFK.Management;
using MageAFK.Spells;
using MageAFK.Stats;
using MageAFK.Tools;
using MageAFK.UI;
using UnityEngine;


namespace MageAFK.Combat
{

    #region StatusEffect
    [System.Serializable]
  public abstract class StatusEffect
  {
    public OrginType source;
    public int iD;
    public float duration; // How long does this effect last?
    public float startingDuration; // Starting duration
    public float waitTime = -1; // If Status needs to be updated, this is the time between updates (burns, execute) -1 means it will be applied once
    public bool decrement = true;


    public StatusEffect(float duration, int iD, OrginType source)
    {
      this.source = source;
      this.duration = duration;
      if (duration == -5) decrement = false;
      startingDuration = duration;
      this.iD = iD;
    }




    public virtual bool AddEffect(Dictionary<StatusType, Dictionary<(OrginType, int), StatusEffect>> activeEffects, Entity entity, bool isNew) { return false; }
    public virtual void Apply(Entity entity) { /* common apply code */ }
    public virtual void Remove(Entity entity, Dictionary<StatusType, Dictionary<(OrginType, int), StatusEffect>> activeEffects) { /* Remove logic */ }
  }
  #endregion

  #region SlowEffect
  public class SlowEffect : StatusEffect
  {
    public float magnitude; // Custom field for SlowEffect
    public SlowEffect(float duration, float magnitude, int iD, OrginType source)
        : base(duration, iD, source)
    {
      this.magnitude = magnitude;
    }

    public override bool AddEffect(Dictionary<StatusType, Dictionary<(OrginType, int), StatusEffect>> activeEffects, Entity entity, bool isNew)
    {
      if (isNew)
      {
        activeEffects[StatusType.Slow].Add((source, iD), this);
        ApplyBest(entity, activeEffects[StatusType.Slow]);
      }

      foreach (var effect in activeEffects[StatusType.Slow].Values)
        effect.duration = effect.startingDuration;

      if (activeEffects.ContainsKey(StatusType.Burn) && activeEffects[StatusType.Burn].Count > 0)
      {
        var toRemove = activeEffects[StatusType.Burn].Keys.ToArray();

        foreach (var iD in toRemove)
          activeEffects[StatusType.Burn].Remove(iD);
      }

      return true;
    }

    public static void ApplyBest(Entity entity, Dictionary<(OrginType, int), StatusEffect> effects)
    {
      if (effects.Count <= 0) { entity.HandleStatusEffect(Stat.MovementSpeed); return; }

      SlowEffect bestSlowEffect = effects.Values.Select(e => e as SlowEffect)
                                                .OrderByDescending(se => se.magnitude)
                                                .FirstOrDefault();

      if (bestSlowEffect != null)
        entity.HandleStatusEffect(Stat.MovementSpeed, bestSlowEffect.magnitude);
    }

    public override void Remove(Entity entity, Dictionary<StatusType, Dictionary<(OrginType, int), StatusEffect>> activeEffects)
    {
      ApplyBest(entity, activeEffects[StatusType.Slow]);
    }

  }
  #endregion

  #region BurnEffect
  public class BurnEffect : StatusEffect
  {

    public float magnitude; //Amount of damage

    public const float timeBetweenBurn = .25f;


    public BurnEffect(float duration, float magnitude, int iD, OrginType source)
        : base(duration, iD, source)
    {
      this.magnitude = magnitude;
      waitTime = timeBetweenBurn;
    }

    public override bool AddEffect(Dictionary<StatusType, Dictionary<(OrginType, int), StatusEffect>> activeEffects, Entity entity, bool isNew)
    {

      if (activeEffects.ContainsKey(StatusType.Slow) && activeEffects[StatusType.Slow].Count > 0)
      {
        var spellsToRemove = activeEffects[StatusType.Slow].Keys.ToArray();

        foreach (var iD in spellsToRemove)
        {
          activeEffects[StatusType.Slow].Remove(iD);
        }

        SlowEffect.ApplyBest(entity, activeEffects[StatusType.Slow]);
      }


      if (isNew)
      {
        activeEffects[StatusType.Burn].Add((source, iD), this);
        return false;
      }
      else
      {
        activeEffects[StatusType.Burn][(source, iD)].duration = startingDuration;
        return true;
      }

    }

    public override void Apply(Entity entity) => entity.StatusDamage(StatusType.Burn, magnitude, iD, source, TextInfoType.Burn);


  }
  #endregion

  #region StunEffect
  public class StunEffect : StatusEffect
  {


    public StunEffect(float duration, int iD, OrginType source)
         : base(duration, iD, source)
    {
    }

    public override bool AddEffect(Dictionary<StatusType, Dictionary<(OrginType, int), StatusEffect>> activeEffects, Entity entity, bool isNew)
    {
      if (!isNew)
      {
        activeEffects[StatusType.Stun][(source, iD)].duration = startingDuration;
        return true;
      }
      else
      {
        activeEffects[StatusType.Stun].Add((source, iD), this);
        return false;
      }

    }
    public override void Apply(Entity entity)
    {
      entity.SetState(States.isStunned, true);
      // Entity.GetComponent<EnemyStateManager>().ChangeCurrentState(EntityAnimation.KnockedDown); // TODO
    }
    public override void Remove(Entity entity, Dictionary<StatusType, Dictionary<(OrginType, int), StatusEffect>> activeEffects)
    {
      if (activeEffects[StatusType.Stun].Count < 1)
      {
        entity.SetState(States.isStunned, false);
      }
    }
  }
  #endregion

  #region CorruptionEffect
  public class CorruptionEffect : StatusEffect
  {

    public float magnitude; // Chance to corrupt enemy (killing them)

    public float timeBetweenCorruptionTick = 1f;
    public CorruptionEffect(float duration, float magnitude, int iD, OrginType source)
         : base(duration, iD, source)
    {
      this.magnitude = magnitude;
      waitTime = timeBetweenCorruptionTick;
    }
    public override bool AddEffect(Dictionary<StatusType, Dictionary<(OrginType, int), StatusEffect>> activeEffects, Entity entity, bool isNew)
    {
      if (!isNew)
      {
        activeEffects[StatusType.Corrupt][(source, iD)].duration = startingDuration;
        return true;
      }
      else
      {
        activeEffects[StatusType.Corrupt].Add((source, iD), this);
        return false;
      }

    }

    public override void Apply(Entity entity)
    {
      if (Utility.RollChance(magnitude))
      {
        entity.StatusDamage(StatusType.Corrupt, (entity as NPEntity).ReturnStat(Stat.Health), iD, source, TextInfoType.Corrupt);
        ServiceLocator.Get<MilestoneHandler>().UpdateMileStone(MilestoneID.CorruptionKills, 1);
      }
    }

  }
  #endregion

  #region ConfusionEffect
  public class ConfusionEffect : StatusEffect
  {

    public ConfusionEffect(float duration, int iD, OrginType source)
       : base(duration, iD, source)
    {
    }


    public override bool AddEffect(Dictionary<StatusType, Dictionary<(OrginType, int), StatusEffect>> activeEffects, Entity entity, bool isNew)
    {
      if (!isNew)
      {
        activeEffects[StatusType.Confuse][(source, iD)].duration = startingDuration;
        return true;
      }
      else
      {
        activeEffects[StatusType.Confuse].Add((source, iD), this);
        if (activeEffects[StatusType.Confuse].Count == 1)
        {
          return false;
        }
        return true;
      }

    }

    public override void Apply(Entity entity)
    {
      var member = entity as IConfused;
      if (member != null)
      {
        entity.SetState(States.isConfused, true);
        member.OnConfused();
      }
    }
    public override void Remove(Entity entity, Dictionary<StatusType, Dictionary<(OrginType, int), StatusEffect>> activeEffects)
    {
      var member = entity as IConfused;
      if (member != null && activeEffects[StatusType.Confuse].Count <= 0)
      {
        entity.states[States.isConfused] = false;
        member.RemoveConfused();
      }
    }
  }
  #endregion

  #region HolyEffect
  public static class HolyEffect
  {
    public static void Apply(Entity entity, float magnitude, int iD, OrginType orgin)
    {
      NPEntity nPEntity = entity as NPEntity;
      if (nPEntity != null)
      {
        var damage = nPEntity.data.GetStats(AIDataType.Altered)[Stat.Health] * (magnitude /* Represents magnitude */ / 100);
        entity.StatusDamage(StatusType.Smite, damage, iD, orgin, TextInfoType.Smite);

        Spell.SpawnEffect(entity.transform.position, SpellEffectAnimation.SmiteEffect);
      }
    }

  }
  #endregion

  #region BleedEffect
  public class BleedEffect : StatusEffect
  {

    public float baseBleed;
    public float currentBleedDamage;
    public int stacks = 1;
    public const float baseDuration = 2f;
    private const float waitBetweenBleed = .8f;

    /// <summary>
    /// Bleed effect should only have one occurence. 
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="bleedDamage"></param>
    /// <param name="iD"></param>
    /// <param name="source"></param>
    public BleedEffect(float duration, float bleedDamage, int iD, OrginType source)
       : base(duration, iD, source)
    {
      waitTime = waitBetweenBleed;
      baseBleed = bleedDamage;
      currentBleedDamage = bleedDamage;
    }

    public override bool AddEffect(Dictionary<StatusType, Dictionary<(OrginType, int), StatusEffect>> activeEffects, Entity entity, bool isNew)
    {
      if (activeEffects[StatusType.Bleed].Count > 0)
      {
        ServiceLocator.Get<MilestoneHandler>().UpdateMileStone(MilestoneID.StackBleed, 1);
        var currentEffect = activeEffects[StatusType.Bleed].First().Value as BleedEffect;

        currentEffect.stacks++;
        currentEffect.currentBleedDamage += baseBleed;
        currentEffect.duration = baseDuration;
        return true;
      }
      else
      {
        activeEffects[StatusType.Bleed].Add((source, iD), this);
        return false;
      }

    }

    public override void Apply(Entity entity) => entity.StatusDamage(StatusType.Bleed, currentBleedDamage, iD, source, TextInfoType.Bleed);
  }
  #endregion

  #region WeakenEffect
  public class WeakenEffect : StatusEffect
  {
    public float magnitude; // Percentage Of Entity damage to lower.
    public WeakenEffect(float duration, float magnitude, int iD, OrginType source)
        : base(duration, iD, source)
    {
      this.magnitude = magnitude;
    }
    public override bool AddEffect(Dictionary<StatusType, Dictionary<(OrginType, int), StatusEffect>> activeEffects, Entity entity, bool isNew)
    {
      if (isNew)
      {
        activeEffects[StatusType.Weaken].Add((source, iD), this);
      }


      ApplyBest(entity, activeEffects[StatusType.Weaken]);

      foreach (WeakenEffect effect in activeEffects[StatusType.Weaken].Values)
      {
        effect.duration = effect.startingDuration;
      }
      return false;
    }

    public static void ApplyBest(Entity entity, Dictionary<(OrginType, int), StatusEffect> effects)
    {
      if (effects.Count <= 0) { entity.HandleStatusEffect(Stat.Damage); return; }

      WeakenEffect bestEffect = null;
      foreach (StatusEffect effect in effects.Values)
      {
        if (effect is WeakenEffect weakenEffect)
        {
          if (bestEffect == null || weakenEffect.magnitude > bestEffect.magnitude)
          {
            bestEffect = weakenEffect;
          }
        }
      }

      if (bestEffect != null)
      {
        entity.HandleStatusEffect(Stat.Damage, bestEffect.magnitude);
      }
    }

    public override void Remove(Entity entity, Dictionary<StatusType, Dictionary<(OrginType, int), StatusEffect>> activeEffects)
    {
      ApplyBest(entity, activeEffects[StatusType.Weaken]);
    }
  }
  #endregion

  #region  RootEffect
  public class RootEffect : StatusEffect
  {
    public RootEffect(float duration, int iD, OrginType source)
        : base(duration, iD, source)
    {
    }

    public override bool AddEffect(Dictionary<StatusType, Dictionary<(OrginType, int), StatusEffect>> activeEffects, Entity entity, bool isNew)
    {
      if (!isNew)
      {
        activeEffects[StatusType.Root][(source, iD)].duration = startingDuration;
      }
      else
      {
        activeEffects[StatusType.Root].Add((source, iD), this);
      }

      return false;
    }

    public override void Apply(Entity entity)
    {
      entity.states[States.isRooted] = true;
      (entity as NPEntity).Rb.velocity = Vector2.zero;
      Spell.SpawnEffect(entity.transform.position, SpellEffectAnimation.RootEffect);
    }
    public override void Remove(Entity entity, Dictionary<StatusType, Dictionary<(OrginType, int), StatusEffect>> activeEffects)
    {
      if (activeEffects[StatusType.Root].Count <= 0)
      {
        entity.states[States.isRooted] = false;
      }
    }
  }
  #endregion

  #region PoisonEffect
  public class PoisonEffect : StatusEffect
  {
    public float magnitude; // Damage increase percentage (ex should be decimal form when added)

    public PoisonEffect(float duration, float magnitude, int iD, OrginType source)
        : base(duration, iD, source)
    {
      this.magnitude = magnitude / 100;
    }
    public override bool AddEffect(Dictionary<StatusType, Dictionary<(OrginType, int), StatusEffect>> activeEffects, Entity entity, bool isNew)
    {
      if (isNew)
      {
        activeEffects[StatusType.Poison].Add((source, iD), this);
      }

      foreach (PoisonEffect effect in activeEffects[StatusType.Poison].Values)
      {
        effect.duration = effect.startingDuration;
      }

      return false;
    }

    public static float ReturnBest(Dictionary<(OrginType, int), StatusEffect> effects)
    {
      var bestEffect = 0f;
      if (effects != null && effects.Count > 0)
      {
        foreach (StatusEffect effect in effects.Values)
        {
          if (effect is PoisonEffect poisonEffect)
          {
            if (poisonEffect.magnitude > bestEffect)
            {
              bestEffect = poisonEffect.magnitude;
            }
          }
        }
        return bestEffect;
      }

      return bestEffect;
    }
  }
  #endregion

  #region  FearEffect
  public class FearEffect : StatusEffect
  {
    public FearEffect(float duration, int iD, OrginType source)
        : base(duration, iD, source)
    {
    }

    public override bool AddEffect(Dictionary<StatusType, Dictionary<(OrginType, int), StatusEffect>> activeEffects, Entity entity, bool isNew)
    {

      if (activeEffects[StatusType.Fear].Count > 0)
        ServiceLocator.Get<MilestoneHandler>().UpdateMileStone(MilestoneID.FearFearedEnemies, 1);

      if (!isNew)
      {
        activeEffects[StatusType.Fear][(source, iD)].duration = startingDuration;
      }
      else
      {
        activeEffects[StatusType.Fear].Add((source, iD), this);
      }

      Apply(entity);

      return true;
    }

    public override void Apply(Entity entity)
    {
      var member = entity as IFeared;
      if (member != null)
      {
        entity.SetState(States.isFeared, true);
        member.OnFeared();
      }
    }
    public override void Remove(Entity entity, Dictionary<StatusType, Dictionary<(OrginType, int), StatusEffect>> activeEffects)
    {
      if (activeEffects[StatusType.Fear].Count <= 0)
      {
        entity.states[States.isFeared] = false;
      }
    }
  }
  #endregion


}
