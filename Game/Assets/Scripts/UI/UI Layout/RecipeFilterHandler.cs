using System.Collections.Generic;
using UnityEngine;
using MageAFK.Items;
using System.Linq;
using TMPro;

using System.Text.RegularExpressions;
using System;
using UnityEngine.Pool;
using MageAFK.Management;

namespace MageAFK.UI
{
    public class RecipeFilterHandler : MonoBehaviour, IFilterHandler
  {


    [Header("Objects")]
    [SerializeField] private GameObject filterPopUp;
    [SerializeField] private List<TMP_Text> filters; // 0 is type , 1 is grade
    [SerializeField] private TMP_InputField searchBar;

    [Header("References")]
    [SerializeField] private RecipeUI recipeUI;
    [SerializeField] private FilterHandler filterHandler;

    private ItemTypeFilter typeFilter = ItemTypeFilter.AllTypes;
    private ItemGradeFilter gradeFilter = ItemGradeFilter.AllGrades;
    private string stringFilter;
    private FilterGroup lastFilterGroup = null;

    private class FilterGroup
    {
      private ItemTypeFilter typeFilter;
      private ItemGradeFilter gradeFilter;
      private string stringFilter;

      public FilterGroup(ItemTypeFilter t, ItemGradeFilter g, string s)
      {
        typeFilter = t;
        gradeFilter = g;
        stringFilter = s;
      }

      public bool CheckIfSameValues(FilterGroup group) => group.typeFilter == typeFilter && group.gradeFilter == gradeFilter && group.stringFilter == stringFilter;
    }

    private void OnEnable()
    {
      stringFilter = "";
      FilterItems();
    }

    public void OnFilterButtonPressed() => filterHandler.OpenFilterPanel(this, typeFilter, gradeFilter);

    public void SearchPressed()
    {
      string result = ExtractLetters(searchBar.text);
      if (stringFilter == result) { return; }
      searchBar.text = result;
      stringFilter = result;
      FilterItems();
    }

    public string ExtractLetters(string input)
    {
      // Remove all non-letter characters using Regex
      string result = Regex.Replace(input, "[^a-zA-Z]", "");
      // Trim the result to remove spaces and check if it's empty
      result = result.Trim();
      if (string.IsNullOrEmpty(result))
      {
        // Handle empty result here if needed
        return "";
      }
      return result;
    }

    private void FilterItems()
    {
      FilterGroup newGroup = new(typeFilter, gradeFilter, stringFilter);
      if (lastFilterGroup == null || RecipeUI.filterRecipes || !lastFilterGroup.CheckIfSameValues(newGroup))
      {
        List<Recipe> filteredItems = ListPool<Recipe>.Get();
        var recipeList = ServiceLocator.Get<RecipeHandler>().ReturnRecipeDict().Select(pair => pair.Value).ToList();

        Enum.TryParse(typeFilter.ToString(), out ItemType typeValue);
        Enum.TryParse(gradeFilter.ToString(), out ItemGrade gradeValue);

        filteredItems.Clear();
        filteredItems = recipeList
         .Where(recipe =>
             (typeFilter == ItemTypeFilter.AllTypes || recipe.output.ReturnIfType(typeValue)) &&
             (gradeFilter == ItemGradeFilter.AllGrades || recipe.output.grade == gradeValue) &&
             (stringFilter == "" || recipe.ReturnName().StartsWith(stringFilter, StringComparison.OrdinalIgnoreCase)))
         .OrderBy(recipe => recipe.isResearched)
         .ThenBy(recipe => (int)recipe.output.grade)
         .ThenBy(recipe => recipe.output.mainType)
         .ToList();

        lastFilterGroup = newGroup;
        RecipeUI.filterRecipes = false;

        recipeUI.UpdateRecipeUI(filteredItems);

        ListPool<Recipe>.Release(filteredItems);
      }
    }

    public void ApplyFilterChange(ItemTypeFilter type, ItemGradeFilter grade)
    {
      typeFilter = type;
      gradeFilter = grade;
      FilterItems();
    }
  }

}