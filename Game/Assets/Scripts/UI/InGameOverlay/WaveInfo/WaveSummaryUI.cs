
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MageAFK.Spells;
using MageAFK.Management;
using MageAFK.AI;
using MageAFK.Tools;
using MageAFK.Core;
using MageAFK.Player;
using MageAFK.Animation;

namespace MageAFK.UI
{

  public class WaveSummaryUI : SerializedMonoBehaviour
  {


    [SerializeField] private Dictionary<PlayerStatisticEnum, TMP_Text> metricTxt;
    [SerializeField] private SpellUI[] spellUI;
    [SerializeField] private TMP_Text waveText;


    [SerializeField] private TMP_Text mobName;
    [SerializeField] private TMP_Text mobDmg;
    [SerializeField] private Image mob;




    [System.Serializable]
    private class SpellUI
    {
      public Image image;
      public TMP_Text value;
      public TMP_Text spellName;
    }
    private void OnDisable() => WaveHandler.SubToWaveState(OnWaveStateChanged, false);
    private void OnWaveStateChanged(WaveState state)
    {
      if (state == WaveState.Wave) ClosePanel();
    }

    public void WaveStatisticsToUI(WaveStatistics statistics)
    {
      UpdateStats(statistics);
      UpdateNemesis(statistics.nemesisPair);
      UpdateSpells(statistics.topSpells);

      gameObject.transform.parent.gameObject.SetActive(true);
      UIAnimations.Instance.OpenPanel(gameObject, () =>
      {
        WaveHandler.SubToWaveState(OnWaveStateChanged, true);
        gameObject.transform.parent.GetComponent<Button>().onClick.AddListener(ClosePanel);
      });
    }

    private void UpdateStats(WaveStatistics statistics)
    {
      waveText.text = $"{statistics.wave} Complete!";

      foreach (KeyValuePair<PlayerStatisticEnum, TMP_Text> pair in metricTxt)
      {
        if (statistics.intMetrics.ContainsKey(pair.Key))
        {
          pair.Value.text = StringManipulation.FormatShortHandNumber(statistics.intMetrics[pair.Key]);
        }
      }
    }

    private void UpdateNemesis(KeyValuePair<EntityIdentification, float> enemyPair)
    {
      if (enemyPair.Key == EntityIdentification.None)
      {
        mob.gameObject.SetActive(false);
        mobName.text = "Nothing";
        mobDmg.text = "0";
      }
      else
      {
        Mob mob = ServiceLocator.Get<EntityHandler>().GetMob(enemyPair.Key);
        mobName.text = mob.name;
        mobDmg.text = enemyPair.Value.ToString("N0");
        this.mob.sprite = mob.headShotImage;
        this.mob.gameObject.SetActive(true);
      }

    }

    private void UpdateSpells((SpellIdentification, float)[] topSpells)
    {
      for (int i = 0; i < spellUI.Length; i++)
      {
        bool state = i < topSpells.Length;

        spellUI[i].image.gameObject.SetActive(state);

        Spell spell = state ? ServiceLocator.Get<SpellHandler>().GetSpellData(topSpells[i].Item1) : null;
        spellUI[i].image.sprite = state ? spell.image : null;
        spellUI[i].value.text = state ? topSpells[i].Item2.ToString("N0") : "";
        spellUI[i].spellName.text = state ? spell.spellName : "";
      }
    }

    private void ClosePanel()
    {
      UIAnimations.Instance.ClosePanel(gameObject, () => { gameObject.transform.parent.gameObject.SetActive(false); });
      transform.parent.GetComponent<Button>().onClick.RemoveListener(ClosePanel);
    }
  }

}
