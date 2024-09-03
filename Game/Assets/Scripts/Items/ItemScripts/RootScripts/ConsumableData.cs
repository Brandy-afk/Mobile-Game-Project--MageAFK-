using Sirenix.OdinInspector;
using UnityEngine;



namespace MageAFK.Items
{
    [CreateAssetMenu(fileName = "New Consumable", menuName = "Items/ItemData/Consumable")]
  public class ConsumableData : ItemData
  {


    [BoxGroup("Consumable")]
    public ItemStatTrait[] traits;


    [BoxGroup("Consumable"), Header("In Minutes")]
    public float duration;

    public virtual string ReturnCustomDesc()
    {
      return null;
    }

    public override ItemStructureTypes ReturnStructure() => ItemStructureTypes.Consumable;

  }

  public class Consumable : Item, IUseHandler
  {
    public float duration { get; }
    public ItemStatTrait[] traits { get; }

    public Consumable(ConsumableData data) : base(data)
    {
      duration = data.duration;
      // traits = data.traits.Select(trait => (ItemStatTrait)trait.Clone()).ToArray();
    }

    public void OnUse()
    {
      // ConsumableHandler.Instance.AddConsumableTask(this);
    }


    public ItemStatTrait[] ReturnTraits()
    {
      return traits;
    }

  }


}
