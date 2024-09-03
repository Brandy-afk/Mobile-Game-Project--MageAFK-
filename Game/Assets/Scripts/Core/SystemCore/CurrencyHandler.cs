using System;
using MageAFK.Management;
using System.Collections.Generic;
using MageAFK.Player;

namespace MageAFK.Core
{
  public class CurrencyHandler : IData<CurrencyData>
  {
    private Dictionary<CurrencyType, Currency> currencies;

    #region Save/Load

    //Perm data
    public void InitializeData(CurrencyData data)
    {
      foreach (var item in data.currenices)
        SetCurrencyValue(item.Item1, item.Item2);
    }

    public void SetCurrencyValue(CurrencyType type, int amount)
    {
      currencies[type].amount = amount;
    }

    public CurrencyData SaveData()
    {
      (CurrencyType, int)[] currencyArray = new (CurrencyType, int)[]
      {
          (CurrencyType.GoldBars, currencies[CurrencyType.GoldBars].amount),
          (CurrencyType.DemonicGems, currencies[CurrencyType.DemonicGems].amount),
          (CurrencyType.SkillPoints, currencies[CurrencyType.SkillPoints].amount)
      };

      return new CurrencyData(currencyArray);
    }

    #endregion

    public CurrencyHandler()
    {
      currencies = new Dictionary<CurrencyType, Currency>
            {
                { CurrencyType.SilverCoins, new Currency(CurrencyType.SilverCoins, 40000, PlayerStatisticEnum.Silver, MilestoneID.CollectSilver) },
                { CurrencyType.GoldBars, new Currency(CurrencyType.GoldBars, 10, PlayerStatisticEnum.Gold, MilestoneID.None)},
                { CurrencyType.DemonicGems, new Currency(CurrencyType.DemonicGems, 100, PlayerStatisticEnum.Gems, MilestoneID.None) },
                { CurrencyType.SkillPoints, new Currency(CurrencyType.SkillPoints, 50, PlayerStatisticEnum.SkillPoints, MilestoneID.None)}
            };

      WaveHandler.SubToSiegeEvent((Status status) =>
      {
        if (status == Status.End_CleanUp)
        {
          SubtractCurrency(CurrencyType.SilverCoins, GetCurrencyAmount(CurrencyType.SilverCoins));
        }
      }, true);
    }

    /// <summary>
    /// For LOADING ONLY.
    /// </summary>


    public void AddCurrency(CurrencyType type, int amount)
    {
      if (currencies.TryGetValue(type, out Currency currency))
      {
        currency.amount += amount;
        ServiceLocator.Get<PlayerData>().AddStatValue(currency.playerStatistic, amount);
        ServiceLocator.Get<MilestoneHandler>().UpdateMileStone(currency.milestoneID, amount);

        if (WaveHandler.WaveState == WaveState.None) SaveManager.Save(SaveData(), DataType.CurrencyData);
      }
    }

    public bool SubtractCurrency(CurrencyType type, int amount)
    {
      if (currencies.ContainsKey(type))
      {
        if ((currencies[type].amount - amount) < 0)
        {
          return false;
        }
        else
        {
          currencies[type].amount -= amount;
          if (WaveHandler.WaveState == WaveState.None) SaveManager.Save(SaveData(), DataType.CurrencyData);
          return true;
        }
      }
      return false;
    }

    public int GetCurrencyAmount(CurrencyType type)
    {
      if (currencies.ContainsKey(type))
      {
        return currencies[type].amount;
      }

      return 0;
    }

    /// <summary>
    /// Returns if a service is affordable based on currency value.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="amount"></param>
    /// <returns>A bool. True -> is affordable, false -> not affordable.</returns>
    public bool ReturnAffordable(CurrencyType type, int amount) => amount <= currencies[type].amount;

    public void SubscribeToCurrencyEvent(Action<int> subscriber, CurrencyType type, bool state)
    {
      if (state)
      {
        currencies[type].OnAmountChanged += subscriber;
        subscriber(currencies[type].amount);
      }
      else
      {
        currencies[type].OnAmountChanged -= subscriber;
      }
    }
  }

  public enum CurrencyType
  {
    SilverCoins,
    DemonicGems,
    SkillPoints,
    GoldBars

  }

  [Serializable]
  public class CurrencyData
  {
    public (CurrencyType, int)[] currenices;
    public CurrencyData((CurrencyType, int)[] currenices) => this.currenices = currenices;
  }


  public class Currency
  {

    private int _amount;

    public int amount
    {
      get { return _amount; }
      set
      {
        if (_amount != value)
        {
          _amount = value;
          OnAmountChanged?.Invoke(_amount); // trigger event
        }
      }
    }
    public event Action<int> OnAmountChanged;

    public CurrencyType type;
    public PlayerStatisticEnum playerStatistic;
    public MilestoneID milestoneID;

    public Currency(CurrencyType type, int intialAmount, PlayerStatisticEnum statisticEnum, MilestoneID iD)
    {
      this.type = type;
      _amount = intialAmount;
      playerStatistic = statisticEnum;
      milestoneID = iD;
    }

    public void Invoke()
    {
      OnAmountChanged?.Invoke(amount);
    }
  }

}
