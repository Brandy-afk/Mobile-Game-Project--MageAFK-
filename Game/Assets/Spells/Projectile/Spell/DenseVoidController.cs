
using MageAFK.AI;
using MageAFK.Testing;
using MageAFK.Tools;
using UnityEngine;

namespace MageAFK.Spells
{
  public class DenseVoidController : SpellProjectile
  {

    [SerializeField] private SuctionSpellTesting testing;

    private Rigidbody2D rb;
    private Animator animator;


    private float lastDamageTime = 0f;
    private float spawnTime;
    private bool active = false;
    private float duration;

    private void Start()
    {
      rb = GetComponent<Rigidbody2D>();
      animator = GetAnimator();
    }

    void OnDrawGizmos()
    {
      // Set a color for the gizmo
      Gizmos.color = Color.red; // You can change this to your preferred color

      // Draw a wire sphere around the black hole representing the damage threshold
      Gizmos.DrawWireSphere(transform.position, testing.damageThreshold);
    }
    private void OnEnable()
    {
      spawnTime = Time.time;
      duration = spell.ReturnStatValue(Stats.Stat.SpellDuration);
      active = true;
    }

    private void Update()
    {
      if (active && Time.time > (spawnTime + duration))
      {
        active = false;
        rb.velocity = Vector2.zero;

        if (gameObject.activeInHierarchy)
        {
          animator.Play("End");
          StartCoroutine(AnimationDisable(animator));
        }
      }
    }


    void OnTriggerStay2D(Collider2D collider)
    {
      if (!active || !Utility.VerifyTags(targetTags, collider)) return;
      NPEntity entity = collider.GetComponentInParent<NPEntity>();
      Rigidbody2D rb = collider.GetComponentInParent<Rigidbody2D>();
      if (rb != null)
      {
        Vector2 directionToCenter = (Vector2)transform.position - rb.position;
        float distance = directionToCenter.magnitude;

        // Check if the distance is within the damage threshold
        if (distance <= testing.damageThreshold && Time.time - lastDamageTime >= testing.timeBetweenDamage)
        {
          // Update last damage time.
          lastDamageTime = Time.time;

          // Assuming the object has a method like "TakeDamage"
          if (entity != null)
          {
            base.HandleDamage(entity);
          }
        }

        if (entity.states[States.isRooted] || entity.states[States.isDead]) { return; }
        else
        {
          float forceMagnitude = Mathf.Clamp(1.0f / distance, 0, testing.maxForce);
          Vector2 force = directionToCenter.normalized * forceMagnitude * testing.suctionPower;  // Multiply by suctionPower here
          rb.AddForce(force);
        }
      }
    }
  }
}
