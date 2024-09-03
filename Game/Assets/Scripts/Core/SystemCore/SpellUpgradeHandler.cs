
using UnityEngine;
using MageAFK.Management;
using System.Collections.Generic;
using MageAFK.Stats;
using MageAFK.Spells;
using Sirenix.OdinInspector;


namespace MageAFK.Core
{
    public class SpellUpgradeHandler : SerializedMonoBehaviour
  {
    [SerializeField] private Dictionary<Stat, StatUpgradeRule> rules;

    [SerializeField] private StatUpgradeRule defaultRule;

    [Button("ApplyDefaultToAll")]
    public void Apply()
    {
      if (rules != null)
      {
        foreach (var pair in rules)
        {
          pair.Value.valMod = defaultRule.valMod;
          pair.Value.costMod = defaultRule.costMod;
          pair.Value.baseCost = defaultRule.baseCost;
        }
      }

    }


    [System.Serializable]
    public class StatUpgradeRule
    {
      [Tooltip("Ex -> Amour = 11 , 12 , etc")]
      public bool incrementalUpgrade;

      [Tooltip("(level * valmod) * baseValue = increase per level || increase per level")]
      public float valMod;
      [Tooltip("(level * costMod) * baseCost = increase per level")]
      public float costMod;
      public float baseCost;
    }

    public bool UpgradeStat(SpellStat stat)
    {
      if (stat == null || !ServiceLocator.Get<CurrencyHandler>().SubtractCurrency(CurrencyType.SilverCoins, GetCost(stat))) return false;
      stat.level++;
      var rule = stat.rule != null ? stat.rule : rules[stat.statType];
      stat.runtimeValue = rule.incrementalUpgrade ?
        stat.baseValue + (stat.level * rule.valMod) :
        stat.baseValue + (stat.level * (stat.baseValue * rule.valMod));

      return true;
    }

    public int GetCost(SpellStat stat)
    {
      var rule = stat.rule != null ? stat.rule : rules[stat.statType];
      return (int)(rule.baseCost + (stat.level * (rule.baseCost * rule.costMod)));
    }

  }
}