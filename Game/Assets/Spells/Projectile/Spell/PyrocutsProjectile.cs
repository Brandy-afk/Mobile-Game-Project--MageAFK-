

using MageAFK.AI;
using MageAFK.Tools;
using UnityEngine;

namespace MageAFK.Spells
{

  public class PyrocutsProjectile : SingleTargetController
  {

    [SerializeField] protected Vector2 cleaveSize;
    [SerializeField] protected Vector2 cleaveOffset;
    [SerializeField] protected Vector2 uppercutSize;
    [SerializeField] protected Vector2 uppercutOffset;

    protected override void OnDrawGizmos()
    {
      base.OnDrawGizmos();
      Gizmos.color = Color.red;
      Gizmos.DrawWireCube((Vector2)transform.position + offset, size);

      Gizmos.color = Color.green;
      Gizmos.DrawWireCube((Vector2)transform.position + cleaveOffset, cleaveSize);

      Gizmos.color = Color.blue;
      Gizmos.DrawWireCube((Vector2)transform.position + uppercutOffset, uppercutSize);
    }

    public void Cleave()
    {
      Collider2D[] colliders = Physics2D.OverlapBoxAll(
                                        (transform.localScale.x < 0 ? new Vector2(-cleaveOffset.x, cleaveOffset.y) : cleaveOffset) + (Vector2)transform.position
                                        , cleaveSize
                                        , 0
                                        , ReturnMask(LayerCollision.Body));

      foreach (var col in colliders)
      {
        if (Utility.VerifyTags(targetTags, col))
          HandleDamage(col.GetComponentInParent<NPEntity>());
      }

    }

    public void Uppercut()
    {
      if (CheckForCollider(uppercutOffset, uppercutSize))
      {
        HandleDamage(target.GetComponent<NPEntity>(), forceStatus: true);
      }
    }

    public void Claw()
    {
      if (CheckForCollider(offset, size))
      {
        HandleDamage(target.GetComponent<NPEntity>(), forceCrit: true);
      }
    }
  }



}
