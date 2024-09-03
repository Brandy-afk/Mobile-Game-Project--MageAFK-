using System.Collections.Generic;
using MageAFK.Management;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MageAFK.Items
{
  [CreateAssetMenu(fileName = "New Recipe", menuName = "Items/Recipes/Recipe")]
  public class Recipe : ScriptableObject
  {

    [SerializeField, TabGroup("Information"), Header("For understanding its use"), GUIColor("GetColorForField")]
    private ItemGrade grade;

    [TabGroup("Information")]
    public ItemData output;

    [SerializeField, TabGroup("Information"), Header("Type of Research")]
    private RecipeUnlockType unlockType;

    [TabGroup("Crafting")]
    public Ingredient[] ingredients;

    [SerializeField, TabGroup("Crafting"), InfoBox("Amount of items per order")]
    private int amountToCraft = 1;

    [SerializeField, TabGroup("Crafting"), Tooltip("Enabled means you will have to set custom time and cost per craft")]
    private bool custom = false;

    [SerializeField, TabGroup("Crafting"), ShowIf("ReturnIfCustom")]
    private CraftingValues craftingValues;

    [ReadOnly] public bool isResearched;


    public virtual void OnRecipeCrafted(Order order) =>
    ServiceLocator.Get<InventoryHandler>().AddItem(output.iD, output.isUpgradable ? ItemLevel.Level0 : ItemLevel.None,
                                          order.multiplier * order.recipe.amountToCraft);
    // if (usesWater) { ServiceLocator.Get<InventoryHandler>().AddItem(ItemFactory.CreateItem(ServiceLocator.Get<ItemDataBase>().ReturnItemData(ItemIdentification.WaterStone)), 1); }


    #region Getters

    public ItemGrade Grade => grade;
    public RecipeUnlockType UnlockType => unlockType;
    public int AmountToCraft => amountToCraft;
    public int Cost => custom ? craftingValues.cost : ServiceLocator.Get<ICraftValue>().ReturnCraftingCost(output);
    public int TimeToBuild => custom ? craftingValues.time : ServiceLocator.Get<ICraftValue>().ReturnCraftingTime(output);


    #endregion

    #region Helper

    #region Interface
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

    private bool ReturnIfCustom() => custom;

    #endregion
    public string ReturnName() => output.itemName;
    public ItemIdentification ReturnID() => output.iD;
    public void SetData(RData data) => isResearched = data.isResearched;

    //For framework
    public void SetGrade(ItemGrade grade) => this.grade = grade;
    public void SetUnlockType(RecipeUnlockType type) => unlockType = type;

    #endregion

    #region oveloads

    public static bool operator ==(Recipe lhs, Recipe rhs)
    {
      if (ReferenceEquals(lhs, null))
      {
        return ReferenceEquals(rhs, null);
      }
      return lhs.Equals(rhs);
    }

    public static bool operator !=(Recipe lhs, Recipe rhs) => !(lhs == rhs);

    public override bool Equals(object other)
    {
      Recipe obj = other as Recipe;
      if (other == null || obj.output == null) return false;
      return output.iD == obj.output.iD;
    }

    public override int GetHashCode()
    {
      return output?.iD.GetHashCode() ?? 0;
    }


    #endregion
  }


  [System.Serializable]
  public class RData
  {
    public ItemIdentification recipeID;
    public bool isResearched;

    public RData(ItemIdentification recipeID, bool isResearched)
    {
      this.recipeID = recipeID;
      this.isResearched = isResearched;
    }
  }

}
