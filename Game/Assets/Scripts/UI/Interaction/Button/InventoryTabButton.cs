
using System;
using MageAFK.Items;
using MageAFK.Management;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MageAFK.UI
{
  public class InventoryTabButton : DynamicItemButton
  {

    [SerializeField] private Image itemImage;
    [SerializeField] private Image panelImage;
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private GameObject indicatorObject;
    [SerializeField] private ItemPanelUI panel;
    [SerializeField] private int index;
    public int Index => index;

    protected override DragZoneIndentifier zone => DragZoneIndentifier.Inventory;
    private InventoryHandler inventoryHandler;

    public override void SetUp(Item item)
    {
      inventoryHandler ??= ServiceLocator.Get<InventoryHandler>();
      content = item;
      var key = item == null ? default : item.Key;
      var slot = inventoryHandler.ReturnSlot(key);
      bool notNull = slot != null && !dragInfo.ReturnIfDragging(key, DragZoneIndentifier.Inventory);

      panelImage.gameObject.SetActive(notNull);

      if (!notNull) return;

      SetupUI(key, slot);
      ToggleItemIndicator(!inventoryHandler.foundItems.Contains(key));
    }

    private void SetupUI((ItemIdentification, ItemLevel) key, ItemSlot slot)
    {
      var itemData = ServiceLocator.Get<IItemGetter>().ReturnItemData(key.Item1);
      panelImage.sprite = ServiceLocator.Get<IItemGradeUIProvider>().GetSlotSprite(itemData.grade, InventorySpriteType.Unfilled, key.Item2);
      amountText.text = "x" + slot.quantity.ToString();
      itemImage.sprite = itemData.image;
    }

    public void ToggleItemIndicator(bool indicatorState)
    {
      itemImage.gameObject.SetActive(!indicatorState);
      indicatorObject.SetActive(indicatorState);
    }

    public void UpdateCount(int count) => amountText.text = "x" + count.ToString();

    #region Interaction
    protected override void OnSingleClick() => panel.SetUpAndOpen(content.iD, content.ReturnLevel());
    protected override void OnDoubleClick() => OnSingleClick();
    public override void OnBeginDrag(PointerEventData eventData)
    {
      if (ReturnIfInteractable())
      {
        var key = (content.iD, content.ReturnLevel());
        if (!ServiceLocator.Get<InventoryHandler>().foundItems.Contains(key))
        {
          OnSingleClick();
          return;
        }

        base.OnBeginDrag(eventData);
      }
      else
      {
        eventData.pointerDrag = null;
      }
    }

    #endregion
  }
}
