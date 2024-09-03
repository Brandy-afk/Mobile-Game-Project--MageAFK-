using MageAFK.Management;
using System.Collections.Generic;
using MageAFK.Core;
using UnityEngine;
using MageAFK.Stats;
using System;
using MageAFK.Player;


namespace MageAFK.AI
{

  public class EntityDataManager : MonoBehaviour
  {

    [Header("Percentage (decimal form), Percentage of base value to be lowest")]
    [SerializeField] private float minEnemySpawnCap = 0.5f;
    [SerializeField] private float minValueCap = 0.05f;



    private readonly List<NPEntity> activeNPEntities = new List<NPEntity>();
    private event Action<int> ActiveNPEntityCountEvent;


    private void Awake() => ServiceLocator.RegisterService(this);
    private void Start()
    {
      ServiceLocator.Get<IPlayerDeath>().SubscribeToPlayerDeath(OnSiegeOver, true);
      WaveHandler.SubToSiegeEvent((Status status) =>
     {
       if (status == Status.End_CleanUp)
       {
         ClearAllActiveEnemies();
       }
     }, true);
    }

    public bool ReturnIfNPEntities() => activeNPEntities.Count > 0;
    public List<NPEntity> ReturnActiveNPEntities() => activeNPEntities;


    public void SubToEntityCount(Action<int> handler, bool state)
    {
      if (state)
      {
        ActiveNPEntityCountEvent += handler;
        ActiveNPEntityCountEvent?.Invoke(activeNPEntities.Count);
      }
      else
      {
        ActiveNPEntityCountEvent = null;
      }
    }
    public void RegisterEntity(NPEntity entity)
    {
      activeNPEntities.Add(entity);
      ActiveNPEntityCountEvent?.Invoke(activeNPEntities.Count);
    }

    public void UnRegisterEntity(NPEntity entity)
    {
      activeNPEntities.Remove(entity);
      ActiveNPEntityCountEvent?.Invoke(activeNPEntities.Count);
    }

    public void ScaleAI()
    {
      float multiplier = ServiceLocator.Get<ScalingHandler>().ReturnEnemyScalerMultiplier();
      var data = ServiceLocator.Get<LocationHandler>().ReturnEntityList();
      foreach (var iD in data)
      {
        AIData d = ServiceLocator.Get<EntityHandler>().GetMob(iD).data;
        d.ScaleStats(multiplier, minEnemySpawnCap);
        d.RecalculateStats(minValueCap);
      }
    }

    public void UpdateNPEntityStats(Stat stat)
    {
      EntityIdentification[] data = ServiceLocator.Get<LocationHandler>().ReturnEnemyList();
      foreach (var iD in data)
      {
        Mob mob = ServiceLocator.Get<EntityHandler>().GetMob(iD);
        mob.data.RecalculateStat(stat, minValueCap);
      }

      UpdateActiveNPEntityStats(stat);
    }

    private void UpdateActiveNPEntityStats(Stat stat)
    {
      if (activeNPEntities != null && activeNPEntities.Count > 0)
      {
        foreach (var entity in activeNPEntities)
        {
          entity.RecalculateStat(stat);
        }
      }
    }

    private void OnSiegeOver()
    {
      if (activeNPEntities != null)
      {
        foreach (var entity in activeNPEntities)
        {
          entity.SetState(States.siegeOver, true);
        }
      }

      var data = ServiceLocator.Get<LocationHandler>().ReturnEntityList();
      foreach (var id in data)
      {
        AIData d = ServiceLocator.Get<EntityHandler>().GetMob(id).data;
        d.ResetStats();
      }
    }

    private void ClearAllActiveEnemies()
    {
      if (activeNPEntities.Count <= 0) { return; }

      List<NPEntity> CopyOfActiveEnemies = new(activeNPEntities);
      foreach (var entity in CopyOfActiveEnemies)
      {
        entity.Die(true);
      }

      activeNPEntities.Clear();
    }

  }




  [System.Serializable]
  public class EnemyDataHolder
  {
    public List<EnemyData> enemyDataList;
    public EnemyDataHolder(List<EnemyData> list)
    {
      enemyDataList = list;
    }
  }

}


