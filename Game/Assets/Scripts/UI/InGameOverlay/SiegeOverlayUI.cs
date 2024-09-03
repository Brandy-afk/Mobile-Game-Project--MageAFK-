using System;
using System.Collections;
using System.Collections.Generic;
using MageAFK.Animation;
using MageAFK.Core;
using MageAFK.Management;
using MageAFK.Player;
using MageAFK.Spells;
using MageAFK.Stats;
using MageAFK.TimeDate;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MageAFK.UI
{
  public class SiegeOverlayUI : SerializedMonoBehaviour
  {

    [SerializeField] private TMP_Text silver, gems, xpLeft, healthRemaining, level, wave, time, timeScale;
    [SerializeField] private Slider xpSlider, healthSlider, waveSlider;

    [SerializeField] private Button[] timeScaleButtons;
    [SerializeField] private Dictionary<SpellSlotIndex, oSpellSlot> spellSlots = new();

    [Header("Animation")]
    [SerializeField] private float textAnimationDuration = .5f;

    [SerializeField, BoxGroup("Ultimate")] private float tipTextFadeDuration = .5f;
    [SerializeField, BoxGroup("Ultimate")] private float tipTime = 3f;
    [SerializeField, BoxGroup("Ultimate")] private float ultLargeFont;
    [SerializeField, BoxGroup("Ultimate")] private float ultSmallFont;

    [SerializeField] private UltimateUIClass uRefs;




    private List<int> currentValues = new() { 0, 0, 0, 0 };

    private float timeLargeFont, timeSmallFont;

    private void Awake()
    {
      timeLargeFont = time.fontSize;
      timeSmallFont = time.fontSize - 15f;
    }

    private void OnEnable()
    {
      ServiceLocator.Get<LevelHandler>().SubscribeToLevelChanged(UpdatePlayerLevelUI, true);
      ServiceLocator.Get<LevelHandler>().SubscribeToXPChanged(UpdatePlayerExperienceUI, true);
      ServiceLocator.Get<CurrencyHandler>().SubscribeToCurrencyEvent(OnDemonicGemsChanged, CurrencyType.DemonicGems, true);
      ServiceLocator.Get<CurrencyHandler>().SubscribeToCurrencyEvent(OnSilverChanged, CurrencyType.SilverCoins, true);
      ServiceLocator.Get<PlayerHealth>().SubscribeToHealthChanged(UpdateHealth, true);
      WaveHandler.SubToWaveState(OnWaveStateChanged, true);
    }

    private void OnDisable()
    {
      ServiceLocator.Get<LevelHandler>().SubscribeToLevelChanged(UpdatePlayerLevelUI, false);
      ServiceLocator.Get<LevelHandler>().SubscribeToXPChanged(UpdatePlayerExperienceUI, false);
      ServiceLocator.Get<CurrencyHandler>().SubscribeToCurrencyEvent(OnDemonicGemsChanged, CurrencyType.DemonicGems, false);
      ServiceLocator.Get<CurrencyHandler>().SubscribeToCurrencyEvent(OnSilverChanged, CurrencyType.SilverCoins, false);
      ServiceLocator.Get<PlayerHealth>().SubscribeToHealthChanged(UpdateHealth, false);
      WaveHandler.SubToWaveState(OnWaveStateChanged, false);
    }

    private void OnWaveStateChanged(WaveState state)
    {
      UpdateWaveUI(state);
      UpdateTimeScaleButtons(state != WaveState.Intermission);
    }

    #region Currencies
    public void OnSilverChanged(int amount)
    {
      UIAnimations.Instance.UpdateTextNumberTransition(silver, currentValues[0], amount, textAnimationDuration);
      currentValues[3] = amount;
    }

    public void OnDemonicGemsChanged(int amount)
    {
      UIAnimations.Instance.UpdateTextNumberTransition(gems, currentValues[1], amount, textAnimationDuration);
      currentValues[1] = amount;
    }

    #endregion

    #region Player Info

    public void UpdatePlayerExperienceUI(int currentXP, int requiredXP)
    {
      xpSlider.maxValue = requiredXP;

      xpLeft.text = $"{requiredXP - currentXP:N0}";
      UIAnimations.Instance.AnimateTextScale(xpLeft);

      UIAnimations.Instance.AnimateSlider(xpSlider, currentXP, false);
    }

    public void UpdatePlayerLevelUI(int level)
    {
      var levelString = level.ToString();
      if (this.level.text != levelString)
      {
        UIAnimations.Instance.AnimateTextScale(this.level);
      }
      this.level.text = levelString;
    }
    public void UpdateHealth(float health, float maxHealth)
    {
      healthSlider.maxValue = maxHealth;
      healthRemaining.text = health.ToString("N1");

      UIAnimations.Instance.AnimateSlider(healthSlider, health, true);
    }

    #endregion

    #region  Slots
    public void FillSpellSlot(Spell spell, SpellSlotIndex index)
    {
      if (spellSlots.TryGetValue(index, out oSpellSlot slot))
      {
        var state = spell != null;
        slot.spell.gameObject.SetActive(state);
        if (state) slot.spell.sprite = spell.image;
      }
    }

    public void UpdateSpellSlot(Spell spell, float fractionRemaining)
    {
      if (!gameObject.activeInHierarchy || !spellSlots.ContainsKey(spell.SlotIndex)) return;
      oSpellSlot slot = spellSlots[spell.SlotIndex];
      slot.cDVisual.fillAmount = fractionRemaining;
      slot.cooldown.text = CreateTimeText(fractionRemaining * spell.ReturnStatValue(Stat.Cooldown));
    }

    public void UpdateUltimateSlot()
    {
      if (!gameObject.activeInHierarchy) return;

      var timeTaskHandler = ServiceLocator.Get<TimeTaskHandler>();
      oSpellSlot slot = spellSlots[SpellSlotIndex.Ult];
      var task = timeTaskHandler.ReturnTimeTask(Ultimate.ultTimeKey);
      slot.cooldown.text = task != null && task.duration > 0 ? timeTaskHandler.GetTimeLeftString(Ultimate.ultTimeKey, ultLargeFont, ultSmallFont) : " ";
      slot.cDVisual.fillAmount = task != null ? (task.duration / task.startingDuration) : 0f;
    }


    #endregion

    #region Time ScaleUI

    public void UpdateTimeScaleUI(float timeScale)
    {
      this.timeScale.text = $"{timeScale}x";
    }

    private void UpdateTimeScaleButtons(bool state)
    {
      foreach (var button in timeScaleButtons)
        button.interactable = state;
    }

    #endregion

    #region Wave UI
    public void UpdateWaveUI(WaveState waveState)
    {
      switch (waveState)
      {
        case WaveState.Counter:
          wave.text = "Counter";
          break;
        case WaveState.Intermission:
          wave.text = "Intermission";
          break;
        case WaveState.Wave:
          wave.text = $"Wave {WaveHandler.Wave}";
          break;
        default:
          break;
      }
    }

    public void UpdateWaveTime()
    {
      var timeTaskHandler = ServiceLocator.Get<TimeTaskHandler>();
      time.text = timeTaskHandler.GetTimeLeftString(WaveHandler.WaveTimeKey, timeLargeFont, timeSmallFont);
      var task = timeTaskHandler.ReturnTimeTask(WaveHandler.WaveTimeKey);
      var value = 1 - (task.duration / task.startingDuration);
      waveSlider.value = value < 0 ? 0 : value;
    }

    #endregion


    #region  Placable Ultimate
    public void UpdateUltimateUseTimer()
    {
      uRefs.time.text = CreateTimeText(ServiceLocator.Get<TimeTaskHandler>().ReturnTimeLeft(Ultimate.ultTimeKey));
    }

    public void UpdateUltimateUses(int left, int max)
    {
      uRefs.placeables.text = $"{left}/{max}";
    }

    public void TogglePlaceableUltUI(bool state, Spell ult = null)
    {
      foreach (var obj in uRefs.on)
      {
        obj.SetActive(state);
      }

      foreach (var obj in uRefs.off)
      {
        obj.SetActive(!state);
      }

      if (state)
      {
        uRefs.tip.text = (ult as Ultimate).tip;

        var val = (int)ult.ReturnStatValue(Stat.SpawnCap);
        UpdateUltimateUses(val, val);

        StartCoroutine(UIAnimations.Instance.FadeText(uRefs.tip, tipTextFadeDuration, 1f));
        StartCoroutine(WaitForTime());
        IEnumerator WaitForTime()
        {
          yield return new WaitForSeconds(tipTime);
          StartCoroutine(UIAnimations.Instance.FadeText(uRefs.tip, tipTextFadeDuration));
        }
      }

      uRefs.button.interactable = !state;
    }

    #endregion
    private string CreateTimeText(float remainingCooldown)
    {
      remainingCooldown = Mathf.Round(remainingCooldown * 10f) / 10f;
      return remainingCooldown > 0 ? $"{remainingCooldown:F1}" : " ";
    }

  }

  [Serializable]
  public class oSpellSlot
  {
    public SpellSlotIndex index;

    public Image spell;
    public Image cDVisual;
    public TMP_Text cooldown;

  }


  [Serializable]
  public class UltimateUIClass
  {
    public GameObject[] off;
    public GameObject[] on;
    public Button button;
    public TMP_Text time;
    public TMP_Text placeables;
    public TMP_Text tip;
  }
}