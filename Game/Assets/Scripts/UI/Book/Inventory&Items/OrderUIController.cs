using MageAFK.Animation;
using MageAFK.Core;
using MageAFK.Items;
using MageAFK.Management;
using MageAFK.TimeDate;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MageAFK.Player;
using UnityEngine.EventSystems;

namespace MageAFK.UI
{
  public class OrderUIController : MonoBehaviour, IPointerClickHandler
  {
    [SerializeField] private GameObject conditionalPanel, finishPanel;
    [SerializeField] private TMP_Text gemCost, quantity, time, finishText;
    [SerializeField] private Image panelImage, itemImage, itemPanelImage;
    [SerializeField] private ButtonUpdateClass skipButton;

    private static OrderUI orderUI;
    private Order order;
    public static void InputOrderUI(OrderUI ui) => orderUI = ui;
    public void SetOrderData(Order orderData, bool _lock)
    {
      order = orderData;

      if (_lock)
        LockOrder();
      else
        UnlockOrder();
    }

    private void LockOrder() => gameObject.SetActive(false);

    private void UnlockOrder()
    {
      gameObject.SetActive(true);
      ToggleAppearence(true);
      var provider = ServiceLocator.Get<IItemGradeUIProvider>();
      itemImage.sprite = order.recipe.output.image;
      itemPanelImage.sprite = provider.GetSlotSprite(order.recipe.output.grade,
                                                     InventorySpriteType.Filled,
                                                     order.recipe.output.isUpgradable ? ItemLevel.Level0 : ItemLevel.None);
      quantity.text = $"x{order.multiplier * order.recipe.AmountToCraft}";
      panelImage.color = provider.GetColor(order.recipe.output.grade);
      finishText.text = $"Collect <color=#B1FFAA>x{order.recipe.AmountToCraft * order.multiplier} {order.recipe.output.itemName}";
    }

    public void UpdateTimeUI()
    {
      if (!gameObject.activeInHierarchy) return;
      time.text = $"<sprite name=Clock>{ServiceLocator.Get<TimeTaskHandler>().GetTimeLeftString(order.key, time.fontSize, time.fontSize - 20)}";
      UpdateSkipButton();
    }

    private void UpdateSkipButton()
    {
      var costToComplete = ServiceLocator.Get<TimeTaskHandler>().ReturnGemCostToComplete(order.key);
      gemCost.text = $"<sprite name=Gem>{costToComplete}";

      bool affordable = ServiceLocator.Get<CurrencyHandler>().ReturnAffordable(CurrencyType.DemonicGems, costToComplete);
      skipButton.black.SetActive(!affordable);
      skipButton.button.interactable = affordable;
    }

    public void OnFinish() => ToggleAppearence(false);

    private void ToggleAppearence(bool isNew)
    {
      conditionalPanel.gameObject.SetActive(isNew);
      finishPanel.gameObject.SetActive(!isNew);
    }

    #region Interaction
    public void CancelPressed(GameObject button)
    {
      UIAnimations.Instance.AnimateButton(button);
      ServiceLocator.Get<CraftingHandler>().EndOrderEarly(order, true);
    }

    public void SkipPressed(GameObject button)
    {
      if (ServiceLocator.Get<CurrencyHandler>().SubtractCurrency(CurrencyType.DemonicGems, ServiceLocator.Get<TimeTaskHandler>().ReturnGemCostToComplete(order.key)))
      {
        UIAnimations.Instance.AnimateButton(button);
        ServiceLocator.Get<CraftingHandler>().EndOrderEarly(order, false);
      }
    }

    public void OnPointerClick(PointerEventData eventData) => orderUI.InspectRecipe(order.recipe);

    public void ItemPressed(GameObject item)
    {
      UIAnimations.Instance.AnimateButton(item);
      orderUI.InpsectItem(order.recipe.ReturnID(), order.recipe.output.isUpgradable ? ItemLevel.Level0 : ItemLevel.None, order.key);
    }


    public void CollectPressed(GameObject button)
    {
      UIAnimations.Instance.AnimateButton(button);
      order.recipe.OnRecipeCrafted(order);
      ServiceLocator.Get<PlayerData>().AddStatValue(PlayerStatisticEnum.ItemsCrafted, order.multiplier * order.recipe.AmountToCraft);
      ServiceLocator.Get<CraftingHandler>().OnCollectOrCancel(order.key);
    }

    #endregion
  }

}