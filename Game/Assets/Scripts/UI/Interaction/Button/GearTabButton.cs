using MageAFK.Core;
using MageAFK.Items;
using MageAFK.Management;
using MageAFK.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MageAFK.UI
{
  public class GearTabButton : DynamicItemButton
  {

    [SerializeField] private GearUI gearUI;

    [Header("Gear Button Handler")]
    [SerializeField] private Item item;
    [SerializeField] private Image panelImage;
    [SerializeField] private Image itemImage;
    [SerializeField] private Image floatImage;
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private TMP_Text stats;
    [SerializeField] private ItemPanelUI itemPanelUI;

    protected override DragZoneIndentifier zone => DragZoneIndentifier.Gear;

    public override void SetUp(Item newItem)
    {
      item = newItem;
      bool state = item != null
                && !ServiceLocator.Get<IDragInfo<Item, (ItemIdentification, ItemLevel)>>()
                                  .ReturnIfDragging((item.iD, item.ReturnLevel()), DragZoneIndentifier.Gear);

      panelImage.gameObject.SetActive(state);
      itemName.gameObject.SetActive(state);
      stats.gameObject.SetActive(state);
      if (!state) return;

      var itemData = ServiceLocator.Get<IItemGetter>().ReturnItemData(item.iD);
      itemImage.gameObject.SetActive(state);
      itemImage.sprite = itemData.image;
      panelImage.sprite = ServiceLocator.Get<IItemGradeUIProvider>().GetSlotSprite(itemData.grade,
                                                                                   InventorySpriteType.Filled,
                                                                                   item.ReturnLevel());
      itemName.text = itemData.itemName;
      SetUpTraits(itemData, item.ReturnLevel());
      ToggleUI();
    }

    public void ItemHovering(bool state, ItemType type)
    {
      if (state &&
      (WaveHandler.WaveState == WaveState.Wave || floatImage.gameObject.activeInHierarchy)) return;

      if (state)
      {
        var dragItemData = ServiceLocator.Get<IItemGetter>()
                                        .ReturnItemData(ServiceLocator.Get<IDragInfo<Item, (ItemIdentification, ItemLevel)>>().Drag.iD);

        if (dragItemData.mainType == type)
        {
          floatImage.gameObject.SetActive(true);
          floatImage.sprite = dragItemData.image;
        }
      }
      else
        floatImage.gameObject.SetActive(false);

    }

    private void SetUpTraits(ItemData data, ItemLevel level)
    {
      var armourData = data as ArmourData;
      var colorHex = ColorUtility.ToHtmlStringRGB(Utility.AlterColor(ServiceLocator.Get<IItemGradeUIProvider>().GetColor(data.grade), false));
      stats.text = armourData.ReturnArmourInfo(level, colorHex);
      stats.enableWordWrapping = armourData.WrapText;
    }

    public void ToggleUI()
    {
      stats.gameObject.SetActive(item == null ? false : gearUI.StatState);
      itemName.gameObject.SetActive(item == null ? false : !gearUI.StatState);
    }

    #region Interaction

    protected override void OnSingleClick() => itemPanelUI.SetUpAndOpen(item.iD, item.ReturnLevel());
    protected override void OnDoubleClick() => OnSingleClick();

    #endregion

  }
}
