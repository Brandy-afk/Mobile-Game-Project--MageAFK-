using System;
using System.Collections;
using System.Collections.Generic;
using MageAFK.Animation;
using MageAFK.Management;
using MageAFK.Player;
using UnityEngine;
using UnityEngine.UI;

namespace MageAFK.UI
{
  public class BookTabGroup : TabGroup
  {


    [SerializeField] private Dictionary<UIPanel, BookPanelSwap> swaps;
    [SerializeField] private RectTransform[] rects;

    [SerializeField] private Sprite onSprite;
    [SerializeField] private Sprite offSprite;

    [Serializable]
    private class BookPanelSwap
    {
      public Image buttonImage;
      public CanvasGroup panelGroup;
    }

    [Header("Animation Options")]
    [SerializeField] private float fadeSpeed;

    [Header("Size control")]
    public float xPos;
    public float offXPos;
    public Vector2 bounceVariance;
    [SerializeField] private float markSlideAnimationSpeed = .25f;
    [SerializeField] private float timeBetweenMarkSlide = .5f;

    [Header("References")]
    [SerializeField] private OverlayAnimationHandler gameOverlayGroup;


    public UIPanel currentBookPanel = UIPanel.Book_Spell;

    private void Awake() => selectedTab = tabButtons[0];
    private void OnEnable()
    {
      UIPanelHandler.SetCurrentPanel(currentBookPanel);
      ServiceLocator.Get<IPlayerDeath>().SubscribeToPlayerDeath(CloseBook, true);
    }

    private void OnDisable() => ServiceLocator.Get<IPlayerDeath>().SubscribeToPlayerDeath(CloseBook, false);



    #region Button Management


    public override void OnTabSelected(TabButton button)
    {
      if (selectedTab == button) { return; }

      IUIPanelProvider tab = button as IUIPanelProvider;
      var incomingBookPanel = tab.ReturnPanel();
      UIPanelHandler.SetCurrentPanel(incomingBookPanel);

      ResetTabs();
      selectedTab = button;

      swaps[incomingBookPanel].buttonImage.sprite = onSprite;
      AnimateSwap(incomingBookPanel);
    }

    public override void ResetTabs()
    {
      foreach (var swap in swaps)
        swap.Value.buttonImage.sprite = offSprite;
    }


    #region Position Managing

    public void StartBookMarkPosRoutine(bool state, Action callback = null)
    {
      StartCoroutine(SetBookMarksPosition(state, callback));
    }

    private IEnumerator SetBookMarksPosition(bool state, Action callback = null)
    {
      foreach (var rect in rects)
      {
        float xValue = !state ? offXPos : xPos;
        UIAnimations.Instance.SlideAndBounce(rect, new Vector2(xValue, rect.localPosition.y), bounceVariance, markSlideAnimationSpeed);
        yield return new WaitForSecondsRealtime(timeBetweenMarkSlide);
      }
      if (callback != null) { callback(); }
    }

    #endregion

    #endregion

    public void CloseBook()
    {
      OverlayAnimationHandler.SetIsAnimating(true);
      UIAnimations.Instance.FadeOut(swaps[currentBookPanel].panelGroup, fadeSpeed, gameOverlayGroup.OnBookCloseButtonPressed);
    }


    #region Animation Swapping

    public void AnimateSwap(UIPanel incoming)
    {
      OverlayAnimationHandler.SetIsAnimating(true);
      UIAnimations.Instance.FadeOut(swaps[currentBookPanel].panelGroup, fadeSpeed, () =>
      {
        swaps[currentBookPanel].panelGroup.gameObject.SetActive(false);
        currentBookPanel = incoming;
        FadeInPanel();
      });
    }

    public void FadeInPanel()
    {
      swaps[currentBookPanel].panelGroup.gameObject.SetActive(true);
      UIAnimations.Instance.FadeIn(swaps[currentBookPanel].panelGroup, fadeSpeed, 0, () => { OverlayAnimationHandler.SetIsAnimating(false); });
    }

    #endregion

  }







}
