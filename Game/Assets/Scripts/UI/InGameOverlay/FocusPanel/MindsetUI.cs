using System;
using MageAFK.Animation;
using MageAFK.Core;
using MageAFK.Management;
using MageAFK.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MageAFK.UI
{
  public class MindsetUI : MonoBehaviour
  {
    [SerializeField] private MindStateUISlot[] uiSlots;
    [SerializeField] private ButtonUpdateClass refreshButton;
    [SerializeField] private TMP_Text costToRefreshText;
    [SerializeField] private Color offColor;
    [SerializeField] private Color onColor;
    // [SerializeField] private int index;


    [Header("References")]
    [SerializeField] private MindsetHandler mindsetHandler;

    [Header("Animations")]
    [SerializeField] private Vector2 desiredScale;
    private static readonly Vector2 baseScale = new Vector2(1, 1);
    [SerializeField] private float animSpeed = .25f;
    private int counter = 1;

    #region Classes
    [Serializable]
    public class MindStateUISlot
    {
      public GameObject blackMask;
      public TMP_Text name;
      public Image back, front;
      public TMP_Text value;
      public TMP_Text statDesc;
    }
    #endregion

    public void SetUpSlots(Mindset[] set)
    {
      ResetUIMindStates();

      for (int i = 0; i < set.Length; i++)
        SetSlot(set[i], uiSlots[i]);

      costToRefreshText.text = mindsetHandler.ReturnCostToRefresh(counter).ToString("N0");
      ServiceLocator.Get<CurrencyHandler>().SubscribeToCurrencyEvent(UpdateRefreshButton, CurrencyType.SilverCoins, true);
    }

    public void SetUpSlotsForWave(int chosenIndex)
    {
      SetSlotActive(chosenIndex);

      for (int i = 0; i < uiSlots.Length; i++)
      {
        uiSlots[i].blackMask.SetActive(i != chosenIndex);
      }

      ServiceLocator.Get<CurrencyHandler>().SubscribeToCurrencyEvent(UpdateRefreshButton, CurrencyType.SilverCoins, false);
      refreshButton.black.SetActive(true);
      refreshButton.button.interactable = false;

      counter = 1;
    }

    private void SetSlot(Mindset set, MindStateUISlot slot)
    {
      slot.name.text = set.iD.ToString();
      slot.statDesc.text = set.desc;
      slot.blackMask.SetActive(false);

      slot.value.text = StringManipulation.FormatStatNumberValue(set.value, set.isPercentage, set.value < 1 ? "N2" : "N1");
    }

    public void OnChangeMindStatePressed(int index)
    {
      if (WaveHandler.WaveState != WaveState.Counter || !mindsetHandler.ChangeChosenMindState(index)) { return; }
      ResetUIMindStates();
      SetSlotActive(index);
    }

    private void SetSlotActive(int index)
    {
      var slot = uiSlots[index];
      slot.front.color = onColor;
      if (gameObject.activeInHierarchy)
        UIAnimations.Instance.ChangeScale(slot.back.rectTransform, desiredScale, animSpeed, baseScale);
      else
        slot.back.rectTransform.localScale = new Vector3(desiredScale.x, desiredScale.y, 1f);
    }

    private void ResetUIMindStates()
    {
      for (int i = 0; i < uiSlots.Length; i++)
      {
        var slot = uiSlots[i];
        slot.front.color = offColor;
        UIAnimations.Instance.TryStopCoroutine(slot.back.gameObject);
        UIAnimations.Instance.ChangeScale(slot.back.rectTransform, baseScale, animSpeed, desiredScale);
      }
    }

    public void OnRefreshPressed()
    {
      if (ServiceLocator.Get<CurrencyHandler>().SubtractCurrency(CurrencyType.SilverCoins, mindsetHandler.ReturnCostToRefresh(counter++)))
        mindsetHandler.CreateNewMindStates();
    }

    private void UpdateRefreshButton(int amount)
    {
      bool state = mindsetHandler.ReturnCostToRefresh() > amount;
      refreshButton.black.SetActive(state);
      refreshButton.button.interactable = !state;
    }
  }

}