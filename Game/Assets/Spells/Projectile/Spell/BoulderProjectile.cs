
using UnityEngine;
using MageAFK.AI;
using MageAFK.Stats;
using MageAFK.Tools;


namespace MageAFK.Spells
{

  public class BoulderProjectile : SpellProjectile
  {

    private Animator animator;
    private Rigidbody2D rgbd;
    private Vector2 initialDirection;
    private float initialSpeed;
    private float currentSpeed;
    private bool actionTriggered;
    private float maxSpeed;


    private void Awake()
    {
      rgbd = GetComponent<Rigidbody2D>();
      animator = GetAnimator();
    }

    public void SetUp()
    {
      // Capture initial velocity
      initialDirection = rgbd.velocity.normalized;
      initialSpeed = rgbd.velocity.magnitude;
      currentSpeed = initialSpeed;
      actionTriggered = false;

      // Set max speed to double the initial speed
      maxSpeed = initialSpeed * 2f;
    }


    private void Update()
    {
      if (currentSpeed < maxSpeed)
      {
        currentSpeed += spell.ReturnStatValue(Stat.AccelerationRate, false) * Time.deltaTime;
        currentSpeed = Mathf.Min(currentSpeed, maxSpeed);

        // Apply the updated velocity
        rgbd.velocity = initialDirection * currentSpeed;

        // Check if the speed has reached 1.5 times the initial speed
        if (!actionTriggered && currentSpeed >= initialSpeed * 1.5f)
        {
          actionTriggered = true;
          animator.Play("Fast");
        }
      }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
      if (!Utility.VerifyTags(targetTags, other)) return;
      NPEntity entity = other.GetComponentInParent<NPEntity>();
      Rigidbody2D entityBody = entity.GetComponentInParent<Rigidbody2D>();

      Vector2 direction = (entityBody.transform.position - transform.position).normalized;
      entityBody.AddForce(direction * spell.ReturnStatValue(Stat.KnockBack), ForceMode2D.Impulse);

      float damage = spell.ReturnStatValue(Stat.Damage, false) * (currentSpeed / initialSpeed);
      _ = HandleDamage(entity, false, false, false, damage);
    }


  }

}
