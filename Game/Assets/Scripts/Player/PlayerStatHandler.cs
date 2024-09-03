using System;
using System.Collections.Generic;
using MageAFK.Stats;
using MageAFK.Management;
using System.Linq;
using MageAFK.Core;

namespace MageAFK.Player
{
  public class PlayerStatHandler : StatHandlerBase, IData<StatData>
  {

    private Dictionary<Stat, Action> statEvents = new();

    #region Load / Save
    //Perm Stats
    public void InitializeData(StatData data) => permStatValues = new Dictionary<Stat, float>(data.stats);
    public StatData SaveData() => new StatData(permStatValues);
    //Load Temp Values
    public void LoadTempValues(Dictionary<Stat, float> tempValues)
    {
      tempStatValues = new Dictionary<Stat, float>(tempValues);

      foreach (var key in tempStatValues.Keys)
        InvokeStatEvent(key);
    }
    public StatData SaveTempData() => new StatData(tempStatValues);

    #endregion

    //Value modification and get
    public override void ModifyStat(Stat stat, float amount, bool isPerm)
    {
      base.ModifyStat(stat, amount, isPerm);
      InvokeStatEvent(stat);

      if (WaveHandler.WaveState == WaveState.None)
        SaveManager.Save(SaveData(), DataType.PermPlayerStat);
    }

    #region Helpers and Events

    public void SubscribeToStatEvent(Stat stat, Action callback, bool state)
    {
      if (!statEvents.ContainsKey(stat))
        statEvents[stat] = null;

      if (state)
      {
        statEvents[stat] += callback;
        InvokeStatEvent(stat);
      }
      else
        statEvents[stat] -= callback;
    }

    private void InvokeStatEvent(Stat stat)
    {
      if (statEvents.ContainsKey(stat) && statEvents[stat] != null)
        statEvents[stat]?.Invoke();
    }

    protected override void OnSiegeEnd()
    {
      Stat[] stats = tempStatValues.Select(pair => pair.Key).ToArray();
      tempStatValues.Clear();
      for (int i = 0; i < stats.Length; i++) InvokeStatEvent(stats[i]);
    }

    #endregion
  }
}


namespace MageAFK.Stats
{
  [Serializable]
  public class StatData
  {
    public Dictionary<Stat, float> stats;

    public StatData(Dictionary<Stat, float> stats)
    {
      this.stats = stats;
    }

    public StatData()
    {

    }

  }
}
