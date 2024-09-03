
using System;
using System.Collections.Generic;
using MageAFK.Animation;
using MageAFK.Core;
using MageAFK.Management;
using MageAFK.Spells;
using MageAFK.Stats;
using MageAFK.Tools;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


namespace MageAFK.UI

{
  public class SpellUpgradeUI : MonoBehaviour, IPagination<Stat>
  {
    [Header("Variables")]
    [SerializeField, FormerlySerializedAs("statUI")] private StatUI[] slots;

    [Serializable]
    private class StatUI
    {
      public GameObject holder;
      public Image outline, purchaseImage;
      public TMP_Text value, cost, stat;
      public Button button;
    }
    [SerializeField] private ButtonUpdateClass[] pageButtons;
    [SerializeField] private TMP_Text pageText;

    [Header("Visual")]
    [SerializeField, TabGroup("Regular")] private Sprite greenSprite;
    [SerializeField, TabGroup("Regular")] private Sprite redSprite;
    [SerializeField] private Sprite maxedSprite;



    [Header("references")]
    [SerializeField] private SpellUpgradeHandler spellUpgradeHandler;
    [SerializeField] private SpellStatPopUp popUp;

    private Pagination<Stat> pagination;
    private Dictionary<int, (StatUI UI, SpellStat STAT)> map;

    private Vector3 regularValuePos;
    private Vector3 maxedValuePos;

    private static bool setUpSwitch = false;
    private Spell current;


    #region Setup
    // [Button]
    // public void SetUpUI()
    // {
    //   if (objs != null)
    //   {
    //     statUI = new StatUI[16];

    //     for (int i = 0; i < objs.Length; i++)
    //     {
    //       statUI[i] = new StatUI();
    //       var s = statUI[i];
    //       var o = objs[i];

    //       s.holder = o.GetChild(0).gameObject;
    //       s.outline = o.GetComponent<Image>();
    //       s.currency = o.GetChild(0).GetChild(3).gameObject;
    //       s.value = o.GetChild(0).GetChild(2).GetComponent<TMP_Text>();
    //       s.cost = o.GetChild(0).GetChild(4).GetComponent<TMP_Text>();
    //       s.stat = o.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
    //       s.purchaseImage = o.GetChild(0).GetChild(1).GetComponent<Image>();
    //       s.button = s.purchaseImage.GetComponent<Button>();
    //     }
    //   }
    // }
    #endregion

    #region Enable / Disable

    private void Awake()
    {
      regularValuePos = slots[0].value.rectTransform.anchoredPosition;
      maxedValuePos = slots[1].value.rectTransform.anchoredPosition;
    }

    private void OnEnable()
    {
      if (current != SpellPopUpUI.currentSpell || setUpSwitch)
      {
        SetUpSpell();
        setUpSwitch = false;
      }
      else if (pagination != null)
      {
        pagination.SetPage(1);
        UpdatePage();
      }

      ServiceLocator.Get<CurrencyHandler>().SubscribeToCurrencyEvent(UpdateButtonStates, CurrencyType.SilverCoins, true);
    }

    private void OnDisable()
      => ServiceLocator.Get<CurrencyHandler>().SubscribeToCurrencyEvent(UpdateButtonStates, CurrencyType.SilverCoins, false);

    #endregion

    private void SetUpSpell()
    {
      current = SpellPopUpUI.currentSpell;
      map ??= new Dictionary<int, (StatUI, SpellStat)>();
      pagination = new Pagination<Stat>(current.sortedStats, this, 6);
      pagination.UpdateDisplay();
      UpdatePageButtons();
      UpdatePage();
    }

    public void UpdateSlot(Stat stat, int index)
    {
      var state = stat != default;
      StatUI uI = slots[index];

      uI.outline.color = new Color(uI.outline.color.r, uI.outline.color.g, uI.outline.color.b, state ? 1f : 0.5f);
      uI.holder.SetActive(state);

      if (state)
      {
        SpellStat spellStat = current.GetStat(stat);
        map[index] = (uI, spellStat);
        uI.stat.text = StringManipulation.AddSpacesBeforeCapitals(stat.ToString());
        UpdateValue(index);
        UpdateCost(index);
        ToggleMaxUI(uI, spellStat.IsMaxed() || WaveHandler.WaveState == WaveState.None);
      }
    }

    private void UpdateValue(int index)
    {
      var values = map[index];
      var symbol = ServiceLocator.Get<StatInformation>().ReturnStatInformation(values.STAT.statType).symbol;
      values.UI.value.text = StringManipulation.FormatStatNumberValue(values.STAT.runtimeValue,
                                                                      values.STAT.isPercentage,
                                                                      values.STAT.runtimeValue < 1 ? "N2" : "N1",
                                                                      false,
                                                                      symbol);
    }

    private void UpdateCost(int index)
    {
      var values = map[index];
      if (values.STAT.IsMaxed())
        return;

      values.UI.cost.text = $"{StringManipulation.FormatShortHandNumber(spellUpgradeHandler.GetCost(values.STAT))}<sprite name=Silver>";
      UpdateState(index, ServiceLocator.Get<CurrencyHandler>().GetCurrencyAmount(CurrencyType.SilverCoins));
    }
    private void UpdateButtonStates(int silver)
    {
      foreach (var index in map.Keys)
        UpdateState(index, silver);
    }


    private void ToggleMaxUI(StatUI slot, bool notUpgradable)
    {
      if (notUpgradable)
      {
        slot.purchaseImage.sprite = maxedSprite;
      }
      slot.cost.gameObject.SetActive(!notUpgradable);
      slot.value.rectTransform.anchoredPosition = notUpgradable ? maxedValuePos : regularValuePos;
      slot.purchaseImage.raycastTarget = !notUpgradable;
    }



    private void UpdateState(int index, int silver)
    {
      var values = map[index];
      if (values.STAT.IsMaxed()) return;

      var state = silver >= spellUpgradeHandler.GetCost(values.STAT);
      values.UI.purchaseImage.raycastTarget = state;
      values.UI.purchaseImage.sprite = state ? greenSprite : redSprite;
    }

    #region Button Interaction

    public void OnStatPressed(int index)
    {
      if (current.sortedStats.Length - 1 < index) return;
      popUp.InputAndOpen(map[index].STAT);
    }

    public void OnStatUpgradePressed(int index)
    {
      if (current.sortedStats.Length - 1 < index) return;

      var spellStat = map[index].STAT;
      if (spellStat.upgradable && spellUpgradeHandler.UpgradeStat(spellStat))
      {
        current.LeveUp();
        UpdateValue(index);
        UpdateCost(index);
        current.AppendRecord(SpellRecordID.Upgraded, 1);
        if (spellStat.IsMaxed())
        {
          ToggleMaxUI(map[index].UI, true);
          CustomMilestoneBehaviour.CheckIfSpellMaxed(current);
        }
      }
    }

    #endregion

    #region Pagination
    public void UpdatePage() => pageText.text = $"{pagination.ReturnCurrentPage()}/{pagination.ReturnTotalPages()}";
    public void UpdatePageButtons() => Pagination<Stat>.UpdatePageButtons(pagination, pageButtons);
    public void AlterPagePressed(bool isNext)
    {
      if (isNext) pagination.NextPage();
      else pagination.PreviousPage();
      UpdatePage();
    }

    public void CustomPaginationBehaviour()
    {
      return;
    }

    #endregion

  }
}
