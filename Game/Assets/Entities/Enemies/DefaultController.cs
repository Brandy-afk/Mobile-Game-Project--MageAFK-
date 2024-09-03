using MageAFK.Tools;
using UnityEngine;

namespace MageAFK.AI
{
    public class DefaultController : EnemyProjectile
    {
        [SerializeField] private ProjectLayerMask notConfusedLayer = ProjectLayerMask.NonPlayerBodyProjectile;
        [SerializeField] private ProjectLayerMask confusedLayer = ProjectLayerMask.ConfusedBodyProjectile;

        [SerializeField, Tooltip("Projectile has a animation it plays upon hitting enemy")] private bool hitAnim = false;

        protected Rigidbody2D rb;
        protected Animator animator;

        public override void SetStats(float damage, NPEntity enemy, bool isConfused)
        {
            base.SetStats(damage, enemy, isConfused);
            gameObject.layer = GameResources.GetLayerMask(isConfused ? confusedLayer : notConfusedLayer).index;
        }
        private void Start()
        {
            animator = GetAnimator();
            rb = GetComponent<Rigidbody2D>();
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (Utility.VerifyTags(targetTags, other))
            {
                var entity = other.GetComponentInParent<Entity>();
                OnHit(entity);
            }
        }

        public override void OnHit(Entity entity = null)
        {
            if (entity != null) entity.DoDamage(damage, source);
            rb.velocity = Vector2.zero;

            if (animator != null && hitAnim)
                animator.Play("Hit");
            else
                Disable();
        }
    }
}
