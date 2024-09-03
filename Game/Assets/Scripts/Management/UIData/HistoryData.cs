using System.Collections.Generic;
using System.Linq;
using MageAFK.Core;
using MageAFK.UI;
using UnityEngine;

namespace MageAFK.Management
{
  [CreateAssetMenu(fileName = "HistoryData", menuName = "UIData/HistoryData", order = 0)]
  public class HistoryData : UIData<SiegeHistoryData, SiegeStatistic>
  {
    public Queue<SiegeStatistic> history;
    public int MAX_SLOTS = 10;

    private void OnEnable() => WaveHandler.SubToSiegeEvent(OnSiegeEvent, true, Priority.Last);

    private void OnSiegeEvent(Status state)
    {
      if (state == Status.End)
      {
        var content = ServiceLocator.Get<SiegeStatisticTracker>().BuildSiegeStats();
        if (history.Count >= MAX_SLOTS)
          history.Dequeue();
        history.Enqueue(content);
        InvokeDataAltered(content);
        UIController.Instance.Save<SiegeHistoryData>(DataType.SiegeHistoryData);
      }
    }

    public override void InitializeData(SiegeHistoryData data)
    {
      if (data.historyStats == null) return;
      history = new Queue<SiegeStatistic>();
      foreach (var content in data.historyStats)
        history.Enqueue(content);
    }

    public override SiegeHistoryData SaveData() => new SiegeHistoryData(history.ToList());

  }
}