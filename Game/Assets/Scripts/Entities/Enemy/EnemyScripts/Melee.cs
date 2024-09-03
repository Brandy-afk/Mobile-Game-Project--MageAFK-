using MageAFK.Player;
using MageAFK.Stats;
using MageAFK.Tools;
using UnityEngine;

namespace MageAFK.AI
{
    public class Melee : Enemy
  {


    protected void Update()
    {
      if (states[States.isDead]) { return; }
      if (states[States.siegeOver] || states[States.isStunned])
      {
        StopIdle();
        return;
      }
      else if (states[States.isRooted]) StopIdle();
      else if (states[States.isFeared]) { return; }
      else if (states[States.isConfused]) { IsConfused(); }

      // Update orientation or other non-physics related properties
      Utility.FlipXSprite(transform.position, target.Pivot, transform);
    }

    private void FixedUpdate()
    {
      // Handle physics-based movement here
      if (states[States.isDead] || states[States.siegeOver] || states[States.isStunned]) return;
      else if (states[States.isFeared]) { IsFeared(); return; }

      MoveEntityIntoRange();
    }


    protected override void Move(Vector2 direction)
    {
      float desiredSpeed = runtimeStats[Stat.MovementSpeed];

      // Compute how much force is needed to achieve the desired speed.
      // This calculation takes the enemy's current speed in the direction of the player and determines how much more speed is needed.
      float speedInDesiredDirection = Vector2.Dot(Rb.velocity, direction);
      float speedDifference = desiredSpeed - speedInDesiredDirection;

      Vector2 moveForce = direction * speedDifference * Rb.mass;  // multiply by mass to get force

      Rb.AddForce(moveForce);

      stateManager.ChangeCurrentState(EntityAnimation.Run);
    }
    protected override void Attack()
    {
      StartCoroutine(AttackRoutine());
    }


    public void Hit()
    {
      if (target.Transform.GetComponent<Entity>().DoDamage(runtimeStats[Stat.Damage], this) && target != PlayerController.Positions)
      {
        IsConfused();
      }
    }


  }



}

