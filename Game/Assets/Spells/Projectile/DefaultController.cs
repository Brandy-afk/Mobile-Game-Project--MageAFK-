using UnityEngine;
using MageAFK.AI;
using MageAFK.Combat;
using MageAFK.Tools;



namespace MageAFK.Spells
{


  public class DefaultController : SpellProjectile
  {
    protected Rigidbody2D rb;
    protected Animator animator;
    protected bool active = false;
    protected void Start()
    {
      rb = GetComponent<Rigidbody2D>();
      animator = GetAnimator();
    }

    protected virtual void OnEnable() => active = true;


    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
      if (active && Utility.VerifyTags(targetTags, other))
      {
        HandleDamage(other.GetComponentInParent<NPEntity>());
      }
    }

    protected virtual CollisionInformation HandleDamage(NPEntity entity)
    {
      CollisionInformation information = base.HandleDamage(entity);

      if (!information.isPierce)
      {
        active = false;
        if (animator != null)
        {
          rb.velocity = Vector2.zero;
          if (gameObject.activeInHierarchy)
          {
            animator.Play("Hit");
            StartCoroutine(AnimationDisable(animator));
          }
        }
      }
      return information;
    }

  }
}
