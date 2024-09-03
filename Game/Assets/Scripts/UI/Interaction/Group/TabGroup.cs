using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MageAFK.UI

{
  public abstract class TabGroup : SerializedMonoBehaviour
  {
    [Header("Tab Group Options")]
    [SerializeField] protected List<TabButton> tabButtons;
    [ReadOnly] protected TabButton selectedTab;


    public virtual void Subscribe(TabButton button)
    {
      if (tabButtons == null)
      {
        tabButtons = new List<TabButton>();
      }

      if (tabButtons.Contains(button)) { return; }
      tabButtons.Add(button);
    }


    public virtual void OnTabSelected(TabButton button)
    {

    }

    public virtual void ResetTabs()
    {

    }


  }
}

