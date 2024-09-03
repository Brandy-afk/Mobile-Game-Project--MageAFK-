using System;
using MageAFK.Player;
using MageAFK.Management;

namespace MageAFK.Core
{
  public class LevelHandler : IData<LevelData>
  {
    //defaults
    private const int defaultRequiredXP = 1000;

    //Data
    private int currentLevel = 0;
    private int requiredXP = defaultRequiredXP;
    private int currentXPAmount = 0;

    //Events
    public event Action<int, int> XPChanged;
    public event Action<int> LevelChanged;

    public LevelHandler()
    {
      WaveHandler.SubToSiegeEvent((Status status) =>
        {
          if (status == Status.End_CleanUp)
          {
            ResetLevel();
          }
          else if (status == Status.Start)
            InvokeLevelChange();
        }, true);
    }


    #region  Load/Save
    public void InitializeData(LevelData data)
    {
      currentLevel = data.currentLevel;
      currentXPAmount = data.currentXP;
      requiredXP = data.requiredXP;

      InvokeLevelChange();
      InvokeXPChange();
    }

    public LevelData SaveData() => new LevelData(currentLevel, currentXPAmount, requiredXP);

    #endregion

    // XP setter, xp tracker, and level tracker  
    public void GiveExperience(int xp)
    {
      currentXPAmount += xp;
      ServiceLocator.Get<PlayerData>().AddStatValue(PlayerStatisticEnum.Experience, xp);
      ServiceLocator.Get<SiegeStatisticTracker>().ModifiyMetric(PlayerStatisticEnum.Experience, xp);
      while (currentXPAmount >= requiredXP)
      {
        OnLevelUp();
      }
      InvokeXPChange();
    }

    private void OnLevelUp()
    {
      currentLevel++;
      currentXPAmount -= requiredXP;
      float floatXP = requiredXP;
      requiredXP = (int)(floatXP * 1.1f);

      InvokeLevelChange();

      ServiceLocator.Get<LevelProgressHandler>().OnLevelUp(currentLevel);
    }

    // Method for subscription
    #region Events
    //TODO: issue with how scripts call into this. Issue would occur upon user playing the game twice, things would blink up as new.
    public void SubscribeToLevelChanged(Action<int> handler, bool state)
    {
      if (state)
      {
        LevelChanged += handler;
        InvokeLevelChange();
      }
      else
        LevelChanged -= handler;
    }
    public void SubscribeToXPChanged(Action<int, int> handler, bool state)
    {
      if (state)
      {
        XPChanged += handler;
        InvokeXPChange();
      }
      else
        XPChanged -= handler;
    }
    private void InvokeLevelChange() => LevelChanged?.Invoke(currentLevel);
    private void InvokeXPChange() => XPChanged?.Invoke(currentXPAmount, requiredXP);

    #endregion

    #region Helpers
    public int ReturnCurrentLevel() => currentLevel;
    private void ResetLevel()
    {
      currentLevel = 0;
      currentXPAmount = 0;
      requiredXP = defaultRequiredXP;
      InvokeLevelChange();
    }
    #endregion

  }

  [Serializable]
  public class LevelData
  {
    public int currentLevel;
    public int currentXP;
    public int requiredXP;


    public LevelData(int currentLevel, int currentXP, int requiredXP)
    {
      this.currentLevel = currentLevel;
      this.currentXP = currentXP;
      this.requiredXP = requiredXP;
    }

    public LevelData()
    {

    }

  }
}
