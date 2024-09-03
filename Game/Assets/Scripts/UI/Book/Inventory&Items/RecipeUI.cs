using System.Collections.Generic;
using MageAFK.Items;
using MageAFK.Management;
using MageAFK.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MageAFK
{
    public class RecipeUI : MonoBehaviour
    {
        [Header("Object References")]
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private TMP_Text orderAmount;


        [Header("Intialization")]
        [SerializeField] private Transform recipeContentParent;
        [SerializeField] private GameObject recipePrefab;
        [SerializeField] private GameObject lockedPrefab;



        [Header("References")]
        [SerializeField] private RecipeUIController controller;

        private Dictionary<ItemIdentification, GameObject> organizedRecipeDict = new();


        public static bool filterRecipes = false;



        #region LifeCycle
        private void Awake()
        {
            RecipeListButton.InputScripts(controller);

            var recipeHandler = ServiceLocator.Get<RecipeHandler>();
            var recipes = recipeHandler.ReturnRecipeDict();

            foreach (var recipe in recipes.Values)
            {
                if (recipe.isResearched && !organizedRecipeDict.ContainsKey(recipe.ReturnID()))
                    AddNewRecipe(recipe, true);
                else
                    CreateNewLockedObject(recipe.ReturnID());
            }
        }

        private void CreateNewLockedObject(ItemIdentification iD) => organizedRecipeDict[iD] = Instantiate(lockedPrefab, recipeContentParent);

        private void Start() => ServiceLocator.Get<CraftingHandler>().SubscribeToOrderEvent(UpdateHeaderUI, true);

        #endregion


        public void AddNewRecipe(Recipe recipe, bool fromSave = false)
        {
            TryDeleteLock(recipe.ReturnID());
            GameObject slot = Instantiate(recipePrefab, recipeContentParent);
            RecipeListButton button = slot.GetComponent<RecipeListButton>();
            button.SetRecipe(recipe);
            organizedRecipeDict[recipe.ReturnID()] = button.gameObject;
            if (!fromSave) { SetIndicator(recipe.ReturnID()); }
            filterRecipes = true;
        }

        private void TryDeleteLock(ItemIdentification iD)
        {
            if (organizedRecipeDict.TryGetValue(iD, out GameObject lockObj))
            {
                organizedRecipeDict.Remove(iD);
                Destroy(lockObj);
            }
        }

        private void SetIndicator(ItemIdentification iD) => organizedRecipeDict[iD].GetComponent<RecipeListButton>().ToggleIndicator(true);

        public void UpdateHeaderUI() => orderAmount.text = ServiceLocator.Get<CraftingHandler>().ReturnOrderString(true);



        #region Organization
        public void UpdateRecipeUI(List<Recipe> list)
        {
            SetAllRecipesInactive();
            SetOrganizedListToUI(list);
            scrollRect.normalizedPosition = new Vector2(0, 1);
        }

        private void SetAllRecipesInactive()
        {
            if (organizedRecipeDict == null) { return; }
            foreach (var entry in organizedRecipeDict)
            {
                entry.Value.gameObject.SetActive(false);
            }
        }

        private void SetOrganizedListToUI(List<Recipe> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                Recipe recipe = list[i];
                if (organizedRecipeDict.ContainsKey(recipe.ReturnID()))
                {
                    // Enable the GameObject associated with the recipe
                    GameObject recipeGameObject = organizedRecipeDict[recipe.ReturnID()].gameObject;
                    recipeGameObject.SetActive(true);

                    // Make this GameObject a child of the parentObject and set its sibling index to ensure the order
                    recipeGameObject.transform.SetAsFirstSibling();
                }
            }

        }
        #endregion

    }
}
