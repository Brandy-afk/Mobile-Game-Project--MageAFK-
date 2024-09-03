using MageAFK.Items;
using MageAFK.Management;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MageAFK.Core
{



  public enum RewardType
  {
    Silver = 0,
    Gems = 1,
    Experience = 2,
    Gold = 3,
    Items = 4,
    Recipe = 5,
  }

  [System.Serializable]
  public abstract class Reward
  {
    public RewardType rewardType;

    public int amount;
    public abstract void GiveReward();
    public abstract int ReturnAmount();
  }

  [System.Serializable]
  public class RewardLevel
  {
    [SerializeReference, HideReferenceObjectPicker]
    public Reward[] rewards;
  }

  [System.Serializable]
  public class RecipeReward : Reward
  {
    public Recipe recipe;

    public RecipeReward(Recipe recipe)
    {
      this.recipe = recipe;
      rewardType = RewardType.Recipe;
    }

    public RecipeReward() => rewardType = RewardType.Recipe;
    public override void GiveReward() => ServiceLocator.Get<RecipeHandler>().AddKnownRecipe(recipe.ReturnID());
    public ItemGrade ReturnRecipeGrade() => recipe.Grade;

    public override int ReturnAmount() => 1;
  }

  [System.Serializable]
  public class ItemReward : Reward
  {
    public ItemData item;
    public ItemLevel level;

    public ItemReward(ItemData item, ItemLevel level, int quantity)
    {
      this.item = item;
      this.level = level;
      amount = quantity;
      rewardType = RewardType.Items;
    }

    public ItemReward() => rewardType = RewardType.Items;
    public override void GiveReward() => ServiceLocator.Get<InventoryHandler>().AddItem(item.iD, level, amount);

    public override int ReturnAmount() => amount;
  }


  [System.Serializable]
  public class XPReward : Reward
  {
    public XPReward(int quantity)
    {
      amount = quantity;
      rewardType = RewardType.Experience;
    }
    public XPReward() => rewardType = RewardType.Experience;
    public override void GiveReward() => ServiceLocator.Get<LevelHandler>().GiveExperience(amount);
    public override int ReturnAmount() => amount;
  }

  [System.Serializable]
  public class CurrencyReward : Reward
  {
    public CurrencyType type;

    public CurrencyReward(CurrencyType type, int amount, RewardType currencyReward)
    {
      this.type = type;
      this.amount = amount;
      rewardType = currencyReward;
    }
    public CurrencyReward() => rewardType = RewardType.Silver;
    public override void GiveReward() => ServiceLocator.Get<CurrencyHandler>().AddCurrency(type, amount);
    public override int ReturnAmount() => amount;
  }

}