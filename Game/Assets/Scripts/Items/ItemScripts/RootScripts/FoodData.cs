
using System;
using Sirenix.OdinInspector;
using UnityEngine;



namespace MageAFK.Items
{
  [CreateAssetMenu(fileName = "New Food", menuName = "Items/ItemData/Food")]
  public class FoodData : ItemData
  {


    [Title("[Common Food = .1 <-> .2]")]
    [Title("[Unique Food = .2 <-> .4]")]
    [Title("[Rare Food = .4 <-> .5]")]
    [Title("[Artifact Food = .5 <-> .6]")]
    [Title("[Corrupt Food = .6 <-> .9]")]

    [BoxGroup("Food"), Header("Percentage of max health to heal on use (.5 = 5%)(DECIMAL)")]
    public float healValue;

    
    public override ItemStructureTypes ReturnStructure() => ItemStructureTypes.Food;


  }

  public class Food : Item, IUseHandler
  {

    public Food(FoodData data) : base(data)
    {

    }

    public void OnUse()
    {
      throw new NotImplementedException();
    }
  }


}
