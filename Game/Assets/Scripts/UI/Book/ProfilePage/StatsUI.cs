
using System.Collections.Generic;
using MageAFK.Management;
using MageAFK.Player;
using MageAFK.Tools;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MageAFK.UI
{
  public class StatsUI : SerializedMonoBehaviour
  {
    [Header("Objects"), TabGroup("Variables")]
    [SerializeField] private Dictionary<PlayerStatisticEnum, TMP_Text> statDict;

    [SerializeField] private List<Transform> statObjects;

    [System.Serializable]
    public class StatUIObject
    {
      public PlayerStatisticEnum stat;
      public Transform transform;
    }

    [SerializeField, TabGroup("Creation")] private PlayerStatisticEnum value;
    [SerializeField, TabGroup("Creation")] private Sprite sprite;

    [Button("Add Stat")]
    public void AddStat()
    {

      if (!statDict.ContainsKey(value) && statObjects.Count > 0)
      {
        var trans = statObjects[0];
        statObjects.RemoveAt(0);
        var valueObject = trans.GetChild(1).GetComponent<TMP_Text>();
        valueObject.text = "0";
        trans.GetChild(0).GetComponent<TMP_Text>().text = StringManipulation.AddSpacesBeforeCapitals(value.ToString());
        trans.GetChild(2).GetChild(0).GetComponent<Image>().sprite = sprite;
        statDict[value] = valueObject;
      }
    }


    private void Awake()
    {
      ServiceLocator.Get<PlayerData>().InputStatUI(this);
    }

    private void OnEnable()
    {
      ServiceLocator.Get<PlayerData>().UpdateAllStatsToUI();
    }

    public void UpdateStatUI(PlayerStatisticEnum stat, int value)
    {
      if (statDict.ContainsKey(stat))
      {
        statDict[stat].text = StringManipulation.FormatShortHandNumber(value);
      }
      else
      {
        // Debug.Log($"{stat} not found"); TODO  Turned off for the time being
      }

    }


  }

}