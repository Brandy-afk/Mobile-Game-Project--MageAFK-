using UnityEngine;
using TMPro;
using MageAFK.Core;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using MageAFK.Player;
using MageAFK.Management;

namespace MageAFK.UI
{
  public class EndScreenUI : SerializedMonoBehaviour
  {

    [Header("Fields")]

    [SerializeField] private Dictionary<PlayerStatisticEnum, TMP_Text> statFields;
    [SerializeField] private GameObject bestWave;
    [SerializeField] private TMP_Text wave;
    [SerializeField] private TMP_Text location;
    [SerializeField] private TMP_Text insult;
    [SerializeField] private TMP_Text level;
    [SerializeField] private HistoryData data;


    private void Awake() => BuildEndScreen(data.history.ToArray()[data.history.Count - 1]);

    public void BuildEndScreen(SiegeStatistic statistics)
    {
      wave.text = statistics.wave;
      location.text = $"The {statistics.location}";

      foreach (KeyValuePair<PlayerStatisticEnum, TMP_Text> pair in statFields)
      {
        if (statistics.intMetrics.TryGetValue(pair.Key, out int intVal))
          pair.Value.text = intVal.ToString("N0");
        else
          pair.Value.text = "0";

      }

      level.text = statistics.level;
      bestWave.SetActive(statistics.isBestWave);
    }

  }

}