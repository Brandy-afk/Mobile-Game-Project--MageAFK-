using System;

namespace MageAFK.Items
{
  public static class ItemFactory
  {
    public static Item CreateItem(ItemData data, ItemLevel level = ItemLevel.Level0)
    {
      switch (data)
      {
        case ArmourData armourData:
          return CreateArmourItem(armourData, level);

        case ConsumableData consumableData:
          return new Consumable(consumableData);

        case UsableData usableData:
          return new Usable(usableData);

        case FoodData foodData:
          return new Food(foodData);

        case ItemData itemData:
          return new Item(itemData);

        // Repeat for other subclasses of ItemData...

        default:
          throw new ArgumentException($"Unsupported type of ItemData: {data.GetType()}");
      }
    }

    private static Item CreateArmourItem(ArmourData data, ItemLevel level)
    {
      switch (data)
      {
        case StatArmourData statData:
          return new StatArmour(statData, level);

        case BloodPendantData bloodPendant:
          return new BloodPendant(bloodPendant, level);

        case CrownData crownData:
          return new Crown(crownData, level);

        case FoolsCapData foolsCapData:
          return new FoolsCap(foolsCapData, level);

        case StormBringerData stormBringerData:
          return new StormBringer(stormBringerData, level);

        default:
          return null;
      }


    }

  }
}