using MageAFK.AI;
using MageAFK.Spells;
using UnityEngine;

namespace MageAFK
{
    public class SingleTargetController : SpellProjectile
    {


        [HideInInspector] public Collider2D target;
        [SerializeField] protected Vector2 size;
        [SerializeField] protected Vector2 offset;

        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube((Vector2)transform.position + offset, size);
        }

        public void DoDamage()
        {
            if (CheckForCollider(offset, size))
                HandleDamage(target.GetComponentInParent<NPEntity>());
        }

        protected bool CheckForCollider(Vector2 offset, Vector2 size)
        {
            if (target == null) return false;

            Vector3 halfExtents = new Vector3(size.x * 0.5f, size.y * 0.5f, 0f);
            var point = (transform.localScale.x < 0 ? new Vector2(-offset.x, offset.y) : offset) + (Vector2)transform.position;
            Bounds boxBounds = new(point, halfExtents);
            return boxBounds.Intersects(target.bounds);
        }

        public override void Disable()
        {
            gameObject.SetActive(false);
            target = null;
        }
    }
}
