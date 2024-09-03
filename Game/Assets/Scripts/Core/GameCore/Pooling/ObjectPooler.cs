using System.Collections.Generic;
using UnityEngine;
using MageAFK.Management;
using MageAFK.Core;

namespace MageAFK.Pooling
{
  public class ObjectPooler : MonoBehaviour
  {
    public List<Pool> pools;
    public static Dictionary<PoolingObjects, List<GameObject>> poolDictionary = new();

    private void Awake()
    {
      ServiceLocator.RegisterService(this);
      WaveHandler.SubToSiegeEvent(OnSiegeEvent, true);
    }

    private void OnSiegeEvent(Status state)
    {
      if (state == Status.Start)
        OnSiegeStart();
      else if (state == Status.End_CleanUp)
        ClearAll();
    }

    private void OnSiegeStart()
    {
      CreatePool(PoolingObjects.PlayerDamageText);
      CreatePool(PoolingObjects.DamageText);
      CreatePool(PoolingObjects.EnemyUI);
      CreatePool(PoolingObjects.ItemDrop);
      CreatePool(PoolingObjects.ValueText);
    }

    public void CreatePool(PoolingObjects tag)
    {
      foreach (Pool pool in pools)
      {
        if (pool.poolTag == tag)
        {
          List<GameObject> objectPool = new();

          for (int i = 0; i < 1; i++)
          {
            GameObject obj = Instantiate(pool.prefab, pool.parent);
            obj.SetActive(false);
            objectPool.Add(obj);
          }

          poolDictionary.Add(tag, objectPool);
          break;
        }
      }
    }

    public GameObject GetFromPool(PoolingObjects tag)
    {
      if (!poolDictionary.ContainsKey(tag))
      {
        Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
        return null;
      }

      foreach (GameObject objectInPool in poolDictionary[tag])
      {
        if (!objectInPool.activeInHierarchy)
        {
          return objectInPool;
        }
      }

      // If no inactive object is found, create a new one, add it to the pool and return it
      foreach (Pool pool in pools)
      {
        if (pool.poolTag == tag)
        {
          GameObject newObj = Instantiate(pool.prefab, pool.parent);
          newObj.SetActive(false);
          poolDictionary[tag].Add(newObj);
          return newObj;
        }
      }

      return null;
    }

    public void Clear(PoolingObjects tag)
    {
      if (poolDictionary.ContainsKey(tag))
      {
        foreach (var obj in poolDictionary[tag])
        {
          Destroy(obj);
        }

        poolDictionary[tag].Clear();
        poolDictionary.Remove(tag);
      }
      else
      {
        Debug.Log("Tag was not found in dict");
      }
    }

    private void ClearAll()
    {
      if (poolDictionary != null)
      {
        foreach (var pair in poolDictionary)
        {
          for (int i = 0; i < pair.Value.Count; i++)
          {
            Destroy(pair.Value[i]);
          }
        }
        poolDictionary.Clear();
      }
    }
  }

  [System.Serializable]
  public class Pool
  {
    public PoolingObjects poolTag;
    public GameObject prefab;
    public Transform parent;
  }

  public enum PoolingObjects
  {
    // Enum values go here. For example:
    DamageText,
    EnemyUI,
    ValueText,
    ItemDrop,
    UltimateIndicator,
    PlayerDamageText
    // Add more enum values as needed
  }

}