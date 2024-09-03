using MageAFK.Combat;
using UnityEngine;

namespace MageAFK.AI
{
  public abstract class EnemyProjectile : Projectile
  {
    [HideInInspector] public NPEntity source;
    protected float damage;

    public virtual void SetStats(float damage, NPEntity enemy, bool isConfused)
    {
      this.damage = damage;
      source = enemy;
    }

    public abstract void OnHit(Entity entity = null);

    public override void Disable()
    {
      gameObject.SetActive(false);
    }

    protected override void CreateEffect(Entity entity)
    {
      return;
    }
  }

}
