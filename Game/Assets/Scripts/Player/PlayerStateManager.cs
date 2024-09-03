using MageAFK.AI;
using MageAFK.Core;

namespace MageAFK.Player
{
    public class PlayerStateManager : EntityStateManager
  {



    private void Start() => WaveHandler.SubToWaveState(OnWaveChanged, true);

    private void OnWaveChanged(WaveState waveState)
    {
      switch (waveState)
      {
        case WaveState.Wave:
          ChangeCurrentState(EntityAnimation.PrepareToAttack);
          break;

        default:
          ChangeCurrentState(EntityAnimation.Idle);
          break;
      }
    }


    private void Update()
    {
      // Check if current animation has finished
      if (currentAnimation != EntityAnimation.Die && currentAnimation != EntityAnimation.PrepareToAttack && currentAnimation != EntityAnimation.Idle && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
      {
        // Change state based on current state
        switch (currentAnimation)
        {
          case EntityAnimation.PrepareToAttack:
            ChangeCurrentState(EntityAnimation.AttackIdle);
            break;
          case EntityAnimation.SpellCast:
            ChangeCurrentState(EntityAnimation.AttackIdle);
            break;
          case EntityAnimation.PassiveCast:
            ChangeCurrentState(EntityAnimation.AttackIdle);
            break;
            // Add more transitions here as needed
        }
      }
    }
  }

  // public enum PlayerAnimation
  // {
  //   Idle,
  //   Death
  // }
}
