
using UnityEngine;
using MageAFK.AI;
using System.Collections;
using MageAFK.Stats;
using MageAFK.Tools;
using MageAFK.Combat;


namespace MageAFK.Spells
{

  public class IonTrapProjectile : SpellProjectile
  {

    [SerializeField, Tooltip("Decides what kind of tags will cause the trap to proc.")] protected Tags[] trapProcTargets;
    [SerializeField] private float radius = 1.0f;
    [SerializeField] private Vector2 offset;
    private bool active = false;

    private const float TIME_BETWEEN_DAMAGE = .25f;




    private void OnEnable()
    {
      active = true;
    }


    void OnDrawGizmos()
    {
      Gizmos.color = Color.red;
      Gizmos.DrawWireSphere((Vector2)transform.position + offset, radius);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
      if (active && Utility.VerifyTags(trapProcTargets, other))
      {
        active = false;
        StartCoroutine(HandleExplosion());
      }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
      if (active)
        OnTriggerEnter2D(other);
    }

    private IEnumerator HandleExplosion()
    {
      GetComponent<Animator>().Play("Hit");
      float duration = spell.ReturnStatValue(Stat.AfterEffectDuration);
      float elapsedTime = 0f;

      while (elapsedTime < duration)
      {
        DoDamage();
        yield return new WaitForSeconds(TIME_BETWEEN_DAMAGE);
        elapsedTime += TIME_BETWEEN_DAMAGE;
      }
      Disable();
    }

    public void DoDamage()
    {
      Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll((Vector2)transform.position + offset, radius, ReturnMask(LayerCollision.Body));

      foreach (Collider2D collider in enemiesInRange)
      {
        if (!Utility.VerifyTags(targetTags, collider)) continue;
        NPEntity entity = collider.GetComponentInParent<NPEntity>();
        if (entity == null)
        {
          Debug.Log($"Issue concerning this object : {gameObject.name}");
          continue;
        }
        _ = base.HandleDamage(entity);
      }
    }

    public override void Disable()
    {
      (spell as IonTrap).Dequeue(this);
      base.Disable();
    }

  }

}
