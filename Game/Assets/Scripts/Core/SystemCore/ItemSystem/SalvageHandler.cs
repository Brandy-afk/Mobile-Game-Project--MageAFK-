
using System.Collections.Generic;
using MageAFK.Core;
using UnityEngine;
using MageAFK.Player;
using MageAFK.Management;
using System.Linq;

namespace MageAFK.Items
{
  public class SalvageHandler
  {
    private readonly float[] levelIncreases = { 0f, .25f, .75f, 1.25f };

    public bool SalvageItems(Item item, int amount, out List<Reward> rewards)
    {
      if (ServiceLocator.Get<InventoryHandler>().RemoveItem((item.iD, item.ReturnLevel()), amount))
      {
        ServiceLocator.Get<PlayerData>().AddStatValue(PlayerStatisticEnum.ItemsFed, amount);

        rewards = RollRewards(item, amount);
        return true;
      }
      rewards = null;
      return false;
    }

    private List<Reward> RollRewards(Item item, int amount)
    {
      ItemData itemData = ServiceLocator.Get<IItemGetter>().ReturnItemData(item.iD);
      SalvageGradeInformation information = ServiceLocator.Get<GradeStatHandler>().ReturnSalvageInformation(itemData.grade);

      Dictionary<ItemIdentification, Reward> itemMap = new();
      Dictionary<RewardType, Reward> valueMap = new();

      bool isFeed = itemData.ReturnIfType(ItemType.Oblation);
      for (int i = 0; i < amount; i++)
      {
        float randomChance = UnityEngine.Random.Range(0, 100f);
        float cumulativeChance = 0;
        for (int c = 0; c < information.potentialRewards.Length; c++)
        {
          cumulativeChance += information.potentialRewards[c].ReturnChance(isFeed);
          if (randomChance <= cumulativeChance)
          {
            if (information.potentialRewards[c].type == RewardType.Gems && !information.gemsUnlocked
            || information.potentialRewards[c].type == RewardType.Items && itemData.mainType == ItemType.Part) { continue; }
            GetReward(information.potentialRewards[c], item.ReturnLevel(), itemData.grade, valueMap, itemMap, isFeed);
            break;
          }
        }
      }
      return SortRewards(itemMap, valueMap);
    }

    private List<Reward> SortRewards(Dictionary<ItemIdentification, Reward> itemMap, Dictionary<RewardType, Reward> valueMap)
    {
      var items = itemMap.Values.ToList();
      var values = valueMap.Values.ToList();
      values.AddRange(items);
      values.OrderBy(reward => reward.rewardType);
      return values;
    }

    private void GetReward(RewardInfo info, ItemLevel level, ItemGrade grade,
    Dictionary<RewardType, Reward> values, Dictionary<ItemIdentification, Reward> items, bool isFeed)
    {
      int amount = GetRewardAmount(info, level, isFeed);

      switch (info.type)
      {
        case RewardType.Experience:
          if (values.ContainsKey(info.type)) (values[info.type] as XPReward).amount += amount;
          else values[info.type] = new XPReward(amount);
          break;

        case RewardType.Silver:
          if (values.ContainsKey(info.type)) (values[info.type] as CurrencyReward).amount += amount;
          else values[info.type] = new CurrencyReward(CurrencyType.SilverCoins, amount, RewardType.Silver);
          break;

        case RewardType.Gems:
          if (values.ContainsKey(info.type)) (values[info.type] as CurrencyReward).amount += amount;
          else values[info.type] = new CurrencyReward(CurrencyType.DemonicGems, amount, RewardType.Gems);
          break;

        case RewardType.Items:
          ItemData randomItem = ServiceLocator.Get<IItemGetter>().ReturnRandomItem(ItemType.Part, (ItemGrade)Mathf.Clamp((int)grade - 1, 1, 4));
          AddItemReward(items, randomItem, amount);
          break;

        case RewardType.Recipe:
          AddRecipeReward(items);
          break;

        default:
          break;
      }
    }

    private void AddItemReward(Dictionary<ItemIdentification, Reward> items, ItemData data, int amount)
    {
      if (items.TryGetValue(data.iD, out Reward reward))
        (reward as ItemReward).amount += amount;
      else
        items[data.iD] = new ItemReward(data, ItemLevel.None, amount);
    }

    private void AddRecipeReward(Dictionary<ItemIdentification, Reward> items)
    {
      var inRewards = items.Where(pair => pair.Value.rewardType == RewardType.Recipe)
                           .Select(pair => pair.Key)
                           .ToArray();

      Recipe newRecipe = ServiceLocator.Get<RecipeHandler>().ReturnRandomVoidRecipe(inRewards);
      if (newRecipe == null) { return; }

      items.Add(newRecipe.ReturnID(), new RecipeReward(newRecipe));
    }

    private int GetRewardAmount(RewardInfo info, ItemLevel level, bool isFeed)
    {
      int amount = UnityEngine.Random.Range(info.min, info.max);
      var mod = (level != ItemLevel.None ? levelIncreases[(int)level] : 0) + (
        isFeed ? ServiceLocator.Get<GradeStatHandler>().FeedAmountMultiplier : 0) + 1;
      return (int)(amount * mod);
    }
  }

}
