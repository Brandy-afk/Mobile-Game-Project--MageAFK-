
using System.Collections.Generic;
using MageAFK.Management;
using MageAFK.Player;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MageAFK.Core
{

    [CreateAssetMenu(fileName = "New Milestone", menuName = "Milestones")]
  public class Milestone : ScriptableObject
  {
    #region Variables
    [TabGroup("Info")] public MilestoneID iD = MilestoneID.None;
    [TabGroup("Info")] public string title;
    [TabGroup("Info")] public string desc;

    [TabGroup("Info"), ReadOnly] public int rankCap = 4;
    [TabGroup("Info")] public float[] goalValues = new float[4];
    [TabGroup("Info"), Title("Only Gems, Gold, and Recipe Rewards")] public RewardLevel[] rewardValues;

    //Data fields
    [ReadOnly, TabGroup("Data")] public int rank = 0;
    [ReadOnly, TabGroup("Data")] public bool isMaxed;
    [ReadOnly, TabGroup("Data")] public float currentValue;
    public List<Reward[]> rewardPool = new();

    #endregion

    #region Func
    public bool AdvanceValue(float value)
    {
      currentValue += value;


      if (isMaxed) { return false; }
      if (currentValue >= goalValues[rank])
      {
        float ext = currentValue - goalValues[rank];
        return AdvanceMileStone(ext);
      }
      return false;
    }

    private bool AdvanceMileStone(float extraValue)
    {
      rewardPool.Add(rewardValues[rank].rewards);
      rank += 1;

      if (rank == rankCap)
      {
        isMaxed = true;
        ServiceLocator.Get<PlayerData>().AddStatValue(PlayerStatisticEnum.MilestonesComplete, 1);
      }

      currentValue = 0;
      currentValue += extraValue;
      return true;
    }

    public void ClaimNextReward()
    {
      if (!CheckRewardPoolSize()) return;

      foreach (Reward reward in rewardPool[0])
      {
        reward.GiveReward();
      }

      ServiceLocator.Get<CurrencyHandler>().AddCurrency(CurrencyType.SkillPoints, 1);
      rewardPool.RemoveAt(0);
    }

    public bool CheckRewardPoolSize() => rewardPool.Count > 0;

    #endregion

    #region Data
    //TODO: Testing method
    public void ResetMilestone()
    {
      currentValue = 0;
      rank = 0;
      isMaxed = false;
      rewardPool.Clear();
    }

    public void LoadData(MilestoneDataFields fields)
    {
      rank = fields.rank;
      isMaxed = fields.isMaxed;
      currentValue = fields.currentValue;

      for (int i = rank - fields.rewardPoolSize; i < rank; i++)
        rewardPool.Add(rewardValues[i].rewards);
    }

    public MilestoneDataFields SaveData() => new MilestoneDataFields(iD, rank, isMaxed, currentValue, rewardPool.Count);

    #endregion
  }

  public enum MilestoneID
  {
    None = -1,
    KillEnemies = 0,
    ClickOnWizard = 1,
    FinishWaveWithSameBook = 2,
    DoDamage = 3,
    TakeDamage = 4,
    CraftItems = 5,
    BlockDamage = 6,
    OneShotEnemies = 7,
    CorruptionKills = 8,
    KillSlowedEnemies = 9,
    StackBleed = 10,
    FearFearedEnemies = 11,
    UltimateKills = 12,
    CollectSilver = 13,
    PurchaseElixers = 14,
    CompleteWavesInWoods = 15,
    CompleteWavesInDesert = 16,
    CompleteWavesInMountains = 17,
    FeedItems = 18,
    MaxOutSpells = 19,
    FinishWavesWithoutTakingDamage = 20,
  }



}