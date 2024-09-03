using System.Collections.Generic;
using MageAFK.Animation;
using MageAFK.Core;
using MageAFK.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MageAFK.Spells;
using MageAFK.AI;
using Sirenix.OdinInspector;
using MageAFK.Player;
using MageAFK.Management;

namespace MageAFK.UI
{

  public class HistoryPopUp : SerializedMonoBehaviour
  {
    [Header("UI Objects")]
    [SerializeField] private Dictionary<PlayerStatisticEnum, TMP_Text> statFields;
    [SerializeField] private TMP_Text wave;
    [SerializeField] private TMP_Text spellDamage;
    [SerializeField] private TMP_Text spellName;
    [SerializeField] private Image spellImage;
    [SerializeField] private TMP_Text damage;
    [SerializeField] private TMP_Text level;
    [SerializeField] private TMP_Text mobName;
    [SerializeField] private TMP_Text mobDamage;
    [SerializeField] private Image mobImage;

    [SerializeField] private Image location;


    [Header("Interaction Objects")]
    [SerializeField] private Button blackMask;




    public void OpenPopUp(SiegeStatistic statistics)
    {
      FillUI(statistics);
      blackMask.gameObject.SetActive(true);
      blackMask.onClick.AddListener(() => ClosePopUp());

      UIAnimations.Instance.OpenPanel(gameObject);
      UIPanelHandler.SetCurrentPanel(UIPanel.History_History_Information);
    }

    private void FillUI(SiegeStatistic statistics)
    {
      Mob archNemesis = ServiceLocator.Get<EntityHandler>().GetMob(statistics.mobPair.Item1);
      Spell spell = ServiceLocator.Get<SpellHandler>().GetSpellData(statistics.spellPair.Item1);

      location.sprite = ServiceLocator.Get<LocationHandler>().ReturnLocationData(statistics.location).image;

      wave.text = statistics.wave;
      spellName.text = spell.name;
      spellDamage.text = $"{StringManipulation.FormatShortHandNumber(statistics.spellPair.Item2)} DMG";
      spellImage.sprite = spell.image;
      damage.text = StringManipulation.FormatShortHandNumber(statistics.totalDamageDone);
      level.text = $"Level {statistics.level}";

      mobName.text = archNemesis.name;
      mobImage.sprite = archNemesis.headShotImage;
      mobDamage.text = $"{StringManipulation.FormatShortHandNumber(statistics.mobPair.Item2)} DMG";

      foreach (KeyValuePair<PlayerStatisticEnum, TMP_Text> pair in statFields)
      {
        if (statistics.intMetrics.TryGetValue(pair.Key, out int intVal))
          pair.Value.text = intVal.ToString("N0");
        else
          pair.Value.text = "0";
      }

    }


    public void ClosePopUp()
    {
      if (!gameObject.activeInHierarchy) { return; }
      blackMask.onClick.RemoveListener(() => ClosePopUp());
      UIAnimations.Instance.ClosePanel(gameObject, () => { blackMask.gameObject.SetActive(false); });
    }

  }


}