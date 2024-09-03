using MageAFK.Animation;
using MageAFK.Core;
using MageAFK.Management;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MageAFK.UI
{
  public class BookCurrencyUI : CurrencyUI
  {
    [SerializeField] private TMP_Text silverText;
    [SerializeField] private TMP_Text level;


    protected override void OnEnable()
    {
      base.OnEnable(); ;

      bool state = WaveHandler.WaveState != WaveState.None;
      silverText.gameObject.SetActive(state);
      level.transform.parent.gameObject.SetActive(state);

      if (state)
      {
        ServiceLocator.Get<CurrencyHandler>().SubscribeToCurrencyEvent(OnSilverChanged, CurrencyType.SilverCoins, true);
        ServiceLocator.Get<LevelHandler>().SubscribeToLevelChanged(OnLevelChanged, true);
      }
    }

    protected override void OnDisable()
    {
      base.OnEnable();
      if (silverText.gameObject.activeSelf)
      {
        ServiceLocator.Get<CurrencyHandler>().SubscribeToCurrencyEvent(OnSilverChanged, CurrencyType.SilverCoins, false);
        ServiceLocator.Get<LevelHandler>().SubscribeToLevelChanged(OnLevelChanged, false);
      }

    }
    private void OnSilverChanged(int silver) => silverText.text = $"<sprite name=Silver>{silver:N0}";

    private void OnLevelChanged(int currentLevel)
    {
      if (level.text != currentLevel.ToString())
        UIAnimations.Instance.AnimateTextScale(level);

      level.text = $"<sprite name=XP>{currentLevel:N0}";
    }
  }
}