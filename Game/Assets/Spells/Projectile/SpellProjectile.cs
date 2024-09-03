
using UnityEngine;
using MageAFK.AI;
using MageAFK.Combat;
using MageAFK.Stats;
using MageAFK.Tools;


namespace MageAFK.Spells
{

  //Base class
  public abstract class SpellProjectile : Projectile
  {

    public Spell spell;

    protected virtual CollisionInformation HandleDamage(NPEntity entity, bool forceCrit = false, bool forceStatus = false, bool forcePierce = false, float baseDamage = .01f)
    {
      //Create all collision information and package it
      if (entity == null)
      {
        Debug.Log($"Null entity! {gameObject.name}");
        return default;
      }

      CollisionInformation information = SpellCollisionHandler.ReturnCollisionInformation(spell, entity, forceCrit, forceStatus, forcePierce, baseDamage);

      if (information.isStatusProc && spell.effect != StatusType.None)
        CreateEffect(entity);

      entity.DoDamage(information.damage, spell, information.textType);

      return information;
    }

    protected override LayerMask ReturnMask(LayerCollision collision)
    {
      switch (collision)
      {
        case LayerCollision.Body:
          return GameResources.GetLayerMask(ProjectLayerMask.NonPlayerBodies).mask;
        case LayerCollision.Both:
          return GameResources.GetLayerMask(ProjectLayerMask.NonPlayerBodies).mask | GameResources.GetLayerMask(ProjectLayerMask.NonPlayerFeet).mask;
        case LayerCollision.Feet:
          return GameResources.GetLayerMask(ProjectLayerMask.NonPlayerFeet).mask;
        default:
          Debug.LogWarning("Error");
          return default;
      }
    }

    protected override void CreateEffect(Entity entity)
    {
      entity.GetComponent<StatusHandler>()?.CreateEffect(OrginType.Spell
                                             , spell.effect
                                             , Spell.ReturnEffectMagnitude(spell)
                                             , spell.ReturnStatValue(Stat.StatusDuration)
                                             , (int)spell.iD);
    }

  }

}
