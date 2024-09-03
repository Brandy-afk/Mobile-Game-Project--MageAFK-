using UnityEngine;

namespace MageAFK.Spells
{
    public class AoEBoxController : AbstractAoE
    {

        [SerializeField] protected Vector2 size;

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube((Vector2)transform.position + gizmoOffset, size);
        }

        public void DoDamage() => DoDamage(PhysicsCheck());

        protected Collider2D[] PhysicsCheck()
        => Physics2D.OverlapBoxAll((Vector2)transform.position + gizmoOffset, size, ReturnMask(mask));
    }
}
