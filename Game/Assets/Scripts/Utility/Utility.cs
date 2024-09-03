using System;
using System.Collections.Generic;
using System.Text;
using MageAFK.Combat;
using MageAFK.Core;
using UnityEditor;
using UnityEngine;

namespace MageAFK.Tools
{
  public static class Utility
  {
    private static Camera _mainCam;  // Backing field

    public static Camera MainCam
    {
      get
      {
        if (_mainCam == null)
        {
          _mainCam = Camera.main;
        }
        return _mainCam;
      }
      private set
      {
        _mainCam = value;
      }
    }

    public static List<T> LoadAllObjects<T>() where T : UnityEngine.Object
    {
      string[] guids = AssetDatabase.FindAssets($"t:{typeof(T)}"); // ex t:Skill
      List<T> l = new List<T>();

      foreach (string guid in guids)
      {
        string assetPath = AssetDatabase.GUIDToAssetPath(guid);
        var obj = AssetDatabase.LoadAssetAtPath<T>(assetPath);
        if (obj != null)
        {
          l.Add(obj);
          Debug.Log($"Loaded obj: {obj.name}");
        }
      }

      return l;
    }


    public static void ShuffleCollection<T>(IList<T> array)
    {
      int n = array.Count;
      System.Random rng = new();
      while (n > 1)
      {
        int k = rng.Next(n--);
        T temp = array[n];
        array[n] = array[k];
        array[k] = temp;
      }
    }


    public static bool RollChance(int value)
    {
      float roll = UnityEngine.Random.Range(0, 100);
      return value >= roll ? true : false;
    }
    public static bool RollChance(float value)
    {
      float roll = UnityEngine.Random.Range(0f, 100f);
      return value >= roll ? true : false;
    }

    public static int RollCumulativeChances(float[] chances, float maxRange = 100f, int iD = 0)
    {
      StringBuilder builder = new();
      for (int i = 0; i < chances.Length; i++)
        builder.Append(chances[i] + " ");

      Debug.Log($"Drops - ID{iD}- {builder}");

      float cumulativeChance = 0;
      float randomChance = UnityEngine.Random.Range(0f, maxRange);
      Debug.Log($"Drops - ID{iD} - {randomChance}");
      for (int i = 0; i < chances.Length; i++)
      {
        cumulativeChance += chances[i];

        if (randomChance <= cumulativeChance)
          return i;
      }

      return -1;
    }

    public static bool GetIsInRange(Vector2 pos, Vector2 tar, float distance, float variance = 0) => Vector2.Distance(pos, tar) <= distance + variance;

    /// <summary>
    /// Get Random Map Position. All variables must be 0-1
    /// </summary>
    /// <param name="minX"></param>
    /// <param name="maxX"></param>
    /// <param name="minY"></param>
    /// <param name="maxY"></param>
    /// <returns></returns>
    public static Vector2 GetRandomMapPosition(float minX = 0.05f, float maxX = 0.95f, float minY = 0.05f, float maxY = 0.7f)
    {
      if (MainCam == null)
      {
        MainCam = Camera.main;
      }

      Vector2 spawnPoint = new(UnityEngine.Random.Range(0.05f, 0.95f), UnityEngine.Random.Range(0.05f, .7f));

      return MainCam.ViewportToWorldPoint(spawnPoint);
    }

    public static void FlipXSprite(Vector2 main, Vector2 second, Transform sprite, bool? state = null)
    {
      state = state != null ? state : second.x < main.x;

      float xFactor = Mathf.Abs(sprite.transform.localScale.x);
      sprite.transform.localScale = new Vector3((bool)state ? -xFactor : xFactor,
                                                  sprite.transform.localScale.y,
                                                  sprite.transform.localScale.z);
    }
    public static void FlipXSpriteByDirection(Vector2 direction, Transform sprite, bool? state = null)
    {
      // If 'state' is not provided, determine it based on the direction vector
      state = state ?? (direction.x < 0);

      float xFactor = Mathf.Abs(sprite.transform.localScale.x);
      sprite.transform.localScale = new Vector3((bool)state ? -xFactor : xFactor,
                                                sprite.transform.localScale.y,
                                                sprite.transform.localScale.z);
    }

    public static void FlipYSprite(Vector3 main, Vector3 second, Transform sprite, bool? state = null)
    {
      state = state != null ? state : second.x < main.x;

      float yFactor = Mathf.Abs(sprite.transform.localScale.y);
      sprite.transform.localScale = new Vector3(sprite.transform.localScale.x,
                                                 (bool)state ? -yFactor : yFactor,
                                                  sprite.transform.localScale.z);
    }

    public static void SetVelocity(GameObject instance, Vector3 target, float speed, float variance = 0)
    {
      Vector2 direction = (target - instance.transform.position).normalized;
      if (variance > 0)
      {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle += UnityEngine.Random.Range(-variance, variance);

        // Convert the angle back to radians
        float angleRadians = angle * Mathf.Deg2Rad;

        // Create a new direction vector from the angle
        direction.x = Mathf.Cos(angleRadians);
        direction.y = Mathf.Sin(angleRadians);
      }

      SetVelocity(direction, instance, speed);
    }

    public static void SetVelocity(Vector2 direction, GameObject instance, float speed)
    {
      Rigidbody2D rb = instance.GetComponent<Rigidbody2D>();
      if (rb != null)
      {
        rb.velocity = direction * speed;
      }
    }

    public static Vector2 SetRotation(GameObject instance, Vector3 target, float variance = 0)
    {
      if (target == null || instance == null) return Vector2.down;
      Vector2 direction = (target - instance.transform.position).normalized;
      float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

      if (variance > 0)
      {
        angle += UnityEngine.Random.Range(-variance, variance);
      }

      instance.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

      return instance.transform.right;
    }

    public static Vector2 RotateVector2(Vector2 v, float degrees)
    {
      float radians = degrees * Mathf.Deg2Rad;
      float sin = Mathf.Sin(radians);
      float cos = Mathf.Cos(radians);

      float tx = v.x;
      float ty = v.y;

      return new Vector2(cos * tx - sin * ty, sin * tx + cos * ty);
    }


    private const float topMargin = .15f;
    private const float sideMargin = .05f;
    public static bool IfWallReturnNewVector(Rigidbody2D rb, Transform trans, out Vector2 direction)
    {
      Vector3 viewportPos = MainCam.WorldToViewportPoint(trans.position);
      direction = Vector2.zero;

      if (viewportPos.x < sideMargin)
      {
        // Enemy is close to the left edge
        direction = RotateVector2(Vector2.right, UnityEngine.Random.Range(0, 30));
        return true;
      }
      else if (viewportPos.x > 1 - sideMargin)
      {
        // Enemy is close to the right edge
        direction = RotateVector2(Vector2.left, UnityEngine.Random.Range(-30, 0));
        return true;
      }
      if (viewportPos.y < sideMargin)
      {
        // Enemy is close to the bottom edge
        direction = RotateVector2(Vector2.up, UnityEngine.Random.value > 0.5f ? UnityEngine.Random.Range(-30, -15) : UnityEngine.Random.Range(15, 30));
        return true;
      }
      else if (viewportPos.y > 1 - topMargin)
      {
        // Enemy is close to the top edge
        rb.velocity = Vector2.zero;
        direction = RotateVector2(Vector2.down, UnityEngine.Random.Range(-30, 30));
        return true;
      }

      return false;
    }

    public static Color AlterColor(Color originalColor, bool lighter = true)
    {
      float factor = lighter ? .25f : -.25f;
      return new Color(
            Mathf.Clamp01(originalColor.r + factor),
            Mathf.Clamp01(originalColor.g + factor),
            Mathf.Clamp01(originalColor.b + factor),
            originalColor.a
        );
    }

    private static readonly Dictionary<Tags, string> cachedTags = new();
    public static bool VerifyTags(Tags[] targetTags, Component other)
    {
      for (int i = 0; i < targetTags.Length; i++)
      {
        while (true)
        {
          try
          {
            if (other.CompareTag(cachedTags[targetTags[i]]))
            {
              return true;
            }
            break;
          }
          catch (KeyNotFoundException)
          {
            cachedTags[targetTags[i]] = targetTags[i].ToString();
          }
        }
      }

      return false;
    }

    public static T InsertHandler<T>(T ev, T handler, Priority p) where T : Delegate
    {
      Delegate[] invocationList = ev?.GetInvocationList() ?? new Delegate[0];

      List<Delegate> handlers = new List<Delegate>(invocationList);

      int pos = 0;
      switch (p)
      {
        case Priority.Middle:
          pos = (int)Mathf.Ceil(handlers.Count / 2);
          break;
        case Priority.Last:
          pos = handlers.Count - 1;
          break;
      }

      if (pos >= 0 && pos <= handlers.Count)
        handlers.Insert(pos, handler);
      else
        handlers.Add(handler);


      return (T)Delegate.Combine(handlers.ToArray());
    }

  }
}
