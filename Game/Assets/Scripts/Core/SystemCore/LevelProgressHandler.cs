using System;
using MageAFK.Management;
using MageAFK.Player;
using MageAFK.Stats;

namespace MageAFK.Core
{
  public class LevelProgressHandler : IData<(float, int)>
  {
    public event Action OnValueAltered;

    //Leveling up damage increase, NEEDS TO BE IN DECIMAL (ex .2 = 20%)
    public float damagePerlevel { get; private set; } = 0.05f;
    public int skillPointMod { get; private set; } = 5;


    public void InitializeData((float, int) data)
    {
      SetSkillPointMod(data.Item2);
      damagePerlevel = data.Item1;

      OnValueAltered?.Invoke();
    }
    public (float, int) SaveData() => (damagePerlevel, skillPointMod);


    public void OnLevelUp(int level)
    {
      ServiceLocator.Get<PlayerStatHandler>().ModifyStat(Stat.Damage, damagePerlevel, false);
      if (level % skillPointMod == 0)
        ServiceLocator.Get<CurrencyHandler>().AddCurrency(CurrencyType.SkillPoints, 1);
    }


    public void IncreaseDamagePerLevel(float value)
    {
      ServiceLocator.Get<PlayerStatHandler>().ModifyStat(Stat.Damage, -(damagePerlevel * ServiceLocator.Get<LevelHandler>().ReturnCurrentLevel()), false);
      damagePerlevel += value;
      ServiceLocator.Get<PlayerStatHandler>().ModifyStat(Stat.Damage, damagePerlevel * ServiceLocator.Get<LevelHandler>().ReturnCurrentLevel(), false);
      OnValueAltered?.Invoke();

      if (WaveHandler.WaveState == WaveState.None)
        SaveManager.Save(SaveData(), DataType.LevelUpgradeData);
    }

    public void SetSkillPointMod(int sp)
    {
      skillPointMod = sp;
      OnValueAltered?.Invoke();

      if (WaveHandler.WaveState == WaveState.None)
        SaveManager.Save(SaveData(), DataType.LevelUpgradeData);
    }

  }

  [Serializable]
  public class LevelProgressData
  {



  }
}