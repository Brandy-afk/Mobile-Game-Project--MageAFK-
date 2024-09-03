using UnityEngine;
using Sirenix.OdinInspector;
using MageAFK.Tools;
using MageAFK.Stats;

namespace MageAFK.Items
{
  [CreateAssetMenu(fileName = "New Stat Armour", menuName = "Items/ItemData/Equippable/StatArmourData")]
  public class StatArmourData : ArmourData
  {

    public override string ReturnArmourInfo(ItemLevel level, string boldHex) => StringManipulation.CreateTraitString(traits, boldHex, level);

    [Space(5)]
    [BoxGroup("Armour")] public ItemStatTrait[] traits;

  }

  public class StatArmour : Armour
  {
    private readonly (Stat, float, bool)[] traits;
    public StatArmour(StatArmourData data, ItemLevel level) : base(data, level)
    {
      traits = new (Stat, float, bool)[data.traits.Length];
      for (int i = 0; i < data.traits.Length; i++)
      {
        float value = data.traits[i].ReturnUpgradedValue(level);
        value = data.traits[i].isPercentage ? value / 100 : value;
        traits[i] = (data.traits[i].stat, value, data.traits[i].isForEnemy);
      }
    }
    public override void ToggleGear(bool state) => StatHandlerBase.AlterEntityStats(traits, state);
  }


}