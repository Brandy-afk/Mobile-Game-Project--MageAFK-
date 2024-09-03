using MageAFK.Player;
using System.Collections.Generic;
using UnityEngine;
using MageAFK.Management;
using MageAFK.AI;
using MageAFK.Tools;
using System.Linq;

namespace MageAFK.Core
{
  public class EntityTracker
  {
    public readonly List<INonPlayerPosition> entities = new();
    private readonly Dictionary<FocusEntity, List<INonPlayerPosition>> map = new();
    public bool IsTarget => entities.Count > 0;

    public EntityTracker() => ServiceLocator.Get<IPlayerDeath>().SubscribeToPlayerDeath(RemoveAllEntities, true);

    #region Adding / Removing

    public void AddTrackableEntity(NPEntity entity)
    {
      var focus = EntityHandler.ReturnFocusEntity(entity.data.iD);

      if (!map.ContainsKey(focus))
        map[focus] = new List<INonPlayerPosition>();

      map[focus].Add(entity);
      entities.Add(entity);
    }


    public void RemoveEntity(NPEntity entity)
    {
      if (entities == null) { return; }

      bool isRemoved = entities.Remove(entity);

      if (isRemoved)
        map[EntityHandler.ReturnFocusEntity(entity.data.iD)].Remove(entity);
    }


    private void RemoveAllEntities()
    {
      if (entities.Count > 0)
      {
        entities.Clear();
        map.Clear();
      }
    }

    #endregion



    #region  Target Calls

    //For Player calls / SPells
    public INonPlayerPosition ReturnBestTarget(FocusEntity entity, Transform point = null)
    {
      if (point != null)
        return GetClosestTarget(entities, point);

      if (entity == FocusEntity.Random)
        return GetRandomTarget(entities);


      if (map.ContainsKey(entity))
      {
        if (map[entity].Count > 0)
          return GetClosestTarget(map[entity], PlayerController.Positions.Transform);
        else
          return GetClosestTarget(entities, PlayerController.Positions.Transform);
      }
      else
        return GetClosestTarget(entities, PlayerController.Positions.Transform);
    }

    private INonPlayerPosition GetRandomTarget(IList<INonPlayerPosition> targets)
    {
      if (targets is null || targets.Count == 0)
        return null;

      System.Random rnd = new();

      int randomIndex = rnd.Next(targets.Count);
      return targets[randomIndex];
    }

    public INonPlayerPosition[] GetMultipleRandomTargets(int amount)
    {
      if (entities is null || entities.Count == 0)
        return null;

      var newList = new List<INonPlayerPosition>(entities);

      Utility.ShuffleCollection(new List<INonPlayerPosition>(entities));

      return newList.Take(amount).ToArray();
    }

    public INonPlayerPosition GetClosestTarget(IList<INonPlayerPosition> targets, Transform point, float range = Mathf.Infinity)
    {
      if (targets is null) return null;

      INonPlayerPosition bestTarget = null;
      foreach (var potentialTarget in targets)
      {
        if (potentialTarget != null && potentialTarget.Transform != point)
        {
          if (ReturnIfInRange(point.position, potentialTarget.Pivot, range, out float dSqrToTarget))
          {
            range = dSqrToTarget;
            bestTarget = potentialTarget;
          }
        }
      }

      return bestTarget;
    }

    public INonPlayerPosition GetFurthestTarget(IList<INonPlayerPosition> targets)
    {
      if (targets is null) return null;

      float range = 0;
      INonPlayerPosition bestTarget = null;
      Vector3 currentPosition = PlayerController.Positions.Pivot;
      foreach (INonPlayerPosition potentialTarget in targets)
      {
        if (potentialTarget != null)
        {
          if (!ReturnIfInRange(currentPosition, potentialTarget.Pivot, range, out float dSqrToTarget))
          {
            range = dSqrToTarget;
            bestTarget = potentialTarget;
          }
        }
      }

      return bestTarget;
    }


    private bool ReturnIfInRange(Vector2 point, Vector2 target, float range, out float dSqrToTarget)
    {
      Vector2 directionToTarget = target - point;
      dSqrToTarget = directionToTarget.sqrMagnitude;
      return dSqrToTarget < range;
    }

    public INonPlayerPosition ReturnRandomEnemyInRange(float range, Transform point)
    {
      INonPlayerPosition[] l = entities.Where(entity => ReturnIfInRange(point.position, entity.Pivot, range, out _)).ToArray();
      return l.Length > 0 ? l[UnityEngine.Random.Range(0, l.Length - 1)] : null;
    }

    #endregion

  }

}
