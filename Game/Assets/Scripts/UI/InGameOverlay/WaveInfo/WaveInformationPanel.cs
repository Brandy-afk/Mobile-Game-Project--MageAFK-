using MageAFK.Core;
using MageAFK.Management;
using MageAFK.Pooling;
using MageAFK.TimeDate;
using MageAFK.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace MageAFK.UI
{
  public class WaveInformationPanel : MonoBehaviour
  {


    [Header("Objects")]
    [SerializeField] private TMP_Text title;

    //Enemy UI
    [SerializeField] private MobUI[] mobUI;

    [System.Serializable]
    public class MobUI
    {
      public Animator animator;
      public Image typeImage;
      public TMP_Text typeText, name;
    }
    //Time UI
    [SerializeField] private TMP_Text waveTime;
    [SerializeField] private TMP_Text counterTime;

    //Gem Cost
    [SerializeField] private TMP_Text silverCost;

    //Font Size
    [SerializeField] private float largetFontSize;
    [SerializeField] private float smallFontSize;


    //Masks
    [SerializeField] private ButtonUpdateClass refreshButton;


    [Header("Mechanics")]
    [SerializeField] private int baseCostToRefresh;
    [SerializeField] private int waveCostVariance;
    [SerializeField] private float refreshCostModifier;





    //Button Handling
    private int timesRefreshed = 0;
    private int currentCost;

    //Wave tracking
    private WaveState waveState;
    private int wave;

    #region Life Cycle / Events

    private void OnEnable()
    {
      ServiceLocator.Get<CurrencyHandler>().SubscribeToCurrencyEvent(OnSilverCoinsChanged, CurrencyType.SilverCoins, true);
      WaveHandler.SubToWaveState(OnWaveStateChanged, true);
    }

    private void OnDisable()
    {
      ServiceLocator.Get<CurrencyHandler>().SubscribeToCurrencyEvent(OnSilverCoinsChanged, CurrencyType.SilverCoins, false);
      WaveHandler.SubToWaveState(OnWaveStateChanged, false);
    }

    private void OnSilverCoinsChanged(int amount)
    {
      ToggleButton();
    }
    private void OnWaveStateChanged(WaveState state)
    {
      waveState = state;
      ToggleButton();
      SetTitle();
    }
    #endregion

    public void NewWaveInfo()
    {
      timesRefreshed = 0;
      FillEnemyUI();
      UpdateButton();
      ToggleButton();
    }

    public void SetTitle()
    {
      title.text = waveState == WaveState.Wave ? "Current Wave - Click Enemies for details." : waveState == WaveState.Intermission ? "Last Wave - Click Enemies for details." : "Next Wave - Click Enemies for details.";
    }

    public void FillEnemyUI()
    {
      var mobs = EnemyPooler.currentMobs;
      for (int i = 0; i < mobs.Length; i++)
      {
        if (mobs[i] is null) continue;
        mobUI[i].animator.runtimeAnimatorController = mobs[i].controller;
        mobUI[i].typeImage.sprite = GameResources.Entity.ReturnEnemyTypeSprite(mobs[i].type);
        mobUI[i].typeText.text = mobs[i].type.ToString();
        mobUI[i].name.text = mobs[i].name;
      }
    }

    public void UpdateButton()
    {
      currentCost = baseCostToRefresh + (wave * waveCostVariance);
      currentCost += (int)(currentCost * (timesRefreshed * refreshCostModifier));
      silverCost.text = currentCost.ToString();
      LayoutRebuilder.ForceRebuildLayoutImmediate(silverCost.transform.parent.GetComponent<RectTransform>());
    }

    public void ToggleButton()
    {
      bool state = waveState != WaveState.Counter ? false : currentCost < ServiceLocator.Get<CurrencyHandler>().GetCurrencyAmount(CurrencyType.SilverCoins) ? true : false;
      refreshButton.black.SetActive(!state);
      refreshButton.button.interactable = state;
    }

    public void OnRefreshPressed()
    {
      if (ServiceLocator.Get<CurrencyHandler>().SubtractCurrency(CurrencyType.SilverCoins, currentCost))
      {
        //Update cost and button
        timesRefreshed++;
        UpdateButton();

        //Get new enemies 
        // ServiceLocator.Get<EnemyPooler>().Create();
        FillEnemyUI();

      }
      else
      {
        Debug.Log("Cant Afford Refreshing mobs");
        return;
      }
    }


  }


}
