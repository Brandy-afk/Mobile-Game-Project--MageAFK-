
using System.Collections;
using System.Collections.Generic;
using MageAFK.Animation;
using MageAFK.Core;
using MageAFK.Items;
using MageAFK.Management;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace MageAFK.UI
{
  public class VoidUI : MonoBehaviour
  {
    [SerializeField, TabGroup("Variables")] private Animator tentacles, eye;
    [SerializeField, TabGroup("Variables")] private RectTransform dropZone, sliderValuePanel;
    [SerializeField, TabGroup("Variables")] private GameObject dragHighlight, tipObj, optionPanel, feedOption, upgradeOption, systemPanel, slotObj;
    [SerializeField, TabGroup("Variables")] private Vector2 feedValueSize;
    [SerializeField, TabGroup("Variables")] private Image blackFocus;
    [SerializeField, TabGroup("Variables")] private CanvasGroup buttonGroup, uIGroup;
    [SerializeField, TabGroup("Variables")] private Slider slider;
    [SerializeField, TabGroup("Variables")] private TMP_Text sliderText, buttonText, panelText;
    [SerializeField, TabGroup("Animation")] private float focusTargetAlpha = 0.5f;
    [SerializeField, TabGroup("Animation")] private float focusSpeed = 0.5f;
    [SerializeField, TabGroup("Animation")] private float uIFadeSpeed = 0.25f;
    [SerializeField, TabGroup("Animation")] private float startIdleTime = 1.5f;
    [SerializeField, TabGroup("Animation")] private float endIdleTime = 1f;
    [SerializeField, TabGroup("References")] private VoidItemButton itemButton;
    [SerializeField, TabGroup("References")] private VoidRewardUI rewardUI;

    private IDragInfo<Item, (ItemIdentification, ItemLevel)> dragInfo;

    public Item currentItem { get; private set; }
    private SystemStage system = SystemStage.None;
    public enum SystemStage
    {
      Feed,
      Upgrade,
      None
    }

    #region Constants
    private const string upgradeInfo = "Upgrading items is based on a <color=#64313E>chance</color> in correlation to how much you <color=#64313E>pay</color>.";
    private const string feedInfo = "Feeding <color=#64313E>deletes</color> items for a <color=#64313E>chance</color> at rewards.";
    private const float buttonOffAlpha = 0.3f;

    #endregion

    #region Life Cycle
    private void Start()
    {
      ServiceLocator.Get<IDragZoneCreator<Item>>().AddDragZone(dropZone, Hovering, Dropped, DragZoneIndentifier.Void);
      dragInfo = ServiceLocator.Get<IDragInfo<Item, (ItemIdentification, ItemLevel)>>();
      slider.onValueChanged.AddListener(delegate { OnSliderValueAltered(); });
    }

    private void OnDisable()
    {
      ResetVoidUI(true);
      StopAllCoroutines();
    }

    #endregion

    #region Drag Functions
    private void Hovering(bool state) => dragHighlight.SetActive(state);
    private bool Dropped()
    {
      SetNewItem(dragInfo.Drag);
      return true;
    }
    #endregion

    private void SetNewItem(Item item)
    {
      if (item == null) return;
      ResetVoidUI(false);
      currentItem = item;
      ServiceLocator.Get<InventoryHandler>().SetExclusion((item.iD, item.ReturnLevel()));
      itemButton.SetUp(item);
      upgradeOption.SetActive(item.ReturnLevel() != ItemLevel.None && item.ReturnLevel() != ItemLevel.Level3);
    }

    #region Reset
    public void ResetVoidUI(bool intialPanel)
    {
      SetObjectStates(intialPanel);
      ResetCanvasGroups();

      //Reset out of systems panel
      system = SystemStage.None;

      //Clear the exclusion from filters in inventory
      //and set item null
      if (intialPanel && currentItem != null)
      {
        Debug.Log("Clearing exclusion");
        currentItem = null;
        ServiceLocator.Get<InventoryHandler>().ClearExclusion();
      }
    }

    private void SetObjectStates(bool intialPanel)
    {
      tipObj.SetActive(intialPanel);
      optionPanel.SetActive(!intialPanel);
      systemPanel.SetActive(false);
      slotObj.SetActive(!intialPanel);
    }

    private void ResetCanvasGroups()
    {
      buttonGroup.alpha = 1f;
      buttonGroup.interactable = true;
      uIGroup.gameObject.SetActive(true);
      uIGroup.alpha = 1f;
    }
    #endregion

    #region Interaction
    public void OnOptionPressed(bool isFeed)
    {
      optionPanel.gameObject.SetActive(false);
      SetSystemConditionals(isFeed);

      if (isFeed)
        SetUpFeedSystem();
      else
        SetUpUpgradeSystem();

      slider.value = 1;
      OnSliderValueAltered();
      systemPanel.gameObject.SetActive(true);
    }

    public void OnConfirmation()
    {
      itemButton.ToggleSubInventoryAltered(false);
      List<Reward> rewards;
      bool isFail = false;
      if ((system == SystemStage.Feed && ServiceLocator.Get<SalvageHandler>().SalvageItems(currentItem, (int)slider.value, out rewards))
      || (system == SystemStage.Upgrade && ServiceLocator.Get<UpgradeHandler>().UpgradeItem(currentItem, out rewards, out isFail)))
      {
        StartCoroutine(RewardAnimation(rewards, isFail));
        rewardUI.OnClose += OnRewardsClosed;
      }
      else
      {
        Debug.LogError("Something went wrong...");
      }
    }

    private void OnRewardsClosed()
    {
      itemButton.ToggleSubInventoryAltered(true);
      uIGroup.alpha = 1f;
      uIGroup.interactable = true;
      blackFocus.gameObject.SetActive(false);

      //If item is still in inventory, keep ui concerning it.
      bool moreThenZero = ServiceLocator.Get<InventoryHandler>().ReturnItemAmount((currentItem.iD, currentItem.ReturnLevel())) > 0;
      ResetVoidUI(!moreThenZero);
    }
    #endregion

    #region Setup and Updating

    #region Setup
    private void SetSystemConditionals(bool isFeed)
    {
      system = isFeed ? SystemStage.Feed : SystemStage.Upgrade;
      panelText.text = isFeed ? feedInfo : upgradeInfo;     /* Upgrade size */
      sliderValuePanel.sizeDelta = isFeed ? feedValueSize : Vector2.zero;
    }
    private void SetUpFeedSystem() => UpdateFeedSlider();
    private void SetUpUpgradeSystem()
    {
      ServiceLocator.Get<UpgradeHandler>().CreateNewValues(currentItem);
      slider.maxValue = 10;
    }

    #endregion

    #region Update
    private void OnSliderValueAltered()
    {
      if (system == SystemStage.Upgrade)
        UpdateUpgradeSystem();
      else if (system == SystemStage.Feed)
        UpdateFeedSystem();
    }

    private void UpdateUpgradeSystem()
    {
      //Update values
      var values = ServiceLocator.Get<UpgradeHandler>().ChangeIndex((int)slider.value);
      sliderText.text = $"<sprite name=Silver>{values.Item1:N0}";
      buttonText.text = $"Upgrade\n<color=#FFFE9E>{values.Item2}% Chance";

      //Update button
      bool state = ServiceLocator.Get<CurrencyHandler>().ReturnAffordable(CurrencyType.SilverCoins, values.Item1);
      buttonGroup.alpha = state ? 1f : buttonOffAlpha;
      buttonGroup.interactable = state;
    }

    private void UpdateFeedSystem()
    {
      var value = slider.value.ToString();
      sliderText.text = value;
      buttonText.text = $"Feed\n<color=#FFFE9E>x{value}";
    }

    public void UpdateFeedSlider()
    {
      if (system == SystemStage.Feed)
      {
        int amount = ServiceLocator.Get<InventoryHandler>().ReturnItemAmount((currentItem.iD, currentItem.ReturnLevel()));
        slider.maxValue = amount;
        slider.interactable = amount > 1;
      }
    }

    #endregion

    #region Animation

    private IEnumerator RewardAnimation(List<Reward> rewards, bool isFail)
    {
      RewardAnimation_Start();
      yield return new WaitForSecondsRealtime(startIdleTime);
      RewardAnimation_End();
      yield return new WaitForSecondsRealtime(endIdleTime);
      rewardUI.OpenPanel(rewards, system, isFail);
      OverlayAnimationHandler.SetIsAnimating(false);
    }
    private void RewardAnimation_Start()
    {
      OverlayAnimationHandler.SetIsAnimating(true);
      blackFocus.gameObject.SetActive(true);

      UIAnimations.Instance.TransitionUIAlpha(blackFocus, 0f, focusTargetAlpha, focusSpeed, () => uIGroup.gameObject.SetActive(false));

      UIAnimations.Instance.FadeOut(uIGroup, uIFadeSpeed);
      eye.Play("Close");
      tentacles.Play("In");
    }

    private void RewardAnimation_End()
    {
      eye.Play("Open");
      tentacles.Play("Out");
    }

    #endregion

    #endregion
  }
}