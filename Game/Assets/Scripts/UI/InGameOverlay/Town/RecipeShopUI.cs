using System;
using System.Collections.Generic;
using System.Linq;
using MageAFK.Core;
using MageAFK.Items;
using MageAFK.Management;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace MageAFK.UI
{
  public class RecipeShopUI : SerializedMonoBehaviour
  {
    [SerializeField] private ShopSlot[] slots;
    [SerializeField] private Dictionary<ItemGrade, Sprite> panelSprites;
    [SerializeField] private RecipeShopHandler recipeShopHandler;
    [SerializeField] private GameObject indicator;
    [SerializeField] private RecipeShopPopUp popUp;

    private List<ItemIdentification> currentShop;

    [System.Serializable]
    private class ShopSlot
    {
      public UnityEngine.UI.Image item;
      public TMP_Text cost;
      public GameObject blackMask;
      public UnityEngine.UI.Button button;

    }

    private void OnEnable() => ServiceLocator.Get<CurrencyHandler>().SubscribeToCurrencyEvent(OnGoldChanged, CurrencyType.GoldBars, true);
    private void OnDisable()
    {
      ServiceLocator.Get<CurrencyHandler>().SubscribeToCurrencyEvent(OnGoldChanged, CurrencyType.GoldBars, false);

      if (indicator.activeSelf)
        indicator.SetActive(false);
    }
    public void OnGoldChanged(int amount) => UpdateSlots(recipeShopHandler.currentShop);

    public void FillShopSlots(List<ItemIdentification> iDs, bool newShop = false)
    {
      var recipeHandler = ServiceLocator.Get<RecipeHandler>();
      currentShop = iDs;

      for (int i = 0; i < slots.Length; i++)
      {
        Recipe recipe = recipeHandler.ReturnRecipe(iDs[i]);
        if (i > iDs.Count - 1 || recipe.isResearched)
        {
          slots[i].button.gameObject.SetActive(false);
        }
        else
        {
          var cost = recipeShopHandler.ReturnCost(recipe.Grade);
          slots[i].item.sprite = recipe.output.image;
          slots[i].cost.text = string.Concat(Enumerable.Repeat("<sprite name=Gold>", Mathf.Min(4, (cost / 3) + 1)));
        }
      }

      if (newShop)
        indicator.SetActive(true);
    }

    public void UpdateSlots(List<ItemIdentification> identification)
    {
      var recipeHandler = ServiceLocator.Get<RecipeHandler>();
      var currencyHandler = ServiceLocator.Get<CurrencyHandler>();

      for (int i = 0; i < identification.Count; i++)
      {
        Recipe recipe = recipeHandler.ReturnRecipe(identification[i]);
        if (recipe.isResearched) continue;

        bool isAffordable = currencyHandler.ReturnAffordable(CurrencyType.GoldBars, recipeShopHandler.ReturnCost(recipe.Grade));

        slots[i].blackMask.SetActive(!isAffordable);
        slots[i].button.interactable = isAffordable;
      }
    }

    #region Interaction / Popup

    private int chosenIndex = -1;
    public void OnSlotPressed(int index)
    {
      if (index < 0 || index > currentShop.Count - 1)
        return;

      chosenIndex = index;
      Recipe recipe = ServiceLocator.Get<RecipeHandler>().ReturnRecipe(currentShop[chosenIndex]);
      if (popUp.FillAndOpen(panelSprites[recipe.Grade], recipe, recipeShopHandler.ReturnCost(recipe.Grade)))
        popUp.OnDecision += OnPurchaseDecision;
    }

    private void OnPurchaseDecision(bool purchase)
    {
      if (chosenIndex != -1 && purchase)
      {
        var recipeHandler = ServiceLocator.Get<RecipeHandler>();
        Recipe recipe = recipeHandler.ReturnRecipe(currentShop[chosenIndex]);
        if (ServiceLocator.Get<CurrencyHandler>()
        .SubtractCurrency(CurrencyType.GoldBars, recipeShopHandler.ReturnCost(recipe.Grade)))
        {
          recipeHandler.AddKnownRecipe(currentShop[chosenIndex]);
          slots[chosenIndex].button.gameObject.SetActive(false);
        }
      }
      chosenIndex = -1;
    }

    #endregion

  }

}
