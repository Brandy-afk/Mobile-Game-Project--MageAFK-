using System;
using System.Collections.Generic;
using UnityEngine;
using MageAFK.Core;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEditor;
using MageAFK.Tools;
using MageAFK.Management;

namespace MageAFK.Items
{
  public class ItemDataBase : SerializedMonoBehaviour, ICraftValue, IItemGetter, IItemGradeUIProvider
  {

    [Header("Materials")]
    [SerializeField] private Dictionary<ItemGrade, Material> itemDropMats;


    [Header("Grade / Type References")]
    [SerializeField] private Dictionary<ItemGrade, Color> gradeColors;
    [SerializeField] private Dictionary<ItemGrade, Dictionary<InventorySpriteType, SlotSprites>> slotSprites;

    [Space(5)]
    [Header("Item Organization")]
    [SerializeField] private Dictionary<ItemIdentification, ItemData> organizedItems;

    [Space(5)]
    [Header("Crafting Table")]
    [SerializeField] private Dictionary<ItemGrade, Dictionary<ItemStructureTypes, CraftingValues>> craftingTable;

    [Header("References")]
    [SerializeField] private DropHandler dropHandler;


    [LabelText("Remove Query"), Space(10)]
    public ItemIdentification searchQuery;


    private void Awake()
    {
      ServiceLocator.RegisterService<ICraftValue>(this);
      ServiceLocator.RegisterService<IItemGetter>(this);
      ServiceLocator.RegisterService<IItemGradeUIProvider>(this);
    }

    #region Set up
    [Button("Fill Dict")]
    public void FillDict()
    {
      organizedItems = new Dictionary<ItemIdentification, ItemData>();
      var allItems = Utility.LoadAllObjects<ItemData>();

      foreach (var item in allItems)
      {
        organizedItems.Add(item.iD, item);
      }
    }

    [Button("Save all items")]
    public void SaveAllItems()
    {
      var allItems = Utility.LoadAllObjects<ItemData>();

      foreach (var item in allItems)
      {
        EditorUtility.SetDirty(item);
      }

      AssetDatabase.Refresh(); // Refresh the AssetDatabase to show the new script in Unity Editor
      AssetDatabase.SaveAssets();
    }


    [Button("Remove")]
    private void PerformSearch()
    {
      if (organizedItems.ContainsKey(searchQuery))
        organizedItems.Remove(searchQuery);
    }

    [Button("Load Drop Tables")]
    private void BuildStructures()
    {
      if (dropHandler == null) { return; }
      dropHandler.ResetDropTables();

      foreach (var item in organizedItems)
      {
        if (item.Value != null && item.Value.dropInfo != null)
        {
          dropHandler.AddItemToDropTable(item.Value);
        }
      }
    }

    #endregion

    #region Crafting Values
    public int ReturnCraftingCost(ItemData data) => craftingTable[data.grade][data.ReturnStructure()].cost;
    public int ReturnCraftingTime(ItemData data) => craftingTable[data.grade][data.ReturnStructure()].time;
    #endregion

    #region Item Getters
    public ItemData ReturnItemData(ItemIdentification iD)
    {
      if (organizedItems.ContainsKey(iD))
      {
        return organizedItems[iD];
      }

      Debug.Log($"-Error- ID: {iD}");
      return null;
    }
    public ItemData ReturnRandomItem(ItemType type, ItemGrade grade = ItemGrade.None)
    {
      var newArray = ReturnItemArray(type, grade);
      int roll = UnityEngine.Random.Range(0, newArray.Length);
      return ReturnItemData(newArray[roll]);
    }
    public ItemIdentification[] ReturnItemArray(ItemType type, ItemGrade grade) =>
    organizedItems.Where(pair => pair.Value.grade == grade && pair.Value.mainType == type)
                  .Select(pair => pair.Key)
                  .ToArray();
    //used by salvager
    #endregion

    #region UI


    public Material ReturnItemDropMaterial(ItemGrade grade)
    {
      if (itemDropMats.TryGetValue(grade, out Material material))
      {
        return material;
      }
      return null;
    }

    public Color GetColor(ItemGrade grade)
    {
      try
      {
        return gradeColors[grade];
      }
      catch (KeyNotFoundException)
      {
        Debug.LogWarning($"Key not found {grade}");
        return Color.white;
      }
    }

    public Sprite GetSlotSprite(ItemGrade grade, InventorySpriteType type, ItemLevel level)
    {
      try
      {
        SlotSprites sprites = slotSprites[grade][type];

        if ((int)level >= 0)
          return sprites.levelSprites[(int)level];
        else
          return sprites.regular;
      }
      catch (KeyNotFoundException)
      {
        Debug.LogWarning($"Error finding key : {grade}, {type}, {level}");
        return null;
      }
      catch (IndexOutOfRangeException)
      {
        Debug.LogWarning($"Index error : {grade}, {type}, {level}");
        return null;
      }
    }

    #endregion
  }

  public interface ICraftValue
  {
    int ReturnCraftingCost(ItemData data);
    int ReturnCraftingTime(ItemData data);
  }

  public interface IItemGetter
  {
    ItemIdentification[] ReturnItemArray(ItemType type, ItemGrade grade = ItemGrade.None);
    ItemData ReturnRandomItem(ItemType type, ItemGrade grade = ItemGrade.None);
    ItemData ReturnItemData(ItemIdentification iD);
  }

  public interface IItemGradeUIProvider
  {
    Color GetColor(ItemGrade grade);
    Sprite GetSlotSprite(ItemGrade grade, InventorySpriteType type, ItemLevel level);

    Material ReturnItemDropMaterial(ItemGrade grade);
  }


  [Serializable]
  public class SlotSprites
  {
    public Sprite regular;
    public Sprite[] levelSprites;
  }


  public enum InventorySpriteType
  {
    Unfilled,
    Filled,
    Void
  }

}


