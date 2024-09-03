using MageAFK.Management;
using MageAFK.Pooling;
using MageAFK.Stats;
using MageAFK.Tools;
using UnityEngine;

namespace MageAFK.AI
{
  public class Ranged : Enemy
  {

    [SerializeField] private bool targetBody = true;
    [SerializeField] private Transform projectileSpawn;
    [SerializeField] private float projectileSpeed;
    [SerializeField, Tooltip("Do you want the object to rotate based on the players location")] private bool rotateProjectile = true;

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


    public void ShootObject()
    {
      Vector3 direction = (targetBody ? target.Body : target.Feet) - (Vector2)projectileSpawn.position;
      GameObject instance = ServiceLocator.Get<EnemyPooler>().GetProjectile(data.iD);
      instance.transform.position = projectileSpawn.position;

      if (rotateProjectile)
      {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        instance.transform.rotation = Quaternion.Euler(0, 0, angle);
      }
      else
      {
        bool flip = transform.localScale.x == -1;
        Utility.FlipXSprite(Vector2.zero, Vector2.zero, instance.transform, flip);
      }

      if (instance.TryGetComponent<EnemyProjectile>(out var projectile))
      {
        Utility.SetVelocity(direction, instance, projectileSpeed / 10);
        projectile.SetStats(runtimeStats[Stat.Damage], this, states[States.isConfused]);
      }
      else
      {
        Debug.LogWarning("No ProjectileController found on the projectile instance.");
      }
    }

  }

}