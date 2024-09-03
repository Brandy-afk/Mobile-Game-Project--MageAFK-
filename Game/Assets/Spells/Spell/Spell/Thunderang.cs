
using System.Collections.Generic;
using System.Linq;
using MageAFK.AI;
using MageAFK.Combat;
using MageAFK.Core;
using MageAFK.Management;
using MageAFK.Player;
using MageAFK.Stats;
using MageAFK.Tools;
using UnityEngine;

namespace MageAFK.Spells
{

  [CreateAssetMenu(fileName = "Thunderang", menuName = "Spells/Thunderang")]
  public class Thunderang : Spell
  {

    public override void Activate()
    {
      GameObject instance = SpellSpawn(iD, PlayerController.Positions.SpellSpawn);
      var ePos = ServiceLocator.Get<EntityTracker>().ReturnBestTarget(Focus);
      SetUpInstance(instance, ePos);
    }


    public bool SetNewTarget(Transform hit, GameObject instance, HashSet<Transform> set, int count, Tags[] tagTargets)
    {
      var state = count < ReturnStatValue(Stat.MaxTargets);

      if (state)
      {
        var ePos = ReturnClosestTarget(hit, set, tagTargets);
        if (ePos == null) return false;
        SetUpInstance(instance, ePos);
        return true;
      }

      return false;
    }

    private void SetUpInstance(GameObject instance, IEntityPosition target)
    {
      Utility.FlipXSprite(instance.transform.position, target.Body, instance.transform);
      Utility.SetVelocity(instance, target.Body, ReturnStatValue(Stat.SpellSpeed), ReturnStatValue(Stat.AimVariance));
    }

    private IEntityPosition ReturnClosestTarget(Transform point, HashSet<Transform> set, Tags[] tags)
    {
      Collider2D[] cols = Physics2D.OverlapCircleAll(point.position, ReturnStatValue(Stat.Range) /* RADIUS */, GameResources.GetLayerMask(ProjectLayerMask.NonPlayerBodies).mask);
      var potentialTargets = cols.Where(col => !set.Contains(col.transform) && Utility.VerifyTags(tags, col))
                                 .Select(col => col.GetComponent<INonPlayerPosition>())
                                 .ToArray();

      return ServiceLocator.Get<EntityTracker>().GetClosestTarget(potentialTargets, point);
    }




  }
}
