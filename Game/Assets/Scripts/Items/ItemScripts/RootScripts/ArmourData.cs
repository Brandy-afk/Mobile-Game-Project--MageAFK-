using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace MageAFK.Items
{
  public abstract class ArmourData : ItemData
  {
    [SerializeField, BoxGroup("Armour")] private bool wrapInfoText;
    public bool WrapText => wrapInfoText;
    protected override void OnValidate()
    {
      base.OnValidate();
      if (types == null || types.Count <= 0)
      {
        types = new List<ItemType>
        {
          ItemType.Upgradable,
          ItemType.Equippable
        };
      }
    }

    public override ItemStructureTypes ReturnStructure() => ItemStructureTypes.Armour;
    public abstract string ReturnArmourInfo(ItemLevel level, string highlightHex);
  }


  public abstract class Armour : Item
  {
    public ItemLevel level;

    public Armour(ArmourData data, ItemLevel level) : base(data) => this.level = level;
    public abstract void ToggleGear(bool state);
    public override ItemLevel ReturnLevel() => level;
  }


}