using System;
using MageAFK.Management;

namespace MageAFK.UI
{
  public static class UIPanelHandler
  {
    private static UIPanel currentPanel;
    private static event Action<UIPanel> OnPanelChanged;
    public static void SetCurrentPanel(UIPanel panel)
    {
      currentPanel = panel;
      ServiceLocator.Get<IndicatorHandler>().DisableUILink(panel);
      InvokeEvent();
    }
    public static void SubscribeToPanelChanged(Action<UIPanel> subscriber, bool state)
    {
      if (state)
      {
        OnPanelChanged += subscriber;
        InvokeEvent();
      }
      else
      {
        OnPanelChanged -= subscriber;
      }
    }
    public static UIPanel ReturnCurrentPanel() => currentPanel;
    private static void InvokeEvent() => OnPanelChanged?.Invoke(currentPanel);

    

    }
}

