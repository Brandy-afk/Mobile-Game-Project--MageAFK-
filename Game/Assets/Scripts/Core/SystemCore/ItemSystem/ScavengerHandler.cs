
using System.Collections.Generic;
using MageAFK.UI;
using System.Linq;
using MageAFK.Management;
using MageAFK.Core;

namespace MageAFK.Items
{
  public class ScavengerHandler
  {

    private readonly Dictionary<(ItemIdentification, ItemLevel), int> slots = new();
    private ScavengerUI scavengerUI;


    public ScavengerHandler()
    {
      WaveHandler.SubToWaveState(OnWaveStateChanged, true);
      WaveHandler.SubToSiegeEvent((Status status) =>
        {
          if (status == Status.End_CleanUp)
          {
            ClearList();
          }
        }, true);
    }

    private void OnWaveStateChanged(WaveState state)
    {
      switch (state)
      {
        case WaveState.Intermission:
          if (slots.Count > 0) LootScavengedItems(true);
          break;
        case WaveState.None:
          if (slots.Count > 0) LootScavengedItems(false);
          break;
      }
    }

    public void InputScavengerUI(ScavengerUI scavengerUI) => this.scavengerUI = scavengerUI;


    public void AddScavengedItem(ItemIdentification iD, ItemLevel itemLevel, int quantity)
    {
      var key = (iD, itemLevel);

      slots.TryAdd(key, 0);
      slots[key] += quantity;

      if (scavengerUI == null || !scavengerUI.gameObject.activeInHierarchy) return;
      if (scavengerUI.ReturnIfActiveSlot(key))
        scavengerUI.UpdateItemToUI(key, slots[key]);
      else
        FilterSlots();
    }

    public void LootScavengedItems(bool state)
    {
      if (slots.Count <= 0) return;
      scavengerUI?.ClearObjects();

      if (state)
      {
        foreach (var item in slots)
        {
          ServiceLocator.Get<InventoryHandler>().AddItem(item.Key.Item1, item.Key.Item2, item.Value);
          ServiceLocator.Get<SiegeStatisticTracker>().ModifiyMetric(Player.PlayerStatisticEnum.ItemsGained, item.Value);
        }
      }
      ClearList();
    }

    public void UpdateUI() => FilterSlots();

    private void FilterSlots()
    {
      if (slots.Count <= 0) return;
      scavengerUI.FilterSlots(ItemSlotOrganizer.OrderKeys(slots.Keys.ToList(), (ItemTypeFilter.AllTypes, ItemGradeFilter.AllGrades)));
    }

    public void ClearList() => slots.Clear();

    public int ReturnAmount((ItemIdentification, ItemLevel) key)
    {
      if (slots.TryGetValue(key, out int slot))
      {
        return slot;
      }
      return 0;
    }
  }


}