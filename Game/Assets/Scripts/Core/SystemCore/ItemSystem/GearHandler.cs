using System.Collections.Generic;
using System.Linq;
using MageAFK.Management;
using MageAFK.UI;
using System;
using MageAFK.Core;

namespace MageAFK.Items
{
  public class GearHandler : IData<GearData>
  {
    private readonly Dictionary<ItemType, Armour> slots = new Dictionary<ItemType, Armour>()
    {
      {ItemType.Headgear, null},
      {ItemType.Torso, null},
      {ItemType.Jewelry, null}
    };

    private GearUI gearUI;

    public GearHandler()
    {
      WaveHandler.SubToSiegeEvent((Status status) =>
     {
       if (status == Status.End_CleanUp)
       {
         ClearGearSlots();
       }
     }, true);
    }


    #region Load/Save
    public void InitializeData(GearData data)
    {
      foreach (var itemPair in data.slots)
      {
        if (slots.ContainsKey(itemPair.Item1))
        {
          slots[itemPair.Item1] = (Armour)ItemFactory.CreateItem(ServiceLocator.Get<IItemGetter>().ReturnItemData(itemPair.Item2),
                                                                 itemPair.Item3);
          slots[itemPair.Item1].ToggleGear(true);
        }
      }
    }

    public GearData SaveData()
    {
      return new GearData(slots.Where(pair => pair.Value != null)
                               .Select(pair => (pair.Key, pair.Value.iD, pair.Value.level))
                               .ToArray());
    }

    #endregion


    public void SetGearUI(GearUI gearUI)
    {
      this.gearUI = gearUI;
      foreach (var slot in slots)
        gearUI.UpdateUI(slot.Key, slot.Value);
    }

    #region Class Methods
    public bool TryEquippingGear(ItemType type)
    {
      var item = ServiceLocator.Get<IDragInfo<Item, (ItemIdentification, ItemLevel)>>().Drag;
      var itemData = ServiceLocator.Get<IItemGetter>().ReturnItemData(item.iD);
      if (itemData.mainType != type)
      {
        UnityEngine.Debug.Log("Cant Equip: Item main type does not match the required type.");
        return false;
      }
      else if (WaveHandler.WaveState == WaveState.Wave)
      {
        UnityEngine.Debug.Log("Cant Equip: Cannot equip during a wave.");
        return false;
      }
      else if (ReturnIfItemIsEquipped(item, type))
      {
        UnityEngine.Debug.Log("Cant Equip: Item is already equipped.");
        return false;
      }
      else if (!EquipGear(type, item as Armour))
      {
        UnityEngine.Debug.Log("Cant Equip: Failed to equip the item.");
        return false;
      }
      else
      {
        return true;
      }
    }

    private bool EquipGear(ItemType type, Armour item)
    {
      if (!slots.ContainsKey(type) || item is null) return false;
      if (slots[type] != null) { UnequipGear(type); }

      item.ToggleGear(true);

      if (!ServiceLocator.Get<InventoryHandler>().RemoveItem((item.iD, item.ReturnLevel()), 1)) return false;
      slots[type] = item;
      if (gearUI != null) gearUI.UpdateUI(type, item);
      return true;
    }

    public bool UnequipGear(ItemType type, bool NotSwap = false)
    {
      if (!slots.ContainsKey(type) || slots[type] == null) return false;

      slots[type].ToggleGear(false);

      ServiceLocator.Get<InventoryHandler>().AddItem(slots[type].iD, slots[type].ReturnLevel(), 1);
      slots[type] = null;

      if (NotSwap && gearUI != null) { gearUI.UpdateUI(type, null); }
      return true;
    }

    public bool ReturnIfItemIsEquipped(Item item, ItemType type) => slots.ContainsKey(type) && slots[type] == item;

    private void ClearGearSlots()
    {
      List<ItemType> types = new List<ItemType>
      {
        ItemType.Headgear,
        ItemType.Jewelry,
        ItemType.Torso
      };

      foreach (var type in types)
      {
        slots[type] = null;
        if (gearUI == null) continue;
        gearUI.UpdateUI(type, null);
      }
    }


    public List<(ItemIdentification iD, IPrefabReturner prefab)> ReturnPoolableGear()
    {
      List<(ItemIdentification, IPrefabReturner)> l = new();
      foreach (var item in slots.Values)
        if (ServiceLocator.Get<IItemGetter>().ReturnItemData(item.iD) is IPrefabReturner prefabReturner)
          l.Add((item.iD, prefabReturner));
      return l;
    }

    public Item ReturnItem(ItemType type)
    {
      try
      {
        return slots[type];
      }
      catch (KeyNotFoundException)
      {
        return null;
      }
    }


    #endregion

  }



  [Serializable]
  public class GearData
  {
    public (ItemType, ItemIdentification, ItemLevel)[] slots { get; set; }
    public GearData((ItemType, ItemIdentification, ItemLevel)[] slots)
    {
      this.slots = slots;
    }
  }

}


