
using System.Collections.Generic;
using MageAFK.AI;
using MageAFK.Combat;
using MageAFK.Stats;
using MageAFK.Tools;
using UnityEngine;

namespace MageAFK.Spells
{
  public class ThunderangProjectile : SpellProjectile
  {

    private HashSet<Transform> hits = new();
    private int count = 0;

    private void OnDrawGizmos()
    {
      Gizmos.color = Color.red;
      Gizmos.DrawWireSphere(transform.position, (spell as Thunderang).ReturnStatValue(Stat.Range));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
      if (!hits.Contains(other.transform) && Utility.VerifyTags(targetTags, other))
      {
        hits.Add(other.transform.parent);
        count++;

        var enemy = other.GetComponentInParent<Enemy>();
        HandleDamage(enemy);
        enemy.SubscribeToEnemyDeath(OnEnemyDeath, true);
      }
    }

    protected override CollisionInformation HandleDamage(NPEntity entity, bool forceCrit = false, bool forceStatus = false, bool forcePierce = false, float baseDamage = .01f)
    {
      base.HandleDamage(entity);

      if (!(spell as Thunderang).SetNewTarget(entity.transform, gameObject, hits, count, targetTags))
        Disable();

      return default;
    }


    private void OnEnemyDeath(Enemy enemy) => hits.Remove(enemy.Transform);

    private void OnDisable()
    {
      foreach (var col in hits) col.GetComponentInParent<Enemy>().SubscribeToEnemyDeath(OnEnemyDeath, false);
      hits.Clear();
      count = 0;
    }

  }


}

