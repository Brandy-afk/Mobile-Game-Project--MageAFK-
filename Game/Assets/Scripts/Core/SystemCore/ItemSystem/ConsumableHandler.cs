
using System.Collections.Generic;
using System;
using MageAFK.Management;
using System.Linq;

namespace MageAFK.Items
{
    //TODO add to service locator
    public class ConsumableHandler : IData<ConsumableTaskData>
  {

    private Dictionary<int, ConsumableTask> consumables;

    public void InitializeData(ConsumableTaskData data)
    {
      // foreach (ConsumableTask task in data.consumableTasks)
      // {
      //   AddTimeTask(task.itemID, task.timeRemaining, task);
      // }
    }

    public ConsumableTaskData SaveData()
    {
      if (consumables.Count <= 0) { return null; }
      return new ConsumableTaskData(consumables.Values.ToList());
    }





    public void AddConsumableTask(Consumable consumable)
    {
      //   if (consumable == null) { return; }
      //   if (consumables.Count >= CONSUMABLE_SLOTS) { return; }
      //   if (consumables == null) { consumables = new Dictionary<int, ConsumableTask>(); }

      //   TimeSpan timespan = TimeSpan.FromMinutes(consumable.duration);
      //   ServiceLocator.Get<InventoryHandler>().RemoveItem(consumable.key, 1);
      //   StatTrait.AlterEntityStats(consumable.ReturnTraits(), true);
      //   AddTimeTask(consumable.iD, timespan, new ConsumableTask(consumable.iD, consumable.ReturnTraits()));
      // }

      // public void AddTimeTask(ItemIdentification iD, TimeSpan span, ConsumableTask task)
      // {
      //   int timeKey = ServiceLocator.Get<TimeTaskHandler>().GetUniqueTimeKey();
      //   // TimeTaskHandler.Instance.AddTimeTask(timeKey, span, () => OnDurationOver(timeKey));
      //   consumables.Add(timeKey, task);
    }



    // public void OnDurationOver(int key)
    // {
    //   ItemStatTrait.AlterEntityStats(consumables[key].traits, false);
    // }
  }

  [Serializable]
  public class ConsumableTask
  {
    public ItemIdentification itemID;

    public TimeSpan timeRemaining;

    public ItemStatTrait[] traits;

    public ConsumableTask(ItemIdentification id, ItemStatTrait[] traits)
    {
      itemID = id;
      this.traits = traits;
    }

  }

  [Serializable]
  public class ConsumableTaskData
  {
    public List<ConsumableTask> consumableTasks;

    public ConsumableTaskData(List<ConsumableTask> data)
    {
      consumableTasks = data;
    }
  }


}