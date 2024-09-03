
using System.Collections.Generic;
using MageAFK.Core;
using MageAFK.Management;
using UnityEngine;



namespace MageAFK.UI
{
  public class HistoryUI : MonoBehaviour
  {
    [SerializeField] private HistorySlotUIController[] slotControllers;
    [SerializeField] private HistoryPopUp historyPopUp;
    [SerializeField] private HistoryData data;
    private Queue<HistorySlotUIController> historyQue;

    private void Awake()
    {
  
      data.SubToDataEvent((SiegeStatistic stat) => AddTab(stat));

      historyQue = new Queue<HistorySlotUIController>();
      foreach (var content in data.history)
        AddTab(content);
    }

    private void OnDataAltered(SiegeStatistic statistic) => AddTab(statistic);

    private void AddTab(SiegeStatistic stats)
    {
      // Try to dequeue a slot
      HistorySlotUIController controller = historyQue.Count >= data.MAX_SLOTS ? historyQue.Dequeue() : null;

      // If there was no slot to dequeue, get an unused one.
      if (controller == null)
      {
        controller = ReturnNewSlot();
        if (controller == null)
        {
          Debug.LogError("All slots are in use and queue is not yet full. Something went wrong.");
          return;
        }
        controller.gameObject.SetActive(true);
      }

      // Build and queue the slot
      controller.BuildSlot(stats);
      controller.transform.SetAsFirstSibling();
      historyQue.Enqueue(controller);
    }

    private HistorySlotUIController ReturnNewSlot()
    {
      foreach (var slotUI in slotControllers)
      {
        if (!slotUI.active)
        {
          return slotUI;
        }
      }
      return null;
    }


  }

  [System.Serializable]
  public class SiegeHistoryData
  {
    public List<SiegeStatistic> historyStats;

    public SiegeHistoryData(List<SiegeStatistic> historyStats)
    {
      this.historyStats = historyStats;
    }

    public SiegeHistoryData()
    {

    }

  }

}

