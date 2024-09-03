using System.Collections.Generic;
using MageAFK.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MageAFK.AI
{
    [CreateAssetMenu(fileName = "EnemyReferences", menuName = "EnemyReferences", order = 51)]
  public class EntityReferences : SerializedScriptableObject
  {

    [SerializeField] private Dictionary<EnemyType, Sprite> typeDict;


    public Sprite ReturnEnemyTypeSprite(EnemyType type)
    {
      if (!typeDict.ContainsKey(type)) { return null; }
      return typeDict[type];
    }
  }




}