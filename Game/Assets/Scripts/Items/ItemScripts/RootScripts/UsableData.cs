using UnityEngine;



namespace MageAFK.Items
{
    [CreateAssetMenu(fileName = "New Usable", menuName = "Items/ItemData/Usable")]
  public class UsableData : ItemData
  {
    
    
    public override ItemStructureTypes ReturnStructure() => ItemStructureTypes.Usable;

    public string desc;
  }

  public class Usable : Item, IUseHandler
  {

    public ItemStatTrait[] traits { get; }

    public Usable(UsableData data) : base(data)
    {

    }

    public virtual void OnUse()
    {

    }

  }
}
