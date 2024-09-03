using MageAFK.Animation;
using MageAFK.Core;
using MageAFK.Management;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MageAFK.UI
{
  public class CurrencyUI : MonoBehaviour
  {
       [SerializeField] protected TMP_Text gems, gold, skillPoints;

        protected virtual void OnEnable()
        {
            ServiceLocator.Get<CurrencyHandler>().SubscribeToCurrencyEvent(OnGemsChanged, CurrencyType.DemonicGems, true);
            ServiceLocator.Get<CurrencyHandler>().SubscribeToCurrencyEvent(OnSkillPointsChanged, CurrencyType.SkillPoints, true);
            ServiceLocator.Get<CurrencyHandler>().SubscribeToCurrencyEvent(OnGoldChanged, CurrencyType.GoldBars, true);
        }

        protected virtual void OnDisable()
        {
            ServiceLocator.Get<CurrencyHandler>().SubscribeToCurrencyEvent(OnGemsChanged, CurrencyType.DemonicGems, false);
            ServiceLocator.Get<CurrencyHandler>().SubscribeToCurrencyEvent(OnSkillPointsChanged, CurrencyType.SkillPoints, false);
            ServiceLocator.Get<CurrencyHandler>().SubscribeToCurrencyEvent(OnGoldChanged, CurrencyType.GoldBars, false);
        }

        protected virtual void OnGemsChanged(int amount) => gems.text = $"<sprite name=Gem>{amount:N0}";
        protected virtual void OnGoldChanged(int amount) => gold.text = $"<sprite name=Gold>{amount:N0}";
        protected virtual void OnSkillPointsChanged(int amount) => skillPoints.text = $"<sprite name=SkillPoint>{amount:N0}";


  }

}