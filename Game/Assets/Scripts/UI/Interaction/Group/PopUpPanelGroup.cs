using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace MageAFK.UI
{
  public class PopUpPanelGroup : TabGroup
  {
    [SerializeField] private List<UISwapPair> objectsToSwap;
    [SerializeField] private Color onColor;
    [SerializeField] private Color offColor;
    private bool resetPanelOnOpen = true;

    public event Action<UIPanel> OnTabPanelChanged;

    private Dictionary<UIPanel, UISwapPair> swappables;


    [Header("Do not alter")]
    private UIPanel currentPanel;

    private void Awake()
    {
      swappables = new Dictionary<UIPanel, UISwapPair>();

      foreach (UISwapPair pair in objectsToSwap)
      {
        swappables.Add(pair.name, pair);
      }

      currentPanel = objectsToSwap[0].name;

      ResetTabs();
      SetSwappable();
    }

    private void OnEnable()
    {

      if (resetPanelOnOpen)
      {
        OpenPanel(objectsToSwap[0].name);
        return;
      }
      SetCurrentPanel();
    }


    public void InvokePanelEvent()
    {
      OnTabPanelChanged?.Invoke(currentPanel);
    }


    public override void OnTabSelected(TabButton button)
    {
      IUIPanelProvider b = button as IUIPanelProvider;
      if (currentPanel == b.ReturnPanel()) { return; }

      OnTabPanelChanged?.Invoke(b.ReturnPanel());
      OpenPanel(b.ReturnPanel());
    }


    public void OpenPanel(UIPanel panel)
    {
      currentPanel = panel;
      ResetTabs();
      SetSwappable();
      SetCurrentPanel();
      InvokePanelEvent();
    }

    public void SetCurrentPanel()
    {
      UIPanelHandler.SetCurrentPanel(currentPanel);
    }

    private void SetSwappable()
    {
      swappables[currentPanel].panel.SetActive(true);
    }

    public override void ResetTabs()
    {
      foreach (TabButton button in tabButtons)
      {
        IUIPanelProvider popUpButton = button as IUIPanelProvider;
        Image image = button.GetComponent<Image>();
        image.color = offColor;

        if (swappables[popUpButton.ReturnPanel()] == null) { Debug.Log("NULL"); }
        swappables[popUpButton.ReturnPanel()].panel.SetActive(false);
        if (popUpButton.ReturnPanel() == currentPanel) { image.color = onColor; }
      }
    }


  }




}
