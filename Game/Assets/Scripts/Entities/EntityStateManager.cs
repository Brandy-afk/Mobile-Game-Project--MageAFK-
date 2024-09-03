using Sirenix.OdinInspector;
using UnityEngine;


namespace MageAFK.AI
{
  public abstract class EntityStateManager : MonoBehaviour
  {

    protected Animator animator;
    [SerializeField, ReadOnly] protected EntityAnimation currentAnimation;

    protected virtual void Awake()
    {
      animator = GetComponent<Animator>();
    }

    public virtual void ChangeCurrentState(EntityAnimation animation, bool force = false)
    {
      // Don't change state if it's the same as the current one
      if (!force && (currentAnimation == animation || currentAnimation == EntityAnimation.Die)) return;
      
      currentAnimation = animation;

      // Apply animation
      animator.Play(animation.ToString());
    }

    public void StopAnimation() => animator.StopPlayback();

    public EntityAnimation ReturnCurrentAnimation() => currentAnimation;
  }

  public enum EntityAnimation
  {
    //Basic Animations
    Idle,
    Die,
    Run,
    Attack,

    //Player
    PrepareToAttack,
    AttackIdle,
    SpellCast,
    PassiveCast,
    BluntAttack,
    Unprepared,

    //Terra Guardian
    Attack2,
    Fury
  }
}
