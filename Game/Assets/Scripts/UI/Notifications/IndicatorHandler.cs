
using System.Collections.Generic;
using UnityEngine;
using MageAFK.Management;
using Sirenix.OdinInspector;
using MageAFK.Core;
using Unity.VisualScripting;

namespace MageAFK.UI
{
  public class IndicatorHandler : SerializedMonoBehaviour
  {

    [SerializeField] private Dictionary<UIPanel, UILink> UIChain = new();

    [SerializeField, ReadOnly] private HashSet<UIPanel> active = new();
    private void Awake()
    {
      ServiceLocator.RegisterService(this);
      WaveHandler.SubToWaveState(OnWaveStateChanged, true);
    }

    private void OnWaveStateChanged(WaveState state)
    {
      if (state == WaveState.Counter)
      {
        ServiceLocator.Get<IndicatorHandler>().SetUIChain(UIPanel.Plan_Focus);
        ServiceLocator.Get<IndicatorHandler>().SetUIChain(UIPanel.Plan_Mindset);
        ServiceLocator.Get<IndicatorHandler>().SetUIChain(UIPanel.Plan_Counter);
      }
    }

    #region Interface

    [BoxGroup("Indicator-Adder")] public UIPanel targetPanel;
    [BoxGroup("Indicator-Adder")] public GameObject indicatorToAdd;

    [BoxGroup("Indicator-Adder"), Button("Add")]
    public void AddInidicator()
    {
      try
      {
        if (UIChain[targetPanel].indicators.Contains(indicatorToAdd))
        {
          Debug.Log("Failed");
          return;
        }

        UIChain[targetPanel].indicators.Add(indicatorToAdd);
        print("Success!");
      }
      catch (KeyNotFoundException)
      {
        Debug.Log($"{targetPanel} not found!");
      }
    }

    #endregion


    #region CheckLater

    //EDITOR

    //NOTE - DO NOT DELETE - AUTO CREATES PARENTS/CHILD RELATIONSHIP
    // private void OnValidate()
    // {
    //   uiLinks = new List<UILink>();

    //   // First, initialize UILink for each panel and set their parents
    //   foreach (UIPanel panel in System.Enum.GetValues(typeof(UIPanel)))
    //   {
    //     UILink link = new UILink(panel);

    //     string panelName = panel.ToString();
    //     string[] parts = panelName.Split('_');

    //     if (parts.Length > 1)  // Meaning there's at least one underscore
    //     {
    //       string parentName;

    //       // If only one underscore, parent is everything before it.
    //       if (parts.Length == 2)
    //       {
    //         parentName = parts[0];
    //       }
    //       // If multiple underscores, combine everything except the last part.
    //       else
    //       {
    //         parentName = string.Join("_", parts.Take(parts.Length - 1));
    //       }

    //       UIPanel parentEnumValue;
    //       if (Enum.TryParse(parentName, out parentEnumValue))
    //       {
    //         link.parent = parentEnumValue;
    //       }
    //     }
    //     uiLinks.Add(link);
    //   }

    //   // Now, set the children/subpanels for each link
    //   foreach (var link in uiLinks)
    //   {
    //     if (link.parent != UIPanel.None)
    //     {
    //       foreach (var link1 in uiLinks)
    //       {
    //         if (link.parent == link1.panel)
    //         {
    //           if (link1.subPanels == null) { link1.subPanels = new List<UIPanel>(); }
    //           link1.subPanels.Add(link.panel);
    //         }
    //       }
    //     }
    //   }
    // }
    #endregion

    #region Game Indicators

    public void SetUIChain(UIPanel rootPanel)
    {
      if (UIPanelHandler.ReturnCurrentPanel() == rootPanel) { return; }

      if (UIChain.ContainsKey(rootPanel))
      {
        active.Add(rootPanel);
        foreach (GameObject item in UIChain[rootPanel].indicators)
        {
          item.SetActive(true);
        }

        UIPanel parentPanel = UIChain[rootPanel].parent;
        while (parentPanel != UIPanel.None)
        {
          if (parentPanel == UIPanelHandler.ReturnCurrentPanel() || CheckSubPanels(UIChain[parentPanel].subPanels))
          {
            break;
          }

          foreach (GameObject indicator in UIChain[parentPanel].indicators)
          {
            if (indicator == null) Debug.LogWarning($"ParentPanel child null - {parentPanel}");
            indicator.SetActive(true);
          }
          parentPanel = UIChain[parentPanel].parent;
        }
      }
      else
      {
        Debug.Log("Panel not found");
      }
    }

    private bool CheckSubPanels(List<UIPanel> subPanels)
    {
      if (subPanels == null || subPanels.Count < 1) { return false; }

      for (int i = 0; i < subPanels.Count; i++)
      {
        if (subPanels[i] == UIPanelHandler.ReturnCurrentPanel())
        {
          return true;
        }
        else
        {
          if (CheckSubPanels(UIChain[subPanels[i]].subPanels))
          {
            return true;
          }
        }
      }
      return false;
    }

    public void DisableUILink(UIPanel panel)
    {
      if (UIChain.ContainsKey(panel) && UIChain[panel].indicators != null)
      {
        foreach (GameObject item in UIChain[panel].indicators)
        {
          if(item is null)
          {
            Debug.Log($"Null value found in the indicators of : {panel}");
          }
          else
          {
          item.SetActive(false);
          }
        }
        active.Remove(panel);
      }
      else
      {
        if (panel == UIPanel.None) return;
        Debug.Log($"UI Link does not exist : {panel}");
      }
    }

    #endregion


  }

  [System.Serializable]
  public class UILink
  {
    public UIPanel panel;
    public UIPanel parent;
    public List<UIPanel> subPanels;
    public List<GameObject> indicators;


    public UILink(UIPanel panel)
    {
      this.panel = panel;
    }
  }

  public enum UIPanel
  {
    None = 0,
    News = 1,
    Shop = 6,
    History = 7,
    History_History = 8,
    History_Leaderboard = 9,
    Plan = 10,
    Plan_Focus = 11,
    Plan_Mindset = 12,
    Plan_Scavenger = 3,
    Plan_Counter = 4,
    ActiveEffects = 13,
    Map = 14,
    Settings = 15,
    League = 16,
    SiegeEnd = 17,
    Book = 18,
    Book_Spell = 19,
    Book_Spell_Info = 20,
    Book_Inventory = 21,
    Book_Inventory_Gear = 23,
    Book_Inventory_Crafting = 24,
    Book_Inventory_Crafting_Recipes = 25,
    Book_Inventory_Crafting_Orders = 26,
    Book_Inventory_Void = 27,
    Book_Profile = 28,
    Book_Profile_Milestone = 29,
    Book_Profile_SkillTree = 34,
    Book_Profile_SkillTree_Combat = 35,
    Book_Profile_SkillTree_Utility = 36,
    Book_Profile_Stats = 37,
    Book_Profile_Stats_General = 38,
    Book_Profile_Stats_Misc = 39,
    News_News = 48,
    Book_Spell_Info_Info = 49,
    Book_Spell_Info_Statistics = 50,
    Book_Spell_Info_Upgrade = 51,
    History_History_Information = 55,
    Town = 56,
    Town_Recipes = 57,
    Town_Power = 58,
    Town_Farm = 59
  }



}
