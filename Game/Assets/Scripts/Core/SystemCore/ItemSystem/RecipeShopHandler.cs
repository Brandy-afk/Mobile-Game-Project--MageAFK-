using System.Collections.Generic;
using UnityEngine;
using MageAFK.Management;
using MageAFK.Items;
using Sirenix.OdinInspector;
using System.Linq;
using MageAFK.UI;

namespace MageAFK.Core
{
  public class RecipeShopHandler : SerializedMonoBehaviour, IData<RecipeShopData>
  {

    [ReadOnly] public List<ItemIdentification> currentShop;

    [Space(5)]
    [Header("Represents amount of slots associated to the different grades")]
    [Header("0 - common, 1 - unique ,etc")]
    [SerializeField] private int[] gradeAmounts;

    [Header("Recipe Cost based on grade")]
    [SerializeField] private Dictionary<ItemGrade, int> gradeCost;

    [SerializeField] private RecipeShopUI recipeShopUI;

    #region Intialization / Cycle

    private void Awake() => ServiceLocator.RegisterService<IData<RecipeShopData>>(this);

    private void Start()
    {
      WaveHandler.SubToSiegeEvent((Status state) =>
      {
        if (state == Status.End)
          CreateNewShop();
      }, true);
      SaveManager.OnSaveDataLoaded += OnDataLoaded;
    }

    public void InitializeData(RecipeShopData data)
    {
      if (data != null && data.shop.Count > 0)
      {
        currentShop = data.shop;
      }
    }

    public RecipeShopData SaveData()
    {
      return new RecipeShopData(currentShop);
    }



    private void OnDataLoaded()
    {
      if (currentShop == null || currentShop.Count <= 0)
      {
        CreateNewShop();
      }
      else
      {
        recipeShopUI.FillShopSlots(currentShop);
        recipeShopUI.UpdateSlots(currentShop);
      }
    }



    #endregion

    #region Shop Creation
    public void CreateNewShop()
    {
      var gradeToCountMap = BuildGradeToCountMap();
      var recipeHandler = ServiceLocator.Get<RecipeHandler>();
      var potentialRecipes = GetFilteredAndShuffledRecipes(recipeHandler);
      SelectRecipesForShop(gradeToCountMap, potentialRecipes);
      FillAndOrderShop(potentialRecipes, recipeHandler);
      UpdateShopUI();

      SaveManager.Save(SaveData(), DataType.RecipeShopData);
    }
    private void SelectRecipesForShop(Dictionary<ItemGrade, int> gradeToCountMap, List<Recipe> potentialRecipes)
    {
      currentShop = potentialRecipes
          .Where(recipe => gradeToCountMap[recipe.Grade] > 0 && gradeToCountMap[recipe.Grade]-- > 0)
          .Select(recipe => recipe.ReturnID())
          .ToList();
    }

    private Dictionary<ItemGrade, int> BuildGradeToCountMap()
    {
      return new Dictionary<ItemGrade, int>
    {
        {ItemGrade.Common, gradeAmounts[0]},
        {ItemGrade.Unique, gradeAmounts[1]},
        {ItemGrade.Rare, gradeAmounts[2]},
        {ItemGrade.Artifact, gradeAmounts[3]},
        {ItemGrade.Corrupt, 0}
    };
    }

    private List<Recipe> GetFilteredAndShuffledRecipes(RecipeHandler recipeHandler)
    {
      return recipeHandler.ReturnRecipeDict()
          .Where(pair => !pair.Value.isResearched && pair.Value.Grade != ItemGrade.Corrupt)
          .Select(pair => pair.Value)
          .OrderBy(recipe => Random.Range(0, 50)) // Assuming Random.Range is a placeholder; adjust for actual shuffle logic
          .ToList();
    }

    private void FillAndOrderShop(List<Recipe> potentialRecipes, RecipeHandler recipeHandler)
    {
      if (currentShop.Count < 8)
      {
        var additionalRecipes = potentialRecipes
            .Where(recipe => !currentShop.Contains(recipe.ReturnID()))
            .GroupBy(recipe => recipe.Grade)
            .SelectMany(group => group.Take(8 - currentShop.Count))
            .Select(recipe => recipe.ReturnID());

        currentShop.AddRange(additionalRecipes);
      }

      // Order by grade
      currentShop = currentShop
          .OrderBy(id => recipeHandler.ReturnRecipe(id).Grade)
          .ToList();
    }

    private void UpdateShopUI()
    {
      // Assuming recipeShopUI and IndicatorHandler are accessible
      recipeShopUI.FillShopSlots(currentShop, true);
      recipeShopUI.UpdateSlots(currentShop);
      ServiceLocator.Get<IndicatorHandler>().SetUIChain(UIPanel.Town_Recipes);
      SaveManager.Save(SaveData(), DataType.RecipeShopData);
    }
    #endregion

    public int ReturnCost(ItemGrade grade)
    {
      if (gradeCost.ContainsKey(grade))
      {
        return gradeCost[grade];
      }
      Debug.Log($"Error with {grade}");
      return 0;
    }

  }

  [System.Serializable]
  public class RecipeShopData
  {
    public List<ItemIdentification> shop;
    public RecipeShopData(List<ItemIdentification> shop) => this.shop = shop;
  }

}
