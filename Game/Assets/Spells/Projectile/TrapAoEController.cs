using MageAFK.Combat;
using MageAFK.Spells;
using MageAFK.Tools;
using UnityEngine;

namespace MageAFK
{
    public class TrapAoEController : AoECircleController
    {

        [SerializeField, Tooltip("Decides what kind of tags will cause the trap to proc.")] protected Tags[] trapProcTargets;
        private bool active;
        private Animator animator;

        private void Awake() => animator = GetAnimator();

        private void OnEnable()
        {
            active = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (active && Utility.VerifyTags(trapProcTargets, other))
            {
                active = false;
                animator.Play("Hit");
                StartCoroutine(AnimationDisable(animator));
            }
        }

    }
}
