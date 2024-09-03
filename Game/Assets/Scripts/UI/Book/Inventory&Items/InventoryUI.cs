
using System;
using System.Collections.Generic;
using MageAFK.Items;
using MageAFK.Management;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MageAFK.UI
{
  public class InventoryUI : MonoBehaviour, IPagination<(ItemIdentification, ItemLevel)>
  {

    [Header("Objects")]
    //Inventory page
    [SerializeField, TabGroup("Inventory")] private InventoryTabButton[] slots;
    [SerializeField, TabGroup("Inventory")] private ButtonUpdateClass[] buttons;

    [SerializeField, TabGroup("References")] private VoidUI voidUI;
    [SerializeField, TabGroup("References")] private FilterHandler filterPopUp;


    //Current Tab Buttons
    private Dictionary<(ItemIdentification, ItemLevel), InventoryTabButton> activeItems;
    private const int ITEMS_PER_PAGE = 9;

    #region Ref
    private InventoryHandler inventoryHandler;
    private Pagination<(ItemIdentification, ItemLevel)> currentPagination;
    private IDragInfo<Item, (ItemIdentification, ItemLevel)> dragInfo;

    #endregion


    #region Intialization , Cycle, Events
    private void Start()
    {
      activeItems = new();
      dragInfo = ServiceLocator.Get<IDragInfo<Item, (ItemIdentification, ItemLevel)>>();
      inventoryHandler = ServiceLocator.Get<InventoryHandler>();
      inventoryHandler.InputInventoryUI(this);
      ServiceLocator.Get<IDragZoneCreator<Item>>().AddDragZone(slots[0].transform.parent.GetComponent<RectTransform>(),
                                                         OnHovering,
                                                         OnDropped,
                                                         DragZoneIndentifier.Inventory);
      OnEnable();
    }

    public void OnEnable()
    {
      if (inventoryHandler == null) return;
      inventoryHandler.FilterInventorySlots(false);
    }
    public void OnFilterOptionsPressed() => filterPopUp.OpenFilterPanel(inventoryHandler, inventoryHandler.TypeFilter, inventoryHandler.GradeFilter);

    #endregion

    #region UI
    public void FilterSlots(List<(ItemIdentification, ItemLevel)> slots, bool isNewFilter)
    {
      var page = isNewFilter || currentPagination == null ? 1 : currentPagination.ReturnCurrentPage();
      currentPagination = new Pagination<(ItemIdentification, ItemLevel)>(slots, this, ITEMS_PER_PAGE, page);
      currentPagination.UpdateDisplay();
    }
    public void UpdateSlot((ItemIdentification, ItemLevel) key, int index)
    {
      var slot = inventoryHandler.ReturnSlot(key);
      var slotKey = slot != null ? slot.item.Key : default;
      activeItems[slotKey] = slots[index];
      slots[index].SetUp(slot?.item);
    }
    public void AlterUICount((ItemIdentification, ItemLevel) key, int quantity)
    {
      try
      {
        activeItems[key].UpdateCount(quantity);
      }
      catch (KeyNotFoundException)
      {
        Debug.Log($"Active item not found {key}");
      }
    }
    public void RemoveIndicator((ItemIdentification, ItemLevel) key)
    {
      if (IsActive(key))
      {
        var slot = activeItems[key];
        slot.ToggleItemIndicator(false);
      }
    }
    public bool IsActive((ItemIdentification, ItemLevel) key) => activeItems.ContainsKey(key);

    #endregion

    #region Pagination
    public void UpdatePageButtons() => Pagination<(ItemIdentification, ItemLevel)>.UpdatePageButtons(currentPagination, buttons);
    public void AlterPagePressed(bool isNext)
    {
      if (isNext) currentPagination.NextPage();
      else currentPagination.PreviousPage();
    }
    public void CustomPaginationBehaviour() => activeItems.Clear();

    #endregion

    #region Drag Zone Functions

    public void OnHovering(bool state)
    {
      if (state)
      {
        //Turn on indicator
      }
      else
      {
        //Turn off indicator
      }
      //TODO Code here will apply a indicator of some fashion.
    }

    public bool OnDropped()
    {
      if (dragInfo.Orgin == DragZoneIndentifier.Void)
        return VoidItemDropped();
      else if (dragInfo.Orgin == DragZoneIndentifier.Gear)
        return GearItemDropped();

      return false;
    }

    private bool VoidItemDropped()
    {
      voidUI.ResetVoidUI(true);
      return true;
    }

    private bool GearItemDropped()
    {
      var itemData = ServiceLocator.Get<IItemGetter>().ReturnItemData(dragInfo.Drag.iD);
      return ServiceLocator.Get<GearHandler>().UnequipGear(itemData.mainType, true);
    }

    #endregion
  }




}

