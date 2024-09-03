
using MageAFK.AI;
using MageAFK.Combat;
using MageAFK.Tools;
using UnityEngine;

namespace MageAFK.Spells
{
  public class EtherealSpikeController : SpellProjectile, ITrigger
  {


    private PierceStage stage;
    private Collider2D returnCollider = null;
    private Animator animator;
    private Rigidbody2D rb;
    private bool active = true;

    private void Start()
    {
      animator = GetAnimator();
      rb = GetComponent<Rigidbody2D>();
    }

    public void SetUp(PierceStage stage, Collider2D rCollider)
    {
      active = true;
      this.stage = stage;
      returnCollider = rCollider;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
      if (active && Utility.VerifyTags(targetTags, other))
      {
        if (returnCollider != null && other == returnCollider) { return; }
        DoDamage(other.GetComponentInParent<NPEntity>(), other);
      }
    }




    public void DoDamage(NPEntity enemy, Collider2D other)
    {
      CollisionInformation information = base.HandleDamage(enemy);
      if (gameObject.activeInHierarchy)
      {
        animator.Play("Hit");
        StartCoroutine(AnimationDisable(animator));
      }

      Trigger(other);
    }

    public void Trigger(Collider2D source)
    {
      active = false;
      rb.velocity = Vector2.zero;
      if (Utility.RollChance(spell.ReturnStatValue(Stats.Stat.PierceChance)))
      {
        EtherealSpike o = spell as EtherealSpike;
        if (o != null)
        {
          o.OnPierce(transform, (PierceStage)((int)stage + 1), source);
        }
      }

    }


  }


}