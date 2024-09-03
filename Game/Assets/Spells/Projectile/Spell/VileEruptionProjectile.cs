
using UnityEngine;
using MageAFK.AI;
using MageAFK.Stats;
using System.Collections.Generic;
using MageAFK.Tools;


namespace MageAFK.Spells
{

  public class VileEruptionProjectile : SingleTargetController
  {


    [SerializeField] protected Vector2 aSize;
    [SerializeField] protected Vector2 aOffset;
    protected override void OnDrawGizmos()
    {
      base.OnDrawGizmos();

      Gizmos.color = Color.green;
      Gizmos.DrawWireCube((Vector2)transform.position + aOffset, aSize);
    }

    public void SecondaryDamage()
    {
      Collider2D[] colliders = Physics2D.OverlapBoxAll((Vector2)transform.position + aOffset, size, ReturnMask(LayerCollision.Both));

      var damage = spell.ReturnStatValue(Stat.Damage, false) * (spell.ReturnStatValue(Stat.AreaOfEffectDamage) / 100);

      HashSet<NPEntity> hits = new();
      foreach (var col in colliders)
      {
        if (!Utility.VerifyTags(targetTags, col)) continue;
        NPEntity entity = col.GetComponentInParent<NPEntity>();
        if (entity == null || hits.Contains(entity)) continue; //Ensure the collider has an Enemy component
        hits.Add(entity);
        HandleDamage(entity, false, false, false, damage);
      }
    }

  }

}
