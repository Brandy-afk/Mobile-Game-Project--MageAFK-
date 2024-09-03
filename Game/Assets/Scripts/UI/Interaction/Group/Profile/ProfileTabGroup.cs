using System.Collections.Generic;
using MageAFK.Animation;
using UnityEngine;

namespace MageAFK.UI
{
  public class ProfileTabGroup : TabGroup
  {
    //TODO needs work.
    [SerializeField] private UISwapPair[] panels;
    [SerializeField] private GameObject blackMask;

    private UIPanel currentProfilePanel = UIPanel.Book_Profile;


    private Dictionary<UIPanel, GameObject> swaps;

    private void Awake()
    {
      swaps = new Dictionary<UIPanel, GameObject>();

      for (int i = 0; i < panels.Length; i++)
      {
        swaps.Add(panels[i].name, panels[i].panel);
      }

    }


    public override void OnTabSelected(TabButton button)
    {

      if (currentProfilePanel != UIPanel.Book_Profile) { return; }

      IUIPanelProvider p = button as IUIPanelProvider;
      if (!swaps.ContainsKey(p.ReturnPanel())) { return; }



      selectedTab = button;

      UIAnimations.Instance.AnimateButton(selectedTab.gameObject);

      OpenPanel(p);

    }

    private void OpenPanel(IUIPanelProvider p)
    {

      currentProfilePanel = p.ReturnPanel();
      UIPanelHandler.SetCurrentPanel(currentProfilePanel);
      blackMask.SetActive(true);
      UIAnimations.Instance.OpenPanel(swaps[currentProfilePanel]);
    }

    public void CloseCurrentPanel()
    {
      if (currentProfilePanel == UIPanel.Book_Profile) { return; };
      UIAnimations.Instance.ClosePanel(swaps[currentProfilePanel], () => { blackMask.SetActive(false); currentProfilePanel = UIPanel.Book_Profile; });
      UIPanelHandler.SetCurrentPanel(UIPanel.Book_Profile);
    }








  }


  [System.Serializable]
  public class UISwapPair
  {
    public GameObject panel;
    public UIPanel name;
    public GameObject blackMask;
  }

}