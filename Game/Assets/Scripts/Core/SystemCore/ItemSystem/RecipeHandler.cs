using System;
using System.Collections.Generic;
using UnityEngine;
using MageAFK.Management;
using MageAFK.UI;
using Sirenix.OdinInspector;
using System.Linq;
using MageAFK.Core;
using MageAFK.Tools;

namespace MageAFK.Items
{
  /// <summary>
  /// Intended to handle organization and data handling of all Recipes.
  /// </summary>
  public class RecipeHandler : SerializedMonoBehaviour, IData<RecipeData>
  {

    [SerializeField] private Dictionary<ItemIdentification, Recipe> recipeDict;

    [SerializeField] private RecipeUI recipeUI;

    #region SetUp

    [Button("Create Recipe Collection")]
    public void SetUp()
    {
      recipeDict = new Dictionary<ItemIdentification, Recipe>();
      var allRecipes = Utility.LoadAllObjects<Recipe>();

      foreach (var recipe in allRecipes)
      {
        recipeDict.Add(recipe.ReturnID(), recipe);
        if (recipe.UnlockType == RecipeUnlockType.isKnownAtStart)
          recipe.isResearched = true;
        else
          recipe.isResearched = false;
      }
    }

    #endregion

    #region Save/Load/Intialize (In order)

    private void Awake()
    {
      ServiceLocator.RegisterService<IData<RecipeData>>(this);
      ServiceLocator.RegisterService(this);
    }

    public void InitializeData(RecipeData data)
    {
      foreach (var rData in data.recipeData)
        if (recipeDict.TryGetValue(rData.recipeID, out Recipe recipe))
          recipe.SetData(rData);
    }

    public RecipeData SaveData()
    {
      List<RData> recipeD = new();
      foreach (var recipe in recipeDict.Values)
      {
        if (recipe.isResearched)
          recipeD.Add(new RData(recipe.ReturnID(), true));
      }
      return new RecipeData(recipeD);
    }

    #endregion




    public void AddKnownRecipe(ItemIdentification id)
    {
      if (!recipeDict.TryGetValue(id, out Recipe recipe) || recipe.isResearched == true)
      {
        Debug.LogWarning($"Recipe not found for ID || already researched: {id}");
        return;
      }

      recipe.isResearched = true; // Mark the recipe as researched
      recipeUI.AddNewRecipe(recipe);
      // If adding as part of the gameplay (not from loading a save), trigger UI updates

      ServiceLocator.Get<IndicatorHandler>().SetUIChain(UIPanel.Book_Inventory_Crafting_Recipes);
      if (WaveHandler.WaveState == WaveState.None) SaveManager.Save(SaveData(), DataType.RecipeData);
    }



    #region Helpers
    public Dictionary<ItemIdentification, Recipe> ReturnRecipeDict() => recipeDict;
    public Recipe ReturnRandomVoidRecipe(ItemIdentification[] inList)
    {
      var voidRecipes = recipeDict.Values.Where(recipe => recipe.Grade == ItemGrade.Corrupt && !recipe.isResearched)
                                         .Select(recipe => recipe.ReturnID())
                                         .ToArray();

      return voidRecipes.Length > 0 ? recipeDict[voidRecipes[UnityEngine.Random.Range(0, voidRecipes.Length)]] : null;
    }

    public bool ReturnRecipeIfCraftable(ItemIdentification iD)
    {
      if (recipeDict.TryGetValue(iD, out Recipe recipe))
      {
        return recipe.isResearched;
      }
      return false;
    }

    public Recipe ReturnRecipe(ItemIdentification iD)
    {
      if (recipeDict.TryGetValue(iD, out Recipe recipe))
      {
        return recipe;
      }
      return null;
    }


    #endregion

  }

  [Serializable]
  public class RecipeData
  {
    public List<RData> recipeData;

    public RecipeData(List<RData> data)
    {
      recipeData = data;
    }
  }





}