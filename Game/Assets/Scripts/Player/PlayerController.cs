using System;
using System.Collections.Generic;
using MageAFK.AI;
using MageAFK.Combat;
using MageAFK.Core;
using MageAFK.Management;
using MageAFK.Spells;
using MageAFK.Stats;
using MageAFK.Tools;
using MageAFK.UI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MageAFK.Player
{
  public class PlayerController : Entity, IPlayerPosition, IPlayerDeath
  {
    public static IPlayerPosition Positions { get; private set; }

    [SerializeField, TabGroup("Locations")] private Transform projectileSpawn;

    #region Getters / Positions
    public Vector2 SpellSpawn => projectileSpawn.position;

    #endregion

    private PlayerHealth playerHealth;
    private PlayerShaderController playerShader;

    public override Dictionary<States, bool> states { get; set; } = new Dictionary<States, bool>
    {
      {States.isDead, false},
      {States.isStunned, false},
      {States.siegeOver, false},
    };
    public static object Instance { get; internal set; }

    private void Awake()
    {
      Positions = this;
      ServiceLocator.RegisterService<IPlayerDeath>(this);
      playerHealth = GetComponent<PlayerHealth>();
      playerShader = GetComponent<PlayerShaderController>();

      WaveHandler.SubToSiegeEvent((Status status) =>
     {
       if (status == Status.End_CleanUp)
       {
         EnablePlayer();
       }
     }, true);
    }

    //Events
    #region Event
    private event Action OnPlayerDeath;

    public void SubscribeToPlayerDeath(Action handler, bool state, Priority priority = Priority.Middle)
    {
      if (state)
        OnPlayerDeath = Utility.InsertHandler(OnPlayerDeath, handler, priority);
      else
        OnPlayerDeath -= handler;
    }

    #endregion

    [ReadOnly]
    private void EnablePlayer()
    {
      GetComponent<PlayerHealth>().SetHealthToBase();
      ToggleColliders(true);
      SetState(States.isDead, false);
      GetComponent<PlayerStateManager>().ChangeCurrentState(EntityAnimation.Idle, true);
    }


    private bool RollToDodge() =>
     Utility.RollChance(ServiceLocator.Get<PlayerStatHandler>().ReturnModification(Stat.DodgeChance) * 100);


    public override void Die(bool forcedDeath = false)
    {
      SetState(States.isDead, true);
      ServiceLocator.Get<WaveHandler>().EndSiege();
      ServiceLocator.Get<TimeScaleHandler>().ResetTimeScale();
      OnPlayerDeath?.Invoke();
      //Call to wave handler which will handle all needed calls for end of wave. 
      //Clean up enemy tracker to stop unwanted behaviour
      ToggleColliders(false);
      GetComponent<PlayerStateManager>().ChangeCurrentState(EntityAnimation.Die);
    }

    public override void HandleStatusEffect(Stat stat, float mod = 0, bool positive = false)
    {
      throw new NotImplementedException();
    }

    // Burn damage etc
    public override void StatusDamage(StatusType type, float damage, int iD, OrginType source, TextInfoType textType = TextInfoType.Default)
    {
      return;
    }

    public override bool DoDamage(float damage, Spell spell, TextInfoType textType = TextInfoType.Default)
    {
      //Logic not needed yet, for self spell damage
      return false;
    }

    public override bool DoDamage(float damage, NPEntity entity, TextInfoType textType = TextInfoType.Default)
    {
      if (!RollToDodge())
      {
        (bool isDead, float sumDamage) = playerHealth.Damage(damage, entity);

        MetricController.OnPlayerDamaged(sumDamage, entity.data.iD);
        ServiceLocator.Get<MilestoneHandler>().UpdateMileStone(MilestoneID.TakeDamage, sumDamage);

        HandleThornsDamage(entity);

        StartCoroutine(playerShader.FlashEntityHit());

        if (isDead)
        {
          Die();
          return true;
        }
      }

      return false;
    }

    private void HandleThornsDamage(NPEntity entity)
    {
      if (ServiceLocator.Get<EntityHandler>().GetMob(entity.data.iD).type == EnemyType.Melee)
      {
        var thornDmg = ServiceLocator.Get<PlayerStatHandler>().ReturnModification(Stat.ThornsDamage);
        if (thornDmg > 0)
          entity.DoDamage(thornDmg, TextInfoType.ThornsDamage);
      }
    }


    #region Helpers
    private void ToggleColliders(bool state)
    {
      foreach (var col in GetComponentsInChildren<Collider2D>())
        col.enabled = state;
    }


    #endregion

  }


  public interface IPlayerPosition : IEntityPosition
  {
    public Vector2 SpellSpawn { get; }

  }

  public interface IPlayerDeath
  {
    public void SubscribeToPlayerDeath(Action handler, bool state, Priority priority = Priority.Middle);
  }


}
