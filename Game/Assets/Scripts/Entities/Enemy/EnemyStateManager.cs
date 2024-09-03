using MageAFK.AI;

namespace MageAFK
{
    public class EnemyStateManager : EntityStateManager
    {

        private void OnEnable()
        {
            currentAnimation = EntityAnimation.Idle;
        }

        private void Update()
        {
            if (currentAnimation != EntityAnimation.Idle && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                // Change state based on current state
                switch (currentAnimation)
                {
                    case EntityAnimation.Attack:
                        ChangeCurrentState(EntityAnimation.Idle);
                        break;
                }
            }
        }
    }
}
