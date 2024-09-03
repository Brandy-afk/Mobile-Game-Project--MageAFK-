
using System.Collections.Generic;
using MageAFK.UI;
using System;
using MageAFK.Management;
using System.Linq;
using MageAFK.Core;

namespace MageAFK.Items
{
  public class InventoryHandler : IData<PermInventoryData>, IFilterHandler
  {

    //TODO Needs to have null checks added for ui null

    private InventoryUI inventoryUI;

    private Dictionary<(ItemIdentification, ItemLevel), ItemSlot> itemSlots = new();
    public HashSet<(ItemIdentification, ItemLevel)> foundItems = new();
    private (ItemIdentification, ItemLevel) exclusion = (ItemIdentification.None, ItemLevel.None);

    private (ItemTypeFilter, ItemGradeFilter) currentFilters = (ItemTypeFilter.AllTypes, ItemGradeFilter.AllGrades);

    public delegate void OnSlotAlteredHandler();
    public event OnSlotAlteredHandler InventoryAltered;


    public InventoryHandler()
    {
      WaveHandler.SubToSiegeEvent((Status status) =>
     {
       if (status == Status.End_CleanUp)
       {
         ClearInventory();
       }
     }, true);
    }


    #region Load/Save , Intialization

    public void InitializeData(PermInventoryData data) => foundItems = new(data.foundItems);
    public PermInventoryData SaveData() => new PermInventoryData(foundItems.ToArray());

    public void InitializeTempData(TempInventoryData data)
    {
      foreach (ItemDataSlot slot in data.itemDataSlots)
      {
        ItemData itemData = ServiceLocator.Get<IItemGetter>().ReturnItemData(slot.iD);
        Item item = ItemFactory.CreateItem(itemData, slot.level);
        itemSlots.Add((item.iD, item.ReturnLevel()), new ItemSlot(item, slot.amount));
      }
    }

    public TempInventoryData SaveTempData()
    {
      List<ItemDataSlot> dataSlots = new();
      foreach (var slot in itemSlots)
      {
        dataSlots.Add(new ItemDataSlot(slot.Value.item.ReturnLevel(), slot.Value.item.iD, slot.Value.quantity));
      }

      return new TempInventoryData(dataSlots);
    }

    public void InputInventoryUI(InventoryUI uI) => inventoryUI = uI;

    #endregion

    #region Adding Items
    public void AddItem(ItemIdentification iD, ItemLevel itemLevel, int quantity)
    {
      var key = (iD, itemLevel);
      CreateIndicatorIfNew(key);

      if (itemSlots.ContainsKey(key))
        StackItem(key, quantity);
      else
        AddNewItem(key, quantity);



      InventoryAltered?.Invoke();
    }

    private void CreateIndicatorIfNew((ItemIdentification, ItemLevel) key)
    {
      if (!foundItems.Contains(key) && !itemSlots.ContainsKey(key))
        ServiceLocator.Get<IndicatorHandler>().SetUIChain(UIPanel.Book_Inventory);
    }

    private void StackItem((ItemIdentification, ItemLevel) key, int quantity)
    {
      itemSlots[key].quantity += quantity;

      if (inventoryUI != null)
      {
        if (inventoryUI.IsActive(key))
          inventoryUI.AlterUICount(key, itemSlots[key].quantity);
      }
    }

    private void AddNewItem((ItemIdentification, ItemLevel) key, int quantity)
    {
      var newItem = ItemFactory.CreateItem(ServiceLocator.Get<IItemGetter>().ReturnItemData(key.Item1), key.Item2);
      itemSlots[key] = new ItemSlot(newItem, quantity);

      if (inventoryUI != null && inventoryUI.gameObject.activeInHierarchy && ItemSlotOrganizer.ReturnIfFilterMatch(key.Item1, currentFilters))
      {
        FilterInventorySlots(false);
      }
    }

    #endregion

    #region Removing Items
    public bool RemoveItem((ItemIdentification, ItemLevel) key, int quantity)
    {
      bool isActiveItem = inventoryUI != null ? inventoryUI.IsActive(key) : false;

      if (key.Item2 == ItemLevel.Any)
        return RemoveAnyLevelItem(key.Item1, quantity, isActiveItem);
      // Check if item exists in inventory slots
      else if (!itemSlots.TryGetValue(key, out ItemSlot slot))
        return false;
      // Check if the quantity to be removed is greater than the quantity in the slot
      else if (slot.quantity < quantity)
        return false;
      else
      {
        RemoveItemAndUpdateUI(slot, quantity, isActiveItem, key);
        InventoryAltered?.Invoke();
        return true;
      }
    }

    private void RemoveItemAndUpdateUI(ItemSlot slot, int quantity, bool activeItem, (ItemIdentification, ItemLevel) key)
    {
      // Decrease the quantity
      slot.quantity -= quantity;

      if (slot.quantity == 0)
      {
        itemSlots.Remove(key);
        //If active item slot, or is a part of the active filter and if is exclusion-> filter ui
        if (activeItem || ItemSlotOrganizer.ReturnIfFilterMatch(key.Item1, currentFilters) && !ReturnIfExclusion(key, false))
          FilterInventorySlots(false);
      }
      else if (activeItem)
      {
        inventoryUI.AlterUICount(key, slot.quantity);
      }
    }

    private bool RemoveAnyLevelItem(ItemIdentification iD, int quantity, bool isActiveItem)
    {
      // Get the list of keys matching your target prefix, then sort them
      if (ReturnItemAmount((iD, ItemLevel.Any)) < quantity)
        return false;

      var sortedKeys = CreateAllItemOccurencesList(iD);

      foreach (var sortedKey in sortedKeys)
      {
        if (quantity <= 0)
          break;
        // Check if item exists in inventory slots
        if (!itemSlots.TryGetValue(sortedKey, out ItemSlot slot))
          continue;
        // Check if the quantity to be removed is greater than the quantity in the slot
        if (slot.quantity <= quantity)
        {
          // Decrease the quantity to be removed and remove the slot
          quantity -= slot.quantity;
          itemSlots.Remove(sortedKey);
          ReturnIfExclusion(sortedKey, false);
        }
        else
        {
          // decrease the item stack by the remaining quantity
          slot.quantity -= quantity;
          quantity = 0;
        }
      }

      // Re-filter the slots to update the UI
      if (inventoryUI.gameObject.activeInHierarchy && ItemSlotOrganizer.ReturnIfFilterMatch(iD, currentFilters))
        FilterInventorySlots(false);

      InventoryAltered?.Invoke();
      return true;
    }


    public void ClearInventory()
    {
      currentFilters = (ItemTypeFilter.AllTypes, ItemGradeFilter.AllGrades);
      itemSlots.Clear();
      ClearExclusion(false);
      inventoryUI?.FilterSlots(null, true);
    }

    #endregion

    #region Helpers
    private List<(ItemIdentification, ItemLevel)> CreateAllItemOccurencesList(ItemIdentification iD) => itemSlots.Keys
                            .Where(key => key.Item1 == iD)
                            .OrderBy(key => (int)key.Item2)
                            .ToList();

    public int ReturnItemAmount((ItemIdentification, ItemLevel) key)
    {
      // Check if level is 4, perform special operation
      if (key.Item2 == ItemLevel.Any)
      {
        var itemOccurences = CreateAllItemOccurencesList(key.Item1);
        if (itemOccurences.Count == 0 || itemOccurences == null) { return 0; }

        int amount = 0;
        foreach (var sortedKey in itemOccurences)
        {
          amount += itemSlots[sortedKey].quantity;
        }

        return amount;
      }
      else if (itemSlots.ContainsKey(key))
      {
        return itemSlots[key].quantity;
      }
      return 0;
    }

    public ItemSlot ReturnSlot((ItemIdentification, ItemLevel) key)
    {
      if (itemSlots.TryGetValue(key, out ItemSlot slot))
      {
        return slot;
      }
      return null;
    }

    #endregion

    #region Filter
    public void SetExclusion((ItemIdentification, ItemLevel) key)
    {
      if (exclusion == key) return;
      exclusion = key;
      FilterInventorySlots(false);
    }

    private bool ReturnIfExclusion((ItemIdentification, ItemLevel) key, bool tryFilter = true)
    {
      if (exclusion == key)
      {
        ClearExclusion(tryFilter);
        return true;
      }
      return false;
    }

    public void ClearExclusion(bool tryFilter = true)
    {
      var holder = exclusion;
      exclusion = (ItemIdentification.None, ItemLevel.None);
      if (tryFilter && itemSlots.ContainsKey(holder))
        FilterInventorySlots(false);
    }

    public void ApplyFilterChange(ItemTypeFilter typeFilter, ItemGradeFilter gradeFilter)
    {
      currentFilters = (typeFilter, gradeFilter);
      FilterInventorySlots(true);
    }

    public void FilterInventorySlots(bool isNewFilter)
    {
      if (inventoryUI == null || !inventoryUI.gameObject.activeInHierarchy) { return; }
      var filteredItems = ItemSlotOrganizer.OrganizeItemSlots(itemSlots.Where(pair => pair.Key != exclusion)
                                                                       .Select(pair => pair.Value)
                                                                       .ToList(), currentFilters);
      inventoryUI.FilterSlots(filteredItems, isNewFilter);
    }

    public ItemTypeFilter TypeFilter => currentFilters.Item1;
    public ItemGradeFilter GradeFilter => currentFilters.Item2;

    #endregion
  }


  [Serializable]
  public class ItemSlot
  {
    public Item item;
    public int quantity;

    public ItemSlot(Item item, int quantity)
    {
      this.item = item;
      this.quantity = quantity;
    }
  }

  [Serializable]
  public class TempInventoryData
  {
    public List<ItemDataSlot> itemDataSlots;

    public TempInventoryData(List<ItemDataSlot> itemDataSlots)
    {
      this.itemDataSlots = itemDataSlots;
    }

  }

  [Serializable]
  public class ItemDataSlot
  {
    public ItemLevel level;
    public ItemIdentification iD;

    public int amount;

    public ItemDataSlot(ItemLevel level, ItemIdentification iD, int amount)
    {
      this.level = level;
      this.iD = iD;
      this.amount = amount;
    }
  }

  [Serializable]
  public class PermInventoryData
  {
    public (ItemIdentification, ItemLevel)[] foundItems;
    public PermInventoryData((ItemIdentification, ItemLevel)[] found) => foundItems = found;

  }

}



