
using UnityEngine;
using MageAFK.AI;
using Sirenix.OdinInspector;
using MageAFK.Tools;


namespace MageAFK.Spells
{
  public class HolyArrowProjectile : DefaultController
  {

    [ReadOnly] public Transform target;

    protected override void OnTriggerEnter2D(Collider2D other)
    {
      if (active && Utility.VerifyTags(targetTags, other))
      {
        DoDamage(other.GetComponentInParent<NPEntity>());
      }
    }

    public void DoDamage(NPEntity entity)
    {
      bool isTarget = target == entity.transform;
      HandleDamage(entity, isTarget, false, !isTarget);

      if (isTarget)
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
    }

    public override void Disable()
    {
      base.Disable();
      target = null;
    }

  }

}
