
using UnityEngine;
using MageAFK.AI;
using MageAFK.Stats;
using MageAFK.Tools;


namespace MageAFK.Spells
{

    public class AcidburstProjectile : SpellProjectile
  {

    private CircleCollider2D col;

    private void Awake() => col = GetComponent<CircleCollider2D>();

    public void DoDamage()
    {
      Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(col.bounds.center, col.radius, ReturnMask(LayerCollision.Body));

      foreach (Collider2D collider in enemiesInRange)
      {
        if (!Utility.VerifyTags(targetTags, collider)) continue;
        
        NPEntity entity = collider.GetComponentInParent<NPEntity>();
        Rigidbody2D entityBody = entity.GetComponentInParent<Rigidbody2D>();


        Vector2 direction = (entityBody.transform.position - transform.position).normalized;
        entityBody.AddForce(direction * spell.ReturnStatValue(Stat.KnockBack), ForceMode2D.Impulse);

        _ = base.HandleDamage(entity);
      }
    }
  }

}
