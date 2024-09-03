using UnityEngine;
using UnityEngine.EventSystems;

namespace MageAFK.UI
{
  public class UIPanelButton : TabButton, IUIPanelProvider
  {

    [Header("Inventory Panel Options")]
    [SerializeField] private UIPanel panelName;

    public UIPanel ReturnPanel() => panelName;

  }
}
