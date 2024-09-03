
using System.Collections.Generic;
using MageAFK.Core;
using UnityEngine;
using MageAFK.Management;
using Sirenix.OdinInspector;
using MageAFK.Stats;
using System;
using MageAFK.Pooling;
using System.Linq;


namespace MageAFK.AI
{

    public class EntityHandler : SerializedMonoBehaviour
  {

    [SerializeField] private Dictionary<EntityIdentification, Mob> allEntities;
    [SerializeField] private Dictionary<Stat, float> defaultEnemyStats;


    private void Awake() => ServiceLocator.RegisterService(this);

    public Mob GetMob(EntityIdentification iD)
    {
      if (allEntities.ContainsKey(iD))
      {
        return allEntities[iD];
      }
      Debug.Log($"Bad Key {iD}");
      return null;
    }


    public static FocusEntity ReturnFocusEntity(EntityIdentification ID)
    {
      Mob mobToReturn = EnemyPooler.currentMobs.FirstOrDefault(mob => mob.iD == ID);

      if (mobToReturn == null) { return FocusEntity.ClosestTarget; }
      try
      {
        return (FocusEntity)Array.IndexOf(EnemyPooler.currentMobs, mobToReturn);
      }
      catch (MissingReferenceException)
      {
        Debug.Log($"ID : {ID} - Enemy not found in current mobs");
        return FocusEntity.ClosestTarget;
      }
    }


    public static Mob ReturnMobWithFocus(FocusEntity entity)
    {
      try
      {
        return EnemyPooler.currentMobs[(int)entity];
      }
      catch (IndexOutOfRangeException)
      {
        Debug.Log($"{entity} is not a focus enemy.");
        return null;
      }
    }

    public void AddEntity(Mob mob)
    {
      if (allEntities.ContainsKey(mob.iD))
      {
        Debug.Log($"Already in dict {mob.iD}");
        return;
      }

      allEntities.Add(mob.iD, mob);
    }

    #region Interface / Testing

    [Button("Set Default Stats")]
    private void LoadData()
    {
      foreach (var pair in allEntities)
      {
        pair.Value.data.iD = pair.Key;
        if (pair.Value.entity != Entities.Enemy)
        {
          continue;
        }

        Dictionary<Stat, float> baseStats = pair.Value.data.GetStats(AIDataType.Base);

        if (baseStats[Stat.Health] != 0)
        {
          continue;
        }

        foreach (var value in defaultEnemyStats)
        {
          baseStats[value.Key] = defaultEnemyStats[value.Key];
        }
      }

    }

    [Button("reset stats")]
    public void ResetData()
    {
      foreach (var pair in allEntities)
      {
        try
        {
          pair.Value.data.ResetStats();
        }
        catch (MissingReferenceException)
        {
          Debug.Log($"Missing data for {pair.Key}");
          continue;
        }
        catch (NullReferenceException)
        {
          Debug.Log($"Missing data for {pair.Key}");
          continue;
        }
      }
    }

    #endregion
  }

  [System.Serializable]

  public class Mob
  {
    public EntityIdentification iD;
    public string name;
    [TextArea(3, 5)]
    public string desc;
    public Sprite headShotImage;
    public Entities entity;
    public GameObject prefab;
    public AIData data;
    public RuntimeAnimatorController controller;

    [ShowIf("ReturnIfEntityIsEnemy")]
    public MobGrade grade;
    [ShowIf("ReturnIfEntityIsEnemy")]
    public EnemyType type;
    [ShowIf("ReturnIfEntityIsEnemy")]
    public EnemyRace race;



    [ShowIf("ReturnIfEntityIsEnemy"), ShowIf("ReturnIfNotMelee"), ReadOnly]
    public GameObject projectile;


    private bool ReturnIfEntityIsEnemy()
    {
      if (entity == Entities.Enemy)
      {
        return true;
      }
      grade = MobGrade.All;
      race = EnemyRace.None;
      return false;
    }

    public bool ReturnIfNotMelee()
    {
      return (type != EnemyType.Melee);
    }

    public Mob(Mob mob)
    {
      iD = mob.iD;
      desc = mob.desc;
      entity = mob.entity;
      prefab = mob.prefab;
      controller = mob.controller;
      data = mob.data;
      grade = mob.grade;
      type = mob.type;
      race = mob.race;
      projectile = mob.projectile;
    }

    public Mob()
    {

    }

  }

}
