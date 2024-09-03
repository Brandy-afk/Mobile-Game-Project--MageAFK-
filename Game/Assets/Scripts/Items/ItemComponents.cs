using System;
using MageAFK.Stats;
using UnityEngine;
using MageAFK.Tools;
using MageAFK.Management;
using Sirenix.OdinInspector;

namespace MageAFK.Items
{

  public interface ICustomItemObject
  {
    public void InputStats(ItemData data);
  }

  public interface IPrefabReturner
  {
    public GameObject ReturnPrefab();
  }


  [Serializable]
  public struct CraftingValues
  {
    public int cost;
    [Tooltip("In Seconds")]
    public int time;
  }



  [Serializable]
  public class ItemStatTrait
  {
    public Stat stat;

    [InfoBox("Negative Modification need to be negative. Only whole values (not decimal ex -> 0.25)")]
    public float baseAmount;

    [Header("For Enemy")]
    public bool isForEnemy;

    public bool isPercentage = true;


    [HideInInspector]
    public float runtimeAmount;

    public ItemStatTrait() { }


    public string PrintTrait()
    {
      StatInfo info = ServiceLocator.Get<StatInformation>().ReturnStatInformation(stat);
      if (isForEnemy) { return "Enemy " + info.statName; }
      return info.statName;
    }

    public string PrintValue(ItemLevel level = ItemLevel.None)
    {
      var value = level == ItemLevel.None ? runtimeAmount : ReturnUpgradedValue(level);
      return StringManipulation.FormatStatNumberValue(value, isPercentage, value < 1 ? "N2" : "N1");
    }

    public float ReturnUpgradedValue(ItemLevel level)
    {
      float placeholder = baseAmount + baseAmount * ServiceLocator.Get<GradeStatHandler>().ReturnModForUpgrade(Mathf.Clamp((int)level, 0, 3));
      if (!ServiceLocator.Get<StatInformation>().ReturnStatInformation(stat).percentageModification)
        return Mathf.Ceil(placeholder);

      return placeholder;
    }
  }


  [Serializable]
  public class Ingredient
  {
    public ItemData item;
    public ItemLevel level = ItemLevel.None;
    public int quantity;
  }




  public interface IUseHandler
  {
    public void OnUse();
  }


}



