using MageAFK.AI;

namespace MageAFK
{
    public class TerraGuardianStateManager : EntityStateManager
    {
        private bool actionPerformed;
        private void OnEnable() => currentAnimation = EntityAnimation.Idle;

        private void Update()
        {
            if (currentAnimation != EntityAnimation.Idle && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && !actionPerformed)
            {
                // Change state based on current state
                switch (currentAnimation)
                {
                    case EntityAnimation.Attack:
                        ChangeCurrentState(EntityAnimation.Idle);
                        break;
                    case EntityAnimation.Attack2:
                        ChangeCurrentState(EntityAnimation.Idle);
                        break;
                    case EntityAnimation.Fury:
                        ChangeCurrentState(EntityAnimation.Idle);
                        break;
                }
            }

            else if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1) // If a new animation started, reset the flag.
            {
                actionPerformed = false;
            }
        }
    }
}
