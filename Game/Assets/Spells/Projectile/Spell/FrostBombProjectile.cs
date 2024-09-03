
using MageAFK.AI;
using MageAFK.Combat;
using MageAFK.Stats;
using MageAFK.Tools;
using UnityEngine;

namespace MageAFK.Spells
{

  public class FrostBombProjectile : SpellProjectile, ITrigger
  {

    [SerializeField] private float radius = 1.0f;

    private bool active = true;

    void OnDrawGizmos()
    {
      Gizmos.color = Color.red;
      Gizmos.DrawWireSphere(transform.position, radius);
    }

    private void OnEnable() => active = true;


    private void OnTriggerEnter2D(Collider2D other)
    {

      if (active && Utility.VerifyTags(targetTags, other))
      {
        Trigger(null);
        base.HandleDamage(other.GetComponentInParent<NPEntity>());
      }
    }

    public void Trigger(Collider2D source)
    {
      active = false;
      GetComponent<Rigidbody2D>().velocity = Vector2.zero;
      GetComponent<Animator>().Play("Hit");
    }

    public void DoAoEDamage()
    {
      Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, radius, ReturnMask(LayerCollision.Body));

      foreach (Collider2D collider in enemiesInRange)
      {
        if (!Utility.VerifyTags(targetTags, collider)) continue;
        NPEntity enemy = collider.GetComponentInParent<NPEntity>();
        if (enemy == null) continue; // Ensure the collider has an Enemy component
        _ = base.HandleDamage(enemy, baseDamage: spell.ReturnStatValue(Stat.Damage, false) * (spell.ReturnStatValue(Stat.AreaOfEffectDamage) / 100));
      }
    }
  }

}
