using MageAFK.Core;
using MageAFK.Items;
using MageAFK.Management;
using MageAFK.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MageAFK
{
  public class VoidItemButton : DynamicItemButton
  {
    [SerializeField] private VoidUI voidUI;
    [SerializeField] private ItemPanelUI itemPanelUI;
    [SerializeField] private Image panel, itemImage;
    [SerializeField] private TMP_Text amount;

    protected override DragZoneIndentifier zone { get => DragZoneIndentifier.Void; }
    protected override Item content { get => voidUI.currentItem; }

    private void OnEnable() => ToggleSubInventoryAltered(true);
    private void OnDisable() => ToggleSubInventoryAltered(false);

    public void ToggleSubInventoryAltered(bool state)
    {
      if (state)
      {
        ServiceLocator.Get<InventoryHandler>().InventoryAltered += UpdateAmount;
        UpdateAmount();
      }
      else
        ServiceLocator.Get<InventoryHandler>().InventoryAltered -= UpdateAmount;
    }

    #region UI

    public override void SetUp(Item spell)
    {
      bool state = content != null
               && !dragInfo.ReturnIfDragging((content.iD, content.ReturnLevel()), DragZoneIndentifier.Void);

      panel.gameObject.SetActive(state);
      amount.gameObject.SetActive(state);

      if (state)
      {
        var itemData = ServiceLocator.Get<IItemGetter>().ReturnItemData(content.iD);
        panel.sprite = ServiceLocator.Get<IItemGradeUIProvider>().GetSlotSprite(itemData.grade,
                                                                                InventorySpriteType.Void,
                                                                                content.ReturnLevel());
        itemImage.sprite = itemData.image;
        UpdateAmount();
      }
    }

    private void UpdateAmount()
    {
      if (content == null) return;
      amount.text = $"x{ServiceLocator.Get<InventoryHandler>().ReturnItemAmount((content.iD, content.ReturnLevel()))}";
      voidUI.UpdateFeedSlider();
    }
    #endregion

    #region Interaction
    protected override void OnSingleClick() => itemPanelUI.SetUpAndOpen(voidUI.currentItem.iD, voidUI.currentItem.ReturnLevel());
    protected override void OnDoubleClick() => OnSingleClick();

    //We do this because we want to allow this to be augmented even during waves.
    protected override bool ReturnIfAlterable() => ReturnIfInteractable() && WaveHandler.WaveState != WaveState.None;

    #endregion
  }

}
