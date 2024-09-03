
using UnityEngine;
using MageAFK.AI;


namespace MageAFK.Spells
{

  public class TerrorcutProjectile : SingleTargetController
  {
    [SerializeField] protected Vector2[] angledSize;
    [SerializeField] protected Vector2[] angledOffset;


    protected override void OnDrawGizmos()
    {
      Gizmos.color = Color.red;
      Gizmos.DrawWireCube((Vector2)transform.position + offset, size);

      for (int i = 0; i < angledSize.Length; i++)
      {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube((Vector2)transform.position + angledOffset[i], angledSize[i]);
      }
    }

    public void LinearAttack()
    {
      DoDamage();
    }


    public void AngularAttack()
    {
      for (int i = 0; i < angledSize.Length; i++)
      {
        if (CheckForCollider(angledOffset[i], angledSize[i]))
        {
          HandleDamage(target.GetComponentInParent<NPEntity>());
          return;
        }
      }
    }
  }

}
