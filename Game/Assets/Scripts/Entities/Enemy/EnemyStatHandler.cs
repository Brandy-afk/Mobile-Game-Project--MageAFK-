using System.Collections.Generic;
using System.Linq;
using MageAFK.Core;
using MageAFK.Management;
using MageAFK.Stats;



namespace MageAFK.AI
{
  public class EnemyStatHandler : StatHandlerBase, IData<StatData>
  {

    #region Save / Load
    //Perm Stats
    public void InitializeData(StatData data) => permStatValues = new Dictionary<Stat, float>(data.stats);
    public StatData SaveData() => new StatData(permStatValues);
    public void LoadTempValues(Dictionary<Stat, float> tempValues)
    {
      tempStatValues = tempValues;
      foreach (var pair in tempValues)
        ServiceLocator.Get<EntityDataManager>().UpdateNPEntityStats(pair.Key);
    }

    public StatData SaveTempData() => new StatData(tempStatValues);
    #endregion

    public override void ModifyStat(Stat stat, float amount, bool isPerm)
    {
      base.ModifyStat(stat, amount, isPerm);
      ServiceLocator.Get<EntityDataManager>().UpdateNPEntityStats(stat);

      if (WaveHandler.WaveState == WaveState.None)
        SaveManager.Save(SaveData(), DataType.PermEnemyStat);
    }

    protected override void OnSiegeEnd()
    {
      Stat[] stats = tempStatValues.Select(pair => pair.Key).ToArray();
      tempStatValues.Clear();
    }
  }
}
