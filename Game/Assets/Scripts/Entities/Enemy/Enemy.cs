using MageAFK.Core;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using MageAFK.Stats;
using System;
using MageAFK.Player;
using MageAFK.Tools;
using MageAFK.Management;
using System.Collections;

namespace MageAFK.AI
{

    public abstract class Enemy : NPEntity, IConfused, IFeared
  {
    public override Dictionary<Stat, float> runtimeStats { get; set; }
    //States
    [ReadOnly]
    public override Dictionary<States, bool> states { get; set; } = new Dictionary<States, bool>
    {
      {States.isInRange, false},
      {States.isDead, false},
      {States.isRooted, false},
      {States.isStunned, false},
      {States.isConfused, false},
      {States.isFeared, false},
      {States.siegeOver, false},
       {States.inMap, false}
    };

    protected IEntityPosition target = null;

    protected Vector2 fearedDirection;

    #region LifeCycle
    void Awake()
    {
      OnAwake();
    }


    public override void OnSetActive()
    {
      base.OnSetActive();
      //Handles even more generic behaviour

      float randomRangeVariance = UnityEngine.Random.Range(-.5f, .5f);
      runtimeStats[Stat.AttackRange] += randomRangeVariance;

      target = PlayerController.Positions;
    }

    #endregion

    #region Events

    protected event Action<Enemy> OnEnemyDeath;

    public void SubscribeToEnemyDeath(Action<Enemy> handler, bool state)
    {
      if (state)
      {
        OnEnemyDeath += handler;
        if (states[States.isDead])
          handler?.Invoke(this);
      }
      else
      {
        OnEnemyDeath -= handler;
      }

    }


    #endregion

    #region Enemy Behavior
    public virtual void Spawn()
    {
      ServiceLocator.Get<EnemySpawner>().DefaultSpawn(data.iD);
    }

    protected abstract void Move(Vector2 direction);
    protected abstract void Attack();

    protected virtual IEnumerator AttackRoutine()
    {
      yield return null;
      while (GetIsInRange() && !states[States.isDead] && !states[States.isStunned])
      {
        stateManager.ChangeCurrentState(EntityAnimation.Attack);
        yield return new WaitForSeconds(runtimeStats[Stat.Cooldown]);
      }
    }
    protected bool GetIsInRange() => Vector2.Distance(transform.position, target.Pivot) < runtimeStats[Stat.AttackRange];

    protected void MoveEntityIntoRange()
    {
      if (target == null) { stateManager.ChangeCurrentState(EntityAnimation.Idle); return; }
      bool isCurrentlyInRange = GetIsInRange();
      if (!states[States.isInRange] && isCurrentlyInRange)
      {
        // Just got in range.
        Rb.velocity = Vector2.zero;
        Attack();
      }
      else if (!states[States.isRooted] && !isCurrentlyInRange)
      {
        // Not in range. Move towards the player.
        Move((target.Pivot - (Vector2)transform.position).normalized);
      }
      // Update the state for the next frame.
      states[States.isInRange] = isCurrentlyInRange;
    }

    #endregion

    #region Status

    public virtual void OnConfused()
    {
      var entityTracker = ServiceLocator.Get<EntityTracker>();
      target = entityTracker.GetClosestTarget(entityTracker.entities, transform, Mathf.Infinity);
      if (target == null) target = PlayerController.Positions;
    }
    public virtual void IsConfused()
    {
      if (!target.Transform.gameObject.activeSelf || target == PlayerController.Positions)
        OnConfused();
    }

    public virtual void RemoveConfused() => target = PlayerController.Positions;

    public virtual void OnFeared()
    {
      fearedDirection = -(PlayerController.Positions.Pivot - (Vector2)transform.position).normalized;
      fearedDirection = Utility.RotateVector2(fearedDirection, UnityEngine.Random.Range(-30, 30));
      Utility.FlipXSpriteByDirection(fearedDirection, transform);
    }

    public virtual void IsFeared()
    {
      if (Utility.IfWallReturnNewVector(Rb, transform, out Vector2 direction))
      {
        fearedDirection = direction;
        Utility.FlipXSpriteByDirection(fearedDirection, transform);
      }

      Move(fearedDirection);
    }

    #endregion


    #region Helpers

    protected void StopIdle()
    {
      Rb.velocity = Vector2.zero;
      stateManager.ChangeCurrentState(EntityAnimation.Idle);
    }

    public override void Die(bool forcedDeath = false)
    {
      base.Die(forcedDeath);
      OnEnemyDeath?.Invoke(this);
      OnEnemyDeath = null;
    }



    #endregion
  }






}