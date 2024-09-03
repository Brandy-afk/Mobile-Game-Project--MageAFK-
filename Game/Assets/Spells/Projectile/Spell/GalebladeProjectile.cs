
using UnityEngine;
using MageAFK.AI;
using System.Collections;
using MageAFK.Stats;
using MageAFK.Tools;
using MageAFK.Combat;


namespace MageAFK.Spells
{

  public class GalebladeProjectile : SpellProjectile
  {

    [SerializeField, Tooltip("Tags to be damaged by after effect (spinning blade)")] Tags[] afterEffectTargets;
    [SerializeField] private float TIME_BETWEEN_DAMAGE = .25f;
    [SerializeField] private float SLOW_FACTOR = 10f;

    private CapsuleCollider2D capsule;
    private Animator animator;
    private Rigidbody2D rgbd;
    private bool active = true;


    private void OnEnable() => active = true;
    private void Start()
    {
      capsule = GetComponent<CapsuleCollider2D>();
      animator = GetAnimator();
      rgbd = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
      if (active && Utility.VerifyTags(targetTags, other))
      {
        active = false;
        ApplyBehaviour(other);
        if (gameObject.activeInHierarchy) StartCoroutine(WaitThenAttack());
      }
    }

    private IEnumerator WaitThenAttack()
    {
      rgbd.velocity = Vector2.zero;

      float duration = spell.ReturnStatValue(Stat.AfterEffectDuration);
      float elapsedTime = 0f;

      while (elapsedTime < duration)
      {
        if (capsule)
        {
          Collider2D[] enemiesInRange = Physics2D.OverlapCapsuleAll((Vector2)transform.position + capsule.offset, capsule.size, 0, ReturnMask(LayerCollision.Feet));

          foreach (Collider2D collider in enemiesInRange)
          {
            if (Utility.VerifyTags(afterEffectTargets, collider))
              ApplyBehaviour(collider);
          }
        }

        yield return new WaitForSeconds(TIME_BETWEEN_DAMAGE);
        elapsedTime += TIME_BETWEEN_DAMAGE;
      }

      animator.Play("End");
      StartCoroutine(AnimationDisable(animator));
    }

    private void ApplyBehaviour(Collider2D collider)
    {
      NPEntity enemy = collider.GetComponentInParent<NPEntity>();
      if (enemy == null) return;
      // Calculate an opposing force
      var rgbd = enemy.GetComponentInParent<Rigidbody2D>();
      Vector2 opposingForce = -rgbd.velocity * SLOW_FACTOR; // slowFactor is a factor to control the slowing effect
                                                            // Apply the opposing force
      rgbd.AddForce(opposingForce, ForceMode2D.Force);

      _ = base.HandleDamage(enemy);
    }

  }

}
