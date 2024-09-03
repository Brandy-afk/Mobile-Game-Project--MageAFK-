
using UnityEngine;
using MageAFK.Spells;
using TMPro;
using MageAFK.Management;
using System.Text;
using MageAFK.Core;
using MageAFK.Player;

namespace MageAFK.UI
{
  public class CatalogSlot : SpellSlot
  {

    [SerializeField] private TMP_Text cost;
    [SerializeField] private GameObject lockedPanel;
    [SerializeField] private SpellPurchasePopUp purchasePopUp;

    protected override DragZoneIndentifier zone { get => DragZoneIndentifier.Spell_List; }
    private const string SILVER_NAME = "<sprite name=Silver>";

    /// <summary>
    /// Decides how many silver icons will be displayed in relation to its price.
    /// </summary>
    private readonly (int threshold, int iconAmount)[] VISUAL_MAP = new (int, int)[]
    {
      (5000, 1),
      (10000, 2),
      (30000, 3)
    };

    private void OnEnable()
    {
      bool isWave = WaveHandler.WaveState != WaveState.None;
      if (content != null && content.IsUnlocked && isWave)
      {
        content.SubscribeToLevelChanged(UpdateLevel, true);
      }
    }

    private void OnDisable()
    {
      if (content != null && content.IsUnlocked)
        content.SubscribeToLevelChanged(UpdateLevel, false);
    }

    #region UI
    public override void SetUp(Spell incoming)
    {
      if (content != null) OnDisable();
      content = incoming;

      if (content == null || (dragInfo.Drag != null && dragInfo.Drag == content))
      {
        ToggleObjects(force: false);
        return;
      }
      else if (content.IsUnlocked)
      {
        OnEnable();
      }

      SetUpUI();
    }
    private void SetUpUI()
    {
      ToggleObjects(content.IsUnlocked);
      image.sprite = content.image;
      if (!content.IsUnlocked) CreateCostVisual();
    }
    private void CreateCostVisual()
    {
      int icons = 4;
      var cost = ServiceLocator.Get<PlayerStatHandler>().ReturnModifiedValue(Stats.Stat.SpellCost, content.cost);
      for (int i = 0; i < VISUAL_MAP.Length; i++)
      {
        if (cost <= VISUAL_MAP[i].threshold)
        {
          icons = VISUAL_MAP[i].iconAmount;
          break;
        }
      }

      StringBuilder builder = new();
      for (int i = 0; i < icons; i++)
        builder.Append(SILVER_NAME);

      this.cost.text = builder.ToString();
    }
    private void ToggleObjects(bool state = true, bool? force = null)
    {
      lockedPanel.SetActive(force != null ? (bool)force : !state);
      ToggleLevelPanel((force != null ? (bool)force : state) && WaveHandler.WaveState != WaveState.None);
      image.gameObject.SetActive(force == null || (bool)force);
    }
    private void ToggleLevelPanel(bool state) => level.transform.parent.gameObject.SetActive(state);
    private void UpdateLevel() => level.text = content.Level.ToString();

    #endregion

    #region Interaction

    protected override void OnSingleClick()
    {
      if (content.IsUnlocked)
        infoPopUp.InputAndOpen(content);
      else
      {
        purchasePopUp.OpenPanel(content);
        purchasePopUp.OnDecision += OnDecision;
      }
    }

    protected override void OnDoubleClick()
    {
      if (content.IsUnlocked)
      {
        infoPopUp.InputAndOpen(content);
      }
      else
      {
        purchasePopUp.OpenPanel(content);
        purchasePopUp.OnDecision += OnDecision;
      }
    }

    public void OnDecision(bool purchase)
    {
      if (ServiceLocator.Get<CurrencyHandler>().SubtractCurrency(CurrencyType.SilverCoins, content.cost))
      {
        content.SetUnlock(true);
        SetUp(content);
      }
    }



    #endregion
  }



}