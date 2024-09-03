
using System.Linq;
using MageAFK.AI;
using MageAFK.Combat;
using MageAFK.Stats;
using MageAFK.Tools;
using Sirenix.OdinInspector;
using UnityEngine;
namespace MageAFK.Spells
{

  public class StoneSpireProjectile : SingleTargetController
  {

    [ReadOnly] public int procs = 0;

    protected override CollisionInformation HandleDamage(NPEntity entity, bool forceCrit = false, bool forceStatus = false, bool forcePierce = false, float baseDamage = .01f)
    {
      CollisionInformation information = SpellCollisionHandler.ReturnCollisionInformation(spell, entity, forceCrit, forceStatus, forcePierce, baseDamage);

      bool isDead = entity.DoDamage(information.damage, spell, information.textType);

      if (isDead && procs > 0)
      {
        foreach (var trans in ReturnTargets(entity))
        {
          (spell as StoneSpire).SpawnSpire(trans, procs, transform);
        }
      }

      return default;
    }

    private INonPlayerPosition[] ReturnTargets(NPEntity nPEntity)
    {
      //Get nearby targets
      Collider2D[] colliders = Physics2D.OverlapCircleAll(nPEntity.transform.position, spell.ReturnStatValue(Stat.Range), ReturnMask(LayerCollision.Feet));

      //ShuffleCollection
      Utility.ShuffleCollection(colliders);

      //Convert and take certain number of targets
      return colliders.Where(col => Utility.VerifyTags(targetTags, col))
                      .Select(col => col.GetComponentInParent<INonPlayerPosition>())
                      .Take((int)spell.ReturnStatValue(Stat.TargetsPerTrigger))
                      .ToArray();
    }
  }

}
