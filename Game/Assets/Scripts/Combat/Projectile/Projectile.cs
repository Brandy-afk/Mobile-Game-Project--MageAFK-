
using System.Collections;
using MageAFK.AI;
using UnityEngine;

namespace MageAFK.Combat
{
    public abstract class Projectile : MonoBehaviour
    {
        [SerializeField, Tooltip("What tags this object will target. Does not always have an application in a projectile.\nSingle target projectiles need none")] protected Tags[] targetTags;

        protected enum LayerCollision
        {
            Both,
            Feet,
            Body
        }

        protected abstract void CreateEffect(Entity entity);
        protected virtual LayerMask ReturnMask(LayerCollision collision) { return default; }
        public virtual void Disable() { gameObject.SetActive(false); }

        #region Helpers
        protected Animator GetAnimator()
        {
            var animator = GetComponent<Animator>();
            if (animator == null)
            {
                animator = transform.GetComponentInChildren<Animator>();
                if (animator == null)
                    Debug.Log($"Something is wrong with {gameObject.name}");
            }

            return animator;
        }

        protected IEnumerator AnimationDisable(Animator animator)
        {
            while (true)
            {
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                {
                    Disable();
                    break;
                }
                yield return new WaitForSeconds(.15f);
            }
        }

        #endregion
    }

    public interface ITrigger
    {
        public void Trigger(Collider2D source);
    }
}
