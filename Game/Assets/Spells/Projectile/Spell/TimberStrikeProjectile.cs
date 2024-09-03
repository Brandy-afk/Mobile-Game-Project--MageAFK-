
using UnityEngine;
using MageAFK.AI;
using MageAFK.Tools;


namespace MageAFK.Spells
{

  public class TimberStrikeProjectile : SpellProjectile
  {

    [HideInInspector] public NPEntity entity;
    [SerializeField] private Vector2 size;
    [SerializeField] private Vector2 offset;

    private void OnDrawGizmos()
    {
      Gizmos.color = Color.red;
      Gizmos.DrawWireCube((Vector2)transform.position + offset, size);
    }

    private bool active;

    private void OnEnable() => active = true;
    public void ToggelActive() => active = false;

    private void Update()
    {
      if (entity.states[States.isDead] || !entity.states[States.inMap])
        active = false;

      if (entity != null && active)
      {
        transform.position = entity.Feet;
      }
    }

    public void DoDamage()
    {
      Collider2D[] enemiesInRange = Physics2D.OverlapBoxAll((Vector2)transform.position + offset, size, ReturnMask(LayerCollision.Feet));

      foreach (Collider2D collider in enemiesInRange)
      {
        if (!Utility.VerifyTags(targetTags, collider)) continue;
        NPEntity enemy = collider.GetComponentInParent<NPEntity>();
        if (enemy == null) continue; // Ensure the collider has an Enemy component
        HandleDamage(enemy);
      }

    }
  }

}
