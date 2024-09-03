using MageAFK.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using MageAFK.Core;
using MageAFK.Management;
using System;
using MageAFK.Animation;
using MageAFK.Player;
using MageAFK.Stats;
using MageAFK.TimeDate;

namespace MageAFK.UI
{
  public class RecipeUIController : SerializedMonoBehaviour
  {

    [Header("Object References")]

    [SerializeField] private IngSlot[] ingSlots;
    [SerializeField] private Image recipeImage, panelImage;

    [SerializeField] private TMP_Text recipeName, time, timePerItem, cost, costPerItem, craft;

    [SerializeField] private CanvasGroup craftGroup;
    [SerializeField] private float offAlpha = 0.3f;
    [SerializeField] private Button blackBackgroundButton;
    [SerializeField] private Slider slider;

    [SerializeField] private Color greenColor;
    [SerializeField] private Color redColor;

    public Color Green => greenColor;
    public Color Red => redColor;

    private float fontSize = 0;

    [Header("References")]
    [SerializeField] private ItemPanelUI itemPanelUI;

    [Serializable]
    private class IngSlot
    {
      public TMP_Text amount;
      public Image itemImage, panelImage;
    }


    public event Action OnClosed;
    private Recipe recipe;
    private int multiplier;

    #region Cycle
    private void Awake() => slider.onValueChanged.AddListener(delegate { OnSliderValueChanged(); });

    private void OnEnable()
    {
      ServiceLocator.Get<InventoryHandler>().InventoryAltered += OnInventoryAltered;
      ServiceLocator.Get<CurrencyHandler>().SubscribeToCurrencyEvent(OnSilverChanged, CurrencyType.SilverCoins, true);
      ServiceLocator.Get<IPlayerDeath>().SubscribeToPlayerDeath(OnClose, true);
    }

    private void OnDisable()
    {
      ServiceLocator.Get<InventoryHandler>().InventoryAltered -= OnInventoryAltered;
      ServiceLocator.Get<CurrencyHandler>().SubscribeToCurrencyEvent(OnSilverChanged, CurrencyType.SilverCoins, false);
      ServiceLocator.Get<IPlayerDeath>().SubscribeToPlayerDeath(OnClose, false);
    }

    private void OnInventoryAltered()
    {
      UpdateIngredientUI();
      UpdateButtonState();
    }

    private void OnSilverChanged(int amount) => UpdateButtonState();

    #endregion

    #region UI
    public void OpenRecipePanel(Recipe recipe)
    {
      if (recipe == null) Debug.Log("Recipe null");
      if (fontSize == 0) fontSize = timePerItem.fontSize;

      if (this.recipe != recipe)
      {
        this.recipe = recipe;
        slider.value = 1f;
        InputBasicUI();
        InputIngredients();
      }
      else
      {
        InputStatAffectedUI();
      }

      OnSliderValueChanged();
      UpdateIngredientUI();
      OnOpen();
    }

    #region Input
    private void InputBasicUI()
    {
      recipeName.text = recipe.ReturnName();
      recipeImage.sprite = recipe.output.image;
      panelImage.sprite = ServiceLocator.Get<IItemGradeUIProvider>().GetSlotSprite(recipe.output.grade,
                                                                                   InventorySpriteType.Filled,
                                                                                   recipe.output.isUpgradable ? ItemLevel.Level0 : ItemLevel.None);
      InputStatAffectedUI();
    }

    private void InputStatAffectedUI()
    {
      costPerItem.text = $"<sprite name=Silver>{ServiceLocator.Get<CraftingHandler>().GetReducedCost(recipe.Cost, 1):N0}";
      var timeValue = ServiceLocator.Get<PlayerStatHandler>().ReturnModifiedValue(Stat.CraftingTime, recipe.TimeToBuild);
      timePerItem.text = ServiceLocator.Get<TimeTaskHandler>().GetTimeLeftString(0, fontSize, fontSize - 40, true, timeValue);
    }

    private void InputIngredients()
    {
      for (int i = 0; i < ingSlots.Length; i++)
      {
        IngSlot slot = ingSlots[i];
        if (i < recipe.ingredients.Length)
        {
          Ingredient ing = recipe.ingredients[i];
          slot.itemImage.sprite = ing.item.image;
          slot.panelImage.sprite = ServiceLocator.Get<IItemGradeUIProvider>().GetSlotSprite(ing.item.grade,
                                                                                            InventorySpriteType.Filled,
                                                                                            ing.level);
          slot.panelImage.gameObject.SetActive(true);
        }
        else
        {
          slot.panelImage.gameObject.SetActive(false);
        }
      }
    }

    #endregion

    #region Update
    private void UpdateIngredientUI()
    {
      var inventoryHandler = ServiceLocator.Get<InventoryHandler>();
      var craftingHandler = ServiceLocator.Get<CraftingHandler>();

      for (int i = 0; i < recipe.ingredients.Length; i++)
      {
        IngSlot slot = ingSlots[i];
        Ingredient ingredient = recipe.ingredients[i];

        bool enoughResources = (ingredient.quantity * multiplier) <= inventoryHandler.ReturnItemAmount((ingredient.item.iD, ingredient.level));
        slot.amount.text = $"{craftingHandler.GetReducedItemAmount(ingredient.quantity, multiplier)}";
        slot.amount.color = enoughResources ? Green : Red;
      }
    }

    public void UpdateButtonState()
    {
      bool state = ServiceLocator.Get<CraftingHandler>().CanCraft(recipe, multiplier);
      craftGroup.interactable = state;
      craftGroup.alpha = state ? 1f : offAlpha;

    }

    public void OnSliderValueChanged()
    {
      multiplier = (int)slider.value;
      var timeValue = ServiceLocator.Get<PlayerStatHandler>().ReturnModifiedValue(Stat.CraftingTime, recipe.TimeToBuild * multiplier);
      time.text = $"{ServiceLocator.Get<TimeTaskHandler>().GetTimeLeftString(0, fontSize, fontSize - 40, true, timeValue, imageStr: "<sprite name=Clock>")}";
      cost.text = $"<sprite name=Silver>{ServiceLocator.Get<CraftingHandler>().GetReducedCost(recipe.Cost, multiplier):N0}";
      craft.text = $"Craft <color=#FFFBA0>x{multiplier * recipe.AmountToCraft}";
      UpdateIngredientUI();
      UpdateButtonState();
    }

    #endregion

    #endregion

    #region Interaction

    private void OnOpen()
    {
      blackBackgroundButton.gameObject.SetActive(true);
      UIAnimations.Instance.OpenPanel(gameObject, () => blackBackgroundButton.onClick.AddListener(OnClose));
    }
    private void OnClose()
    {
      blackBackgroundButton.onClick.RemoveListener(OnClose);
      OnClosed?.Invoke();
      OnClosed = null;
      UIAnimations.Instance.ClosePanel(gameObject, () => blackBackgroundButton.gameObject.SetActive(false));
    }


    public void StartOrder(GameObject button)
    {
      Debug.Log("Pressed");
      ServiceLocator.Get<CraftingHandler>().InitiateOrder(recipe, multiplier);
      craftGroup.interactable = false;
      UIAnimations.Instance.AnimateButton(button.GetComponent<RectTransform>(), () => craftGroup.interactable = true);
    }

    public void OnItemPressed(int index)
    {
      //Check if main slot
      ItemData item = index == -1 ? recipe.output : recipe.ingredients[index].item;

      //Set up variables
      ItemIdentification iD = item.iD;
      ItemLevel level = item.isUpgradable ? ItemLevel.Level0 : ItemLevel.None;

      //Open item panel and subscribe to its closing event.
      itemPanelUI.SetUpAndOpen(iD, level);
    }


    #endregion
  }
}



