using UnityEngine;
using MageAFK.AI;
using MageAFK.Tools;


namespace MageAFK.Spells
{

  public class SkewerProjectile : SpellProjectile
  {

    private BoxCollider2D col;

    private void Awake()
    {
      col = GetComponent<BoxCollider2D>();
    }

    public void DoDamage()
    {
      Collider2D[] colliders = Physics2D.OverlapBoxAll(col.bounds.center, col.size, col.transform.eulerAngles.z, ReturnMask(LayerCollision.Body));

      foreach (var col in colliders)
      {
        if (!Utility.VerifyTags(targetTags, col)) continue;
        NPEntity entity = col.GetComponentInParent<NPEntity>();
        if (entity == null) continue; // Ensure the collider has an Enemy component
        Spell.SpawnEffect(entity.transform.position, SpellEffectAnimation.Wood_1);
        HandleDamage(entity, false, true);
      }
    }
  }

}
