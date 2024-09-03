using System;
using MageAFK.Animation;
using MageAFK.Core;
using MageAFK.Management;
using MageAFK.Player;
using MageAFK.Pooling;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MageAFK.UI
{
  //Script Summary:
  //Handles game overlay interaction and object animations based on those interactions.
  public class OverlayAnimationHandler : MonoBehaviour
  {
    [Space(10)]
    [Header("Book")]
    [SerializeField] private BookUIReference bookReferences;
    private Vector3 bookOrginLocation;

    [Space(10)]
    [Header("VisualToggles")]
    [SerializeField] private SlideUIClass[] visualToggles;
    [SerializeField] private float usableAnimSpd;


    [Space(10)]
    [Header("Player Health Indicator")]
    [SerializeField] private PlayerHealthText healthRef;


    [Header("References")]
    [SerializeField] private BookTabGroup bookTabGroup;
    [SerializeField] private GameObject overlayUIPanel;
    [SerializeField] private ObjectPooler objectPooler;

    private static CanvasGroup mainGroup;

    #region Interaction Locking
    private static bool animationLock = false;
    public static void SetIsAnimating(bool state)
    {
      if (!state && animationLock) return;
      mainGroup.interactable = !state;
    }
    public static void ToggleLock(bool state) => animationLock = state;

    #endregion

    #region  Classes
    [Serializable]
    public class PlayerHealthText
    {
      public Color posColor, negColor;
      public Vector2 intialPos;
      public Vector2[] targetPos;
      public float[] aSpeeds;
    }

    [Serializable]
    public class BookUIReference
    {
      public GameObject bookUI;
      public Animator bookAnimator;

      [Header("Book Fields")]
      public float bookAnimationSpeed;


      public RectTransform bookArea;
      public Image bookImage;
      public Sprite regularBookSprite;
      public GameObject bookLining;
      public GameObject bookMarkGroup;

      [Header("Currency Panel")]
      public CanvasGroup currencyGroup;
      public float fadeTime;

    }

    [System.Serializable]
    public class SlideUIClass
    {
      public RectTransform rect;
      [Header("Orgin will be set in code")]
      public Vector2 origin;
      public Vector2 target;
    }


    #endregion

    private void Awake()
    {
      bookOrginLocation = new Vector3(0f, bookReferences.bookArea.anchoredPosition.y);
      mainGroup = transform.GetChild(0).GetComponent<CanvasGroup>();
      WaveHandler.SubToSiegeEvent(OnSiegeEvent, true);

    }
    private void OnSiegeEvent(Status state)
    {
      if (state == Status.Start)
        ServiceLocator.Get<PlayerHealth>().OnHealthChangedByAmount += OnPlayerHealthAffected;
      else
        ServiceLocator.Get<PlayerHealth>().OnHealthChangedByAmount -= OnPlayerHealthAffected;
    }



    #region Book Button

    //Opening
    public void StartOpenBookAnimation()
    {
      bookReferences.bookUI.SetActive(true);
      bookReferences.bookArea.gameObject.SetActive(true);
      UIAnimations.Instance.SlideUpPerfect(bookReferences.bookArea, bookReferences.bookAnimationSpeed, OnBookSlideUpFinished, true);
    }

    public void OnBookSlideUpFinished()
    {
      overlayUIPanel.SetActive(false);
      bookReferences.bookAnimator.SetTrigger("triggerOpen");
      bookReferences.currencyGroup.gameObject.SetActive(true);
      UIAnimations.Instance.FadeIn(bookReferences.currencyGroup, bookReferences.fadeTime);
    }

    public void OnOpenBookAnimationFinished()
    {
      bookReferences.bookAnimator.SetTrigger("triggerIdle");
      bookReferences.bookLining.SetActive(true);
      bookReferences.bookMarkGroup.SetActive(true);
      bookTabGroup.StartBookMarkPosRoutine(true, () => { SetIsAnimating(false); });

    }


    //Closing 
    public void OnBookCloseButtonPressed()
    {
      UIPanelHandler.SetCurrentPanel(UIPanel.None);
      bookTabGroup.StartBookMarkPosRoutine(false, OnBookMarkAnimationFinished);
      UIAnimations.Instance.FadeOut(bookReferences.currencyGroup, bookReferences.fadeTime, () => { bookReferences.currencyGroup.gameObject.SetActive(false); });
    }

    public void OnBookMarkAnimationFinished()
    {
      bookReferences.bookImage.sprite = bookReferences.regularBookSprite;
      bookReferences.bookLining.SetActive(false);
      bookReferences.bookMarkGroup.SetActive(false);
      bookReferences.bookAnimator.SetTrigger("triggerClose");
    }

    public void OnBookClosedCallBack()
    {
      overlayUIPanel.SetActive(true);
      UIAnimations.Instance.SlideLocal(bookReferences.bookArea, bookOrginLocation, bookReferences.bookAnimationSpeed + .1f, OnBookAreaClose);
    }

    public void OnBookAreaClose()
    {
      bookReferences.bookUI.SetActive(false);
      SetIsAnimating(false);
    }



    #endregion

    #region Usables Panel

    //Specifically for toggling the usables panel on and off based on the button pressed
    public void ToggleUsables()
    {

      if (UIAnimations.Instance.CheckIfActiveAnimation(visualToggles[0].rect.gameObject)) return;
      bool state = visualToggles[0].rect.gameObject.activeInHierarchy;

      for (int i = 0; i < visualToggles.Length; i++)
      {
        if (!state) visualToggles[i].rect.gameObject.SetActive(true);
        var value = i;
        UIAnimations.Instance.SlideLocal(visualToggles[i].rect
                                        , state ? visualToggles[i].target : visualToggles[i].origin
                                        , usableAnimSpd
                                        , state ? () => { visualToggles[value].rect.gameObject.SetActive(false); }
        : null
                                        , LeanTweenType.easeOutSine);
      }
    }



    #endregion

    #region Player Health Animation

    private void OnPlayerHealthAffected(float change)
    {
      var instance = objectPooler.GetFromPool(PoolingObjects.PlayerDamageText)?.GetComponent<TMP_Text>();
      if (instance == null) return;
      instance.color = change < 0 ? healthRef.negColor : healthRef.posColor;
      instance.rectTransform.localPosition = healthRef.intialPos;
      instance.text = Math.Abs(change).ToString("N1");
      instance.gameObject.SetActive(true);

      UIAnimations.Instance.SlideLocal(instance.rectTransform, healthRef.targetPos[0], healthRef.aSpeeds[0], () => { OnInitialSlideOver(instance); }, LeanTweenType.easeInOutSine, null, false);
    }

    private void OnInitialSlideOver(TMP_Text instance)
    {
      if (!gameObject.activeInHierarchy) { instance.gameObject.SetActive(false); return; }
      StartCoroutine(UIAnimations.Instance.FadeText(instance, healthRef.aSpeeds[1], 0, false));
      UIAnimations.Instance.SlideLocal(instance.rectTransform, healthRef.targetPos[1], healthRef.aSpeeds[1], () => { instance.gameObject.SetActive(false); }, LeanTweenType.easeInOutSine, null, false);
    }

    #endregion
  }



}