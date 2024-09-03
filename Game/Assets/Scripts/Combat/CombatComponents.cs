using MageAFK.AI;
using MageAFK.Core;
using MageAFK.Spells;

namespace MageAFK.Combat
{

  public interface IDynamicAction
  {

  }

  #region Collisions
  public interface ISpellCollisionEvent : IDynamicAction, IPriority
  {
    void HandleCollision(Spell spell, NPEntity entity, ref float damage, ref float mod, bool isCrit, bool isPierce, bool isEffect);
  }

  public interface IPlayerCollisionEvent : IDynamicAction, IPriority
  {
    void HandleCollision(NPEntity entity, ref float damage, ref float mod);
  }

  public interface IPriority
  {
    Priority ReturnPriority();
  }

  #endregion

  #region Wave Actions

  public interface IWaveAction : IDynamicAction
  {

  }

  public interface IWaveStartAction : IWaveAction
  {
    public void OnWaveStart();
  }

  public interface IWaveEndAction : IWaveAction
  {
    public void OnWaveEnd();
  }

  public interface IWaveUpdateAction : IWaveAction
  {
    public float ReturnTimeBetweenUpdates();
    public void UpdateAction();
  }

  public interface IWaveCompleteAction : IWaveStartAction, IWaveEndAction, IWaveUpdateAction
  {

  }


  public enum OrginType
  {
    Spell,
    Enemy,
    Item,
    Effect,
    Other

  }

  public enum Tags
  {
    Canvas,
    Body,
    Feet,
    Enemy,
    Trap,
    Projectile,
    CamBounds,
    Shield
  }

  #endregion
}