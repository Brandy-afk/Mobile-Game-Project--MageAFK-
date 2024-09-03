using MageAFK.UI;
using MageAFK.Management;
using System.Collections.Generic;
using System;
using MageAFK.Core;

namespace MageAFK.Player
{
  public class PlayerData : IData<PlayerDataCollection>
  {
    private StatsUI statsUI;

    public void InputStatUI(StatsUI statsUI) => this.statsUI = statsUI;

    // Dictionaries to store values
    private Dictionary<PlayerStatisticEnum, float> stats = new();

    private Dictionary<PlayerStatisticEnum, Action> statEvents = new();


    // For initialization and saving, adjust the PlayerDataCollection accordingly:
    public void InitializeData(PlayerDataCollection data)
    {
      stats = new Dictionary<PlayerStatisticEnum, float>(data.floatStatsData);
    }

    public PlayerDataCollection SaveData()
    {
      return new PlayerDataCollection(stats);
    }


    // Methods to manage stats
    public void UpdateAllStatsToUI()
    {
      foreach (var item in stats)
        statsUI.UpdateStatUI(item.Key, (int)item.Value);
    }


    public void AddStatValue(PlayerStatisticEnum stat, float value)
    {
      if (stats.ContainsKey(stat))
        stats[stat] += value;
      else
        stats[stat] = value;

      InvokeStatEvent(stat);

      if (WaveHandler.WaveState == WaveState.None)
        SaveManager.Save(SaveData(), DataType.PlayerData);

      if (statsUI == null || !statsUI.gameObject.activeInHierarchy) { return; }
      statsUI.UpdateStatUI(stat, (int)stats[stat]);
    }

    // Assuming profileUI.UpdateStat takes an int as the second parameter.
    // Adjust the code if this is not the case.
    public float GetStatValue(PlayerStatisticEnum stat)
    {
      return stats.TryGetValue(stat, out float value) ? value : 0f;
    }

    private void InvokeStatEvent(PlayerStatisticEnum stat)
    {
      if (statEvents.ContainsKey(stat))
      {
        statEvents[stat]?.Invoke();
      }
    }

    public void SubscribeToStatAltered(PlayerStatisticEnum stat, Action callback, bool state)
    {
      if (!statEvents.ContainsKey(stat))
        statEvents[stat] = null;

      if (state)
      {
        statEvents[stat] += callback;
        callback.Invoke();
      }
      else
        statEvents[stat] -= callback;

    }


  }

  [System.Serializable]
  public class PlayerDataCollection
  {
    public Dictionary<PlayerStatisticEnum, float> floatStatsData;

    public PlayerDataCollection(Dictionary<PlayerStatisticEnum, float> floatStatsData)
    {
      this.floatStatsData = floatStatsData;
    }

    public PlayerDataCollection() { }
  }

  public enum PlayerStatisticEnum
  {
    Kills,
    Silver,
    Gems,
    Damage,
    SpellCasts,
    PotionsDrank,
    Waves,
    Sieges,
    SkillPoints,
    MilestonesComplete,
    Experience,
    SkillsMaxed,
    ItemsCrafted,
    ItemsUpgraded,
    UpgradesFailed,
    ItemsFed,
    SpellsUpgraded,
    SpellsUnlocked,
    DamageTaken,
    BossesKilled,
    FoodConsumed,
    UltimateUsed,
    Gold,
    ItemsGained
    // ... add others as needed
  }


}
