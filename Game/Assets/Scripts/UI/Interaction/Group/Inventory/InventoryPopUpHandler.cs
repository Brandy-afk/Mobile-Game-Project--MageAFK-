using UnityEngine;
using MageAFK.Animation;
using System.Collections.Generic;

namespace MageAFK.UI
{
  public class InventoryPopUpHandler : TabGroup
  {

    // Needs work

    [Header("Game object references")]
    [SerializeField] private Dictionary<UIPanel, UISwapPair> popUpDict;

    private UIPanel current = UIPanel.Book_Inventory;

    #region Button Interaction
    public override void OnTabSelected(TabButton button)
    {
      IUIPanelProvider iPanel = button as IUIPanelProvider;
      if (iPanel == null) { return; }
      OpenNewPanel(iPanel.ReturnPanel());
    }

    private void OpenNewPanel(UIPanel name)
    {
      current = name;
      UIPanelHandler.SetCurrentPanel(current);
      HandlePanelEvent(true);
    }

    public void CloseCurrentPanel()
    {
      if (current == UIPanel.Book_Inventory) { return; }
      HandlePanelEvent(false);
      current = UIPanel.Book_Inventory;
    }

    private void HandlePanelEvent(bool state)
    {
      var uISwap = popUpDict[current];
      if (current == UIPanel.Book_Inventory_Crafting) { TogglePopUp(uISwap, state); return; }
      else ToggleBookPage(uISwap, state);
    }

    private void TogglePopUp(UISwapPair pair, bool state)
    {
      if (state)
      {
        pair.blackMask.SetActive(true);
        UIAnimations.Instance.OpenPanel(pair.panel);
      }
      else
      {
        UIAnimations.Instance.ClosePanel(pair.panel, () => pair.blackMask.SetActive(false));
      }
    }

    private void ToggleBookPage(UISwapPair pair, bool state)
    {
      var buttonsSwap = popUpDict[UIPanel.Book_Inventory];
      buttonsSwap.panel.SetActive(!state);
      pair.panel.SetActive(state);
    }

    #endregion

  }



}