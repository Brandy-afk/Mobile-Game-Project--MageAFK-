using System.Collections.Generic;
using MageAFK.Core;
using UnityEngine;
using MageAFK.Player;
using MageAFK.Management;

namespace MageAFK.Items
{
    public class UpgradeHandler
  {
    private int currentIndex = 0;
    int[] increments;
    private const int INCREMENT_PERCENTAGE = 10;


    #region Set up
    public void CreateNewValues(Item item)
    {
      var grade = ServiceLocator.Get<IItemGetter>().ReturnItemData(item.iD).grade;
      UpgradeGradeInfo info = ServiceLocator.Get<GradeStatHandler>().ReturnUpgradeInformation(grade);

      int rank = (int)item.ReturnLevel();
      int stage1Cost = rank == 0 ? info.upgradeCosts[0].stage1Cost : rank == 1 ? info.upgradeCosts[1].stage1Cost : info.upgradeCosts[2].stage1Cost;
      int stage2Cost = rank == 0 ? info.upgradeCosts[0].stage2Cost : rank == 1 ? info.upgradeCosts[1].stage2Cost : info.upgradeCosts[2].stage2Cost;
      CalculateIncrements(stage1Cost, stage2Cost);
    }

    private void CalculateIncrements(int stage1Cost, int stage2Cost)
    {
      const int STAGE1_THRESHOLD = 7; // 5% * 15 = 75%
      const int STAGE2_THRESHOLD = 3;  // 5% * 5 = 25%

      int[] increments = new int[(100 / INCREMENT_PERCENTAGE)];

      for (int i = 0; i < STAGE1_THRESHOLD; i++)
      {
        increments[i] = stage1Cost;
      }
      for (int i = STAGE1_THRESHOLD; i < STAGE1_THRESHOLD + STAGE2_THRESHOLD; i++)
      {
        increments[i] = stage2Cost;
      }

      this.increments = increments;
    }

    #endregion

    #region Update

    public (int, int) ChangeIndex(int displayIndex)
    {
      currentIndex = displayIndex - 1;
      return (ReturnCost(), ReturnChance());
    }

    private int ReturnCost()
    {
      int sum = 0;
      for (int i = 0; i <= currentIndex; i++)
        sum += increments[i];
      return sum;
    }

    private int ReturnChance()
    {
      int chance = 0;
      for (int i = 0; i <= currentIndex; i++)
        chance += INCREMENT_PERCENTAGE;
      return chance;
    }
    #endregion

    #region Confirmation
    public bool UpgradeItem(Item item, out List<Reward> rewards, out bool isFail)
    {
      rewards = new();
      isFail = true;

      if (ServiceLocator.Get<CurrencyHandler>().SubtractCurrency(CurrencyType.SilverCoins, ReturnCost()))
      {
        ItemData data = ServiceLocator.Get<IItemGetter>().ReturnItemData(item.iD);
        if ((ReturnChance() >= Random.Range(0, 100)) && ServiceLocator.Get<InventoryHandler>().RemoveItem((item.iD, item.ReturnLevel()), 1))
          isFail = false;

        rewards.Add(new ItemReward(data, isFail ? item.ReturnLevel() : (ItemLevel)((int)item.ReturnLevel() + 1), 1));
        ServiceLocator.Get<PlayerData>().AddStatValue(isFail ? PlayerStatisticEnum.UpgradesFailed : PlayerStatisticEnum.ItemsUpgraded, 1);
        return true;
      }

      return false;
    }

    #endregion

  }
}
