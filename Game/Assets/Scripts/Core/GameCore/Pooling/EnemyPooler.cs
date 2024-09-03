using System.Collections.Generic;
using UnityEngine;
using MageAFK.AI;
using MageAFK.Core;
using MageAFK.Management;
using MageAFK.Tools;

namespace MageAFK.Pooling
{
  /// <summary>
  /// The EnemyPooler class is responsible for managing enemies in the game. 
  /// It creates a pool of enemies for efficient instantiation, enabling and disabling.
  /// It also supports pooling enemies of different grades and from different locations. 
  /// </summary>
  public class EnemyPooler : AbstractPooler
  {
    [Header("Objects")]
    [SerializeField] private Transform enemyParent;
    [SerializeField] private Transform projectileParent;

    private readonly Dictionary<EntityIdentification, List<GameObject>> currentEnemyPool
      = new Dictionary<EntityIdentification, List<GameObject>>();
    private readonly Dictionary<EntityIdentification, List<GameObject>> currentProjectilePool
      = new Dictionary<EntityIdentification, List<GameObject>>();
    public readonly static Mob[] currentMobs = new Mob[3];

    
    protected override void RegisterSelf() => ServiceLocator.RegisterService(this);
    protected override void OnWaveStateChanged(WaveState state)
    {
      if (state == WaveState.Counter)
        Create();
      else if (state == WaveState.Wave)
        Pool();
      else if (state == WaveState.Intermission)
        Clear();
    }

    /// <summary>
    /// Load mobs from data
    /// </summary>
    /// <param name="entities"></param>
    public void InitializeData(List<EntityIdentification> entities)
    {
      var entityHandler = ServiceLocator.Get<EntityHandler>();
      for (int i = 0; i < 3; i++)
        currentMobs[i] = entityHandler.GetMob(entities[i]);
    }

    /// <summary>
    /// Create enemy pool if the current pool is empty.
    /// </summary>
    /// <returns>List of randomly selected enemies for each grade.</returns>
    protected override void Create()
    {
      if (currentEnemyPool.Count > 0) { Debug.Log("Clearing Pool..."); Clear(); }
      var currentMobList = ServiceLocator.Get<LocationHandler>().ReturnEnemyList();
      // Shuffle the mob list
      Utility.ShuffleCollection(currentMobList);

      var entityHandler = ServiceLocator.Get<EntityHandler>();
      for (int i = 0; i < 3; i++)
      {
        for (int j = i; j < currentMobList.Length; j++)
        {
          Mob mob = entityHandler.GetMob(currentMobList[j]);
          if (mob.grade != MobGrade.Boss || mob.grade != MobGrade.All)
          {
            currentMobs[i] = mob;
            break;
          }
        }
      }
    }


    /// <summary>
    /// Populate the enemy pool based on the selected enemies.
    /// </summary>
    protected override void Pool()
    {
      // Loop through each spell
      foreach (Mob mob in currentMobs)
      {
        // If the spell is not already in the dictionary, add it
        if (!currentEnemyPool.ContainsKey(mob.data.iD))
          currentEnemyPool[mob.data.iD] = new List<GameObject>();


        while (currentEnemyPool[mob.data.iD].Count < 3)
        {
          GameObject item = Instantiate(mob.prefab, enemyParent.transform);
          item.SetActive(false);
          currentEnemyPool[mob.data.iD].Add(item);
        }

        if (mob.projectile != null)
        {
          if (!currentProjectilePool.ContainsKey(mob.data.iD))
            currentProjectilePool[mob.data.iD] = new List<GameObject>();

          while (currentProjectilePool[mob.data.iD].Count < 3)
          {
            GameObject item = Instantiate(mob.projectile, projectileParent.transform);
            item.SetActive(false);
            currentProjectilePool[mob.data.iD].Add(item);
          }
        }
      }
    }

    /// <summary>
    /// Get an enemy from the pool based on the enemy ID.
    /// </summary>
    /// <param name="enemyID">The ID of the enemy to retrieve from the pool.</param>
    /// <returns>A GameObject representing the enemy if one is available; otherwise, a new enemy is created.</returns>
    public GameObject Get(EntityIdentification enemyID)
    {
      // If this spell is not in the pool, return null
      if (currentEnemyPool == null || !currentEnemyPool.ContainsKey(enemyID)) return null;

      // Find the first inactive object in the pool
      foreach (var obj in currentEnemyPool[enemyID])
      {
        if (obj != null && !obj.activeInHierarchy)
        {
          obj.SetActive(true);
          return obj;
        }
        else if (obj == null)
        {
          return null;
        }
      }

      // If no inactive object is available, you can choose to either return null or create a new object and add it to the pool
      Mob mobToCreate = null;
      foreach (var mob in currentMobs)
      {
        if (mob.data.iD == enemyID)
        {
          mobToCreate = mob;
        }
      }
      if (mobToCreate == null) { return null; }

      GameObject newItem = Instantiate(mobToCreate.prefab, enemyParent.transform);
      newItem.SetActive(false);
      currentEnemyPool[enemyID].Add(newItem);
      return newItem;
    }

    /// <summary>
    /// Get an projectile from the pool based on the enemy ID.
    /// </summary>
    /// <param name="enemyID">The ID of the enemy to retrieve from the pool.</param>
    /// <returns>A GameObject representing the projectile if one is available; otherwise, a new enemy is created.</returns>
    public GameObject GetProjectile(EntityIdentification enemyID)
    {
      // If this spell is not in the pool, return null
      if (!currentProjectilePool.ContainsKey(enemyID)) return null;

      // Find the first inactive object in the pool
      foreach (var obj in currentProjectilePool[enemyID])
      {
        if (!obj.activeInHierarchy)
        {
          obj.SetActive(true);
          return obj;
        }
      }

      // If no inactive object is available, you can choose to either return null or create a new object and add it to the pool
      Mob mobToCreate = null;
      foreach (var mob in currentMobs)
      {
        if (mob.data.iD == enemyID)
        {
          mobToCreate = mob;
        }
      }
      if (mobToCreate == null) { return null; }

      GameObject newItem = Instantiate(mobToCreate.projectile, projectileParent.transform);
      newItem.SetActive(true);
      currentProjectilePool[enemyID].Add(newItem);
      return newItem;
    }




    /// <summary>
    /// Cleans up the current enemy pool by destroying all GameObjects and clearing the collection.
    /// </summary>
    protected override void Clear()
    {
      if (currentEnemyPool == null || currentEnemyPool.Count <= 0) return;

      foreach (var enemy in currentEnemyPool)
      {
        foreach (var prefab in enemy.Value)
        {
          Destroy(prefab);
        }
      }

      if (currentProjectilePool == null) return;

      foreach (var enemy in currentProjectilePool)
      {
        foreach (var projectile in enemy.Value)
        {
          Destroy(projectile);
        }
      }

      for (int i = 0; i < currentMobs.Length; i++)
      {
        currentMobs[i] = null;
      }

      currentEnemyPool.Clear();
      currentProjectilePool.Clear();
    }
  }

}