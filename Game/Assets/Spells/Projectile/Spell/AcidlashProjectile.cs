
using UnityEngine;
using MageAFK.AI;

namespace MageAFK.Spells
{

  public class AcidlashProjectile : SingleTargetController
  {

    [SerializeField] protected Vector2 specialSize;
    [SerializeField] protected Vector2 specialOffset;

    [HideInInspector] public Animator animator { get; private set; }

    private void Awake() => animator = GetAnimator();
    protected override void OnDrawGizmos()
    {
      base.OnDrawGizmos();
      Gizmos.color = Color.green;
      Gizmos.DrawWireCube((Vector2)transform.position + specialOffset, specialSize);
    }

    public void DoSpecialDamage()
    {
      if (CheckForCollider(specialOffset, specialSize))
      {
        HandleDamage(target.GetComponentInParent<NPEntity>(), true, true);
      }
    }

  }

}
