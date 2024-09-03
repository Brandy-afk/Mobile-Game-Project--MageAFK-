
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using MageAFK.Management;
using MageAFK.Tools;

namespace MageAFK.Stats
{
  public class StatInformation : SerializedMonoBehaviour
  {

    [Header("percentageModification means percentage based modification. NOT percentage based values (So incremental)")]
    [Header("meaning everything is a percentage of a base value, EX: Armour would only every be +1 (so false)")]
    [SerializeField] private Dictionary<Stat, StatInfo> information = new();
    private void Awake() => ServiceLocator.RegisterService(this);
    public StatInfo ReturnStatInformation(Stat stat) => information[stat];

    #region Editor

    private void OnValidate()
    {
      foreach (var pair in information)
      {
        pair.Value.statName = StringManipulation.AddSpacesBeforeCapitals(pair.Key.ToString());

        int _index = -1;
        for (int i = 0; i < pair.Value.statName.Length; i++)
        {
          if (pair.Value.statName[i] == '_')
          {
            _index = i;
            break;
          }
        }

        if (_index != -1)
          pair.Value.statName = pair.Value.statName.Substring(0, _index);
      }
    }

    [Button("Pack")]
    public void PackList()
    {
      foreach (Stat s in System.Enum.GetValues(typeof(Stat)))
      {
        if (information.ContainsKey(s)) continue;
        information[s] = new StatInfo();
      }
    }
    #endregion
  }

  [System.Serializable]
  public class StatInfo
  {

    [Header("Means object is effected by modifiers")]
    [Header("Ex : Armour is not percentage mod because it stores a value not a mod")]
    public bool percentageModification = true;

    [TextArea(5, 5), BoxGroup("Strings")]
    public string desc;
    [TextArea(5, 5), BoxGroup("Strings"), ReadOnly]
    public string statName;
    [InfoBox("Such as % or 22s(seconds) or 22ft(feet)")]
    public string symbol;
  }
}

