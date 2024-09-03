using System.Collections.Generic;
using UnityEngine;
using MageAFK.Animation;
using MageAFK.Cam;
using Sirenix.OdinInspector;
using MageAFK.Management;
using MageAFK.Player;

namespace MageAFK.UI
{
  public class InGamePopUpHandler : SerializedMonoBehaviour
  {

    [Header("Objects")]

    [SerializeField] private Dictionary<UIPanel, RectTransform> overlayPanels;
    [SerializeField] private GameObject overlayBackground;
    [SerializeField] private MapRef map;
    [SerializeField] private GameObject nonSiegeUI;

    [SerializeField] private GameObject[] townButtons;


    [Header("References")]
    [SerializeField] private OverlayAnimationHandler overlayAnimationHandler;
    [SerializeField] private CameraController cameraController;


    #region Classes


    [System.Serializable]
    private class MapRef
    {
      public RectTransform panel;
      public Vector3 orginLoc;
      public float animationSpeed;
      public LocationUI locationUI;
    }
    #endregion


    private UIPanel currentPanel;


    private void Start()
    {
      map.orginLoc = map.panel.anchoredPosition;
      ServiceLocator.Get<IPlayerDeath>().SubscribeToPlayerDeath(CloseCurrentPanel, true);
    }

    public void CloseCurrentPanel()
    {
      if (UIPanelHandler.ReturnCurrentPanel() == UIPanel.None) { return; }

      if (currentPanel == UIPanel.Map)
        ToggleMapPanel(false);
      else
        if (overlayPanels.ContainsKey(currentPanel))
        TogglePopUp(currentPanel, false);
    }


    #region Popups
    private void TogglePopUp(UIPanel panelType, bool state)
    {
      OverlayAnimationHandler.SetIsAnimating(true);

      currentPanel = state ? panelType : UIPanel.None;
      UIPanelHandler.SetCurrentPanel(currentPanel);

      if (state)
      {
        overlayBackground.gameObject.SetActive(true);
        UIAnimations.Instance.OpenPanel(overlayPanels[panelType].gameObject, () => { OverlayAnimationHandler.SetIsAnimating(false); });
      }
      else
      {
        UIAnimations.Instance.ClosePanel(overlayPanels[panelType].gameObject, () =>
        {
          OverlayAnimationHandler.SetIsAnimating(false);

          overlayBackground.gameObject.SetActive(false);
        });
      }
    }
    public void ToggleHistoryPanels(bool state)
    {
      TogglePopUp(UIPanel.History, state);
    }

    public void TogglePlanPanels(bool state)
    {
      TogglePopUp(UIPanel.Plan, state);

    }

    public void ToggleNewsPanels(bool state)
    {
      TogglePopUp(UIPanel.News, state);

    }
    #endregion

    #region Book
    public void OnBookButtonPressed()
    {
      OverlayAnimationHandler.SetIsAnimating(true);

      UIPanelHandler.SetCurrentPanel(UIPanel.Book);
      overlayAnimationHandler.StartOpenBookAnimation();
    }

    #endregion

    #region Map
    public void ToggleMapPanel(bool state)
    {
      OverlayAnimationHandler.SetIsAnimating(true);

      currentPanel = state ? UIPanel.Map : UIPanel.None;
      UIPanelHandler.SetCurrentPanel(currentPanel);

      if (state)
      {
        map.panel.gameObject.SetActive(state);
        UIAnimations.Instance.SlideUpPerfect(map.panel, map.animationSpeed, () => { OverlayAnimationHandler.SetIsAnimating(false); }, true);
      }
      else
      {
        UIAnimations.Instance.SlideLocal(map.panel, map.orginLoc, map.animationSpeed, () =>
        {
          OverlayAnimationHandler.SetIsAnimating(false);
          map.panel.gameObject.SetActive(state);
        });
      }
    }

    public void ForceCloseMap()
    {
      map.panel.localPosition = map.orginLoc;
      map.panel.gameObject.SetActive(false);
    }

    #endregion

    #region Town UI
    public void ToggleTownUI(bool state)
    {
      OverlayAnimationHandler.SetIsAnimating(true);

      currentPanel = state ? UIPanel.Town : UIPanel.None;
      UIPanelHandler.SetCurrentPanel(currentPanel);
      var cam = state ? CameraLoc.TownChoices : CameraLoc.Start;
      cameraController.SwapToCam(cam);

      nonSiegeUI.SetActive(!state);
      overlayPanels[UIPanel.Town].gameObject.SetActive(state);

      for (int i = 0; i < townButtons.Length; i++) townButtons[i].SetActive(state);

      OverlayAnimationHandler.SetIsAnimating(false);
    }

    private void ToggleTownPanel(UIPanel panel)
    {
      overlayPanels[UIPanel.Town].gameObject.SetActive(false);
      currentPanel = panel;
      overlayPanels[currentPanel].gameObject.SetActive(true);
      UIPanelHandler.SetCurrentPanel(currentPanel);

      CameraLoc cam = CameraLoc.TownChoices;
      switch (panel)
      {
        case UIPanel.Town_Farm:
          cam = CameraLoc.Farm1;
          break;
        case UIPanel.Town_Recipes:
          cam = CameraLoc.Blacksmith;
          break;
        case UIPanel.Town_Power:
          cam = CameraLoc.Alchemist;
          break;
        default:
          break;
      }

      cameraController.SwapToCam(cam);
    }

    public void CloseCurrentTownPanel()
    {
      if (currentPanel == UIPanel.Town)
      {
        ToggleTownUI(false);
      }
      else
      {
        overlayPanels[currentPanel].gameObject.SetActive(false);
        currentPanel = UIPanel.Town;
        UIPanelHandler.SetCurrentPanel(UIPanel.Town);
        overlayPanels[UIPanel.Town].gameObject.SetActive(true);
        cameraController.SwapToCam(CameraLoc.TownChoices);
      }
    }

    public void OnRecipesPressed()
    {
      ToggleTownPanel(UIPanel.Town_Recipes);
    }

    public void OnPowerPressed()
    {
      ToggleTownPanel(UIPanel.Town_Power);
    }

    public void OnFarmPressed()
    {
      ToggleTownPanel(UIPanel.Town_Farm);
    }

    #endregion

  }

}