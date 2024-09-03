using UnityEngine;

namespace MageAFK.Spells
{
    public class AoECircleController : AbstractAoE
    {

        [SerializeField] protected float radius = 1.0f;

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere((Vector2)transform.position + gizmoOffset, radius);
        }

        public void DoDamage() => DoDamage(PhysicsCheck());

        protected Collider2D[] PhysicsCheck()
        => Physics2D.OverlapCircleAll((Vector2)transform.position + gizmoOffset, radius, ReturnMask(mask));
    }
}
