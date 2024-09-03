
using UnityEngine;
using Sirenix.OdinInspector;
using MageAFK.Management;
using MageAFK.Tools;
using System.Collections.Generic;

namespace MageAFK.Items
{

  [CreateAssetMenu(fileName = "ItemData", menuName = "Items/ItemData/Item(default)")]
  public class ItemData : ScriptableObject
  {
    [BoxGroup("Info")]
    public ItemIdentification iD;
    [BoxGroup("Info"), ReadOnly]
    public string itemName;
    [BoxGroup("Info")]
    public string description;
    [BoxGroup("Info"), PreviewField]
    public Sprite image;

    [BoxGroup("Info"), GUIColor("GetColorForField")]
    public ItemGrade grade;

    [BoxGroup("Type")]
    public ItemType mainType;

    [BoxGroup("Type")]
    public List<ItemType> types;

    [BoxGroup("Drop"), InfoBox("Leave empty if item does not drop, otherwise create a drop scriptable."), PreviewField]
    public DropInformation dropInfo = null;
    [BoxGroup("Drop")]

    [HideInInspector] public bool isUpgradable => types.Contains(ItemType.Upgradable);



    protected virtual void OnValidate()
    {
      itemName = StringManipulation.AddSpacesBeforeCapitals(iD.ToString());
    }

    public virtual ItemStructureTypes ReturnStructure() => ItemStructureTypes.Part;

    #region helpers
    public virtual bool ReturnRecipeState() => ServiceLocator.Get<RecipeHandler>().ReturnRecipeIfCraftable(iD);
    public bool ReturnIfType(ItemType type) => types.Contains(type) || mainType == type;
    public string ReturnTypeDesc() => $"{grade} {mainType}";

    //For fields
    private Color GetColorForField()
    {
      Dictionary<ItemGrade, Color> colors = new Dictionary<ItemGrade, Color>
      {
          {ItemGrade.None, Color.grey},
          {ItemGrade.Common, Color.white},
          {ItemGrade.Unique, Color.green},
          {ItemGrade.Rare, Color.cyan},
          {ItemGrade.Artifact, Color.yellow},
          {ItemGrade.Corrupt, Color.magenta}
      };

      return colors[grade];
    }

    #endregion

  }

  public class Item
  {
    public ItemIdentification iD { get; protected set; }
    public Item(ItemData data) => iD = data.iD;

    public virtual ItemLevel ReturnLevel() => ItemLevel.None;

    public (ItemIdentification, ItemLevel) Key => (iD, ReturnLevel());


    #region Equality Overloads

    public static bool operator ==(Item lhs, Item rhs)
    {
      if (ReferenceEquals(lhs, null))
      {
        return ReferenceEquals(rhs, null);
      }
      return lhs.Equals(rhs);
    }

    public static bool operator !=(Item lhs, Item rhs) => !(lhs == rhs);

    public override bool Equals(object other)
    {
      Item obj = other as Item;
      if (other == null) return false;
      return iD == obj.iD && ReturnLevel() == obj.ReturnLevel();
    }
    public override int GetHashCode() => iD.GetHashCode();

    #endregion

  }
}