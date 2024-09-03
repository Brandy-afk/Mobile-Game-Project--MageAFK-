
using UnityEngine;
using MageAFK.AI;
using MageAFK.Tools;


namespace MageAFK.Spells
{

    public class GrowthProjectile : SpellProjectile
  {


    //Needs to be set intially
    [HideInInspector]
    public bool intial = false;

    private CapsuleCollider2D c;

    private void Awake() => c = GetComponent<CapsuleCollider2D>();

    public void DoDamage()
    {
      if (!c) return;


      Collider2D[] enemiesInRange = Physics2D.OverlapCapsuleAll(c.bounds.center, c.size, CapsuleDirection2D.Horizontal, 0, ReturnMask(LayerCollision.Body));

      foreach (Collider2D collider in enemiesInRange)
      {
        if (!Utility.VerifyTags(targetTags, collider)) continue;
        NPEntity entity = collider.GetComponentInParent<NPEntity>();
        if (entity == null)
        {
          Debug.Log($"Issue concerning this object : {gameObject.name}");
          continue;
        }
        base.HandleDamage(entity);

        if (intial)
        {
          (spell as Growth).SpawnGrowth(collider.GetComponentInParent<IEntityPosition>());
        }
      }

    }

  }

}
