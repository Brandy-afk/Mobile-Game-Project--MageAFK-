using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MageAFK.Management;
using MageAFK.Core;

namespace MageAFK.Animation
{
  public class UIAnimations : MonoBehaviour
  {
    public static UIAnimations Instance;

    [SerializeField] private float textScaleFactor;
    [SerializeField] private float textAnimationDuration;


    [Header("Pop Up Animation")]
    [SerializeField] private float popUpSpeed = .5f;

    [Header("Sliders")]

    [SerializeField] private float sliderDuration;
    [SerializeField] private float maxSliderVariation;
    [SerializeField] private float minSliderVariation;
    [SerializeField] private Gradient healthGradient;


    [Header("Sliding")]
    [SerializeField] private RectTransform currencyPanelRectTransform;

    [Header("Curve Manipulation Fields")]
    [SerializeField] private float arcHeight = 0.5f;
    [SerializeField] private float controlPointPosition = 1 / 3f;


    private void Awake()
    {
      if (Instance == null)
        Instance = this;
    }


    #region Management

    private Dictionary<GameObject, LTDescr> activeTweens = new();
    private Dictionary<GameObject, Coroutine> activeCoroutines = new();

    public bool TryStopCoroutine(GameObject obj)
    {
      if (activeCoroutines.TryGetValue(obj, out Coroutine routine))
      {
        if (routine != null) StopCoroutine(routine);
        activeCoroutines.Remove(obj);
        return true;
      }
      return false;
    }
    public bool TryStopTween(GameObject obj)
    {
      if (activeTweens.TryGetValue(obj, out LTDescr tween))
      {
        LeanTween.cancel(tween.uniqueId);
        activeTweens.Remove(obj);
        return true;
      }
      return false;
    }

    public bool CheckIfActiveAnimation(GameObject obj)
    {
      return activeCoroutines.ContainsKey(obj) || activeTweens.ContainsKey(obj);
    }

    #endregion

    #region Generics

    public void TransitionFloat(GameObject gameObject, float fromValue, float toValue, float duration, Action<float> onUpdate, Action onComplete, LeanTweenType type)
    {
      TryStopTween(gameObject);
      activeTweens[gameObject] = LeanTween.value(gameObject, fromValue, toValue, duration)
                .setEase(type)
                .setOnUpdate((float value) => { onUpdate?.Invoke(value); })
                .setIgnoreTimeScale(true)
                .setOnComplete(() => { onComplete?.Invoke(); activeTweens.Remove(gameObject); });
    }

    #endregion

    #region String Animation


    public IEnumerator FadeText(TMP_Text text, float duration, float targetA = 0, bool ignoreTimeScale = true)
    {
      Color initialColor = text.color;
      float elapsed = 0;

      while (elapsed < duration)
      {
        elapsed += ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
        float alpha = Mathf.Lerp(initialColor.a, targetA, elapsed / duration);  // lerp alpha to 0 (transparent)
        text.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);  // create new color with faded alpha
        yield return null;
      }

      text.color = new Color(initialColor.r, initialColor.g, initialColor.b, targetA);  // set color to fully transparent
    }

    public void AnimateTextScale(TMP_Text targetText)
    {
      Vector3 initialScale = new(1, 1, 1);

      // Scale up the text
      LeanTween.scale(targetText.gameObject, initialScale * textScaleFactor, textAnimationDuration / 2f)
          .setEase(LeanTweenType.easeOutSine)
          .setIgnoreTimeScale(true)
          .setOnComplete(() =>
          {
            // Scale down the text back to the initial scale
            LeanTween.scale(targetText.gameObject, initialScale, textAnimationDuration / 2f)
                  .setEase(LeanTweenType.easeInSine);
          });
    }


    // Transition Number Text 
    private Dictionary<TMP_Text, float> displayedValues = new();

    public void UpdateTextNumberTransition(TMP_Text textField, float startValue, float endValue, float duration)
    {
      // If the text object is not active, set the value directly and return
      if (!textField.gameObject.activeInHierarchy)
      {
        displayedValues[textField] = endValue;
        textField.text = endValue.ToString("N0");
        return;
      }

      // If there is an active coroutine for this text field, stop it
      TryStopCoroutine(textField.gameObject);

      // If there is a displayed value for this text field, use it as start value
      if (displayedValues.TryGetValue(textField, out var currentDisplayedValue))
      {
        startValue = currentDisplayedValue;
      }

      // Start a new coroutine
      activeCoroutines.Add(textField.gameObject, StartCoroutine(AnimateTextCount(textField, startValue, endValue, duration)));
    }

    private IEnumerator AnimateTextCount(TMP_Text textField, float startValue, float endValue, float duration)
    {
      float timer = 0;
      while (timer <= duration)
      {
        // Calculate the current value by linear interpolation
        float currentValue = Mathf.Lerp(startValue, endValue, timer / duration);
        displayedValues[textField] = currentValue;

        // Update the text
        textField.text = Mathf.RoundToInt(currentValue).ToString("N0");

        // Increment the timer and wait until next frame
        timer += Time.unscaledDeltaTime;
        yield return null;
      }

      // Ensure the final value is exactly as expected
      displayedValues[textField] = endValue;
      textField.text = endValue.ToString("N0");

      activeCoroutines.Remove(textField.gameObject);
    }
    #endregion

    #region  FadeIn/Out
    public void FadeOut(CanvasGroup canvasGroup, float fadeOutTime, Action callback = null, bool ignoreTimeScale = true)
    {
      LeanTween.value(canvasGroup.gameObject, 1f, 0f, fadeOutTime)
          .setOnUpdate((float alpha) => { UpdateCanvasGroupAlpha(alpha, canvasGroup); })
          .setOnComplete(() => { if (callback != null) { callback(); } })
          .setDelay(0f)
          .setIgnoreTimeScale(ignoreTimeScale);
    }

    public void FadeIn(CanvasGroup canvasGroup, float fadeInTime, float delayBetweenFades = 0, Action callback = null, bool ignoreTimeScale = true)
    {
      LeanTween.value(canvasGroup.gameObject, 0f, 1f, fadeInTime)
          .setOnUpdate((float alpha) => { UpdateCanvasGroupAlpha(alpha, canvasGroup); })
          .setOnComplete(() => { if (callback != null) { callback(); } })
          .setDelay(delayBetweenFades)
          .setIgnoreTimeScale(ignoreTimeScale);
    }

    private void UpdateCanvasGroupAlpha(float alpha, CanvasGroup canvasGroup)
    {
      canvasGroup.alpha = alpha;
    }
    #endregion

    #region ColorAlteration

    public void ChangeUIColor(Image image, Color targetColor, float duration)
    {
      StartCoroutine(ChangeUIColorRoutine(image, targetColor, duration));
    }

    private IEnumerator ChangeUIColorRoutine(Image image, Color targetColor, float duration)
    {
      Color initialColor = image.color;
      float elapsed = 0;

      while (elapsed < duration)
      {
        elapsed += Time.unscaledDeltaTime;
        image.color = Color.Lerp(initialColor, targetColor, elapsed / duration);
        yield return null;
      }

      image.color = targetColor;
    }

    public void TransitionUIAlpha(Image image, float startAlpha, float endAlpha, float speed, Action callback = null) =>
    StartCoroutine(TransitionUIAlphaRoutine(image, startAlpha, endAlpha, speed, callback));
    private IEnumerator TransitionUIAlphaRoutine(Image image, float startAlpha, float endAlpha, float speed, Action callback = null)
    {
      float t = 0;

      while (t < speed)
      {
        // Increase the time that has passed
        t += Time.unscaledDeltaTime;

        // Calculate the new alpha
        float newAlpha = Mathf.Lerp(startAlpha, endAlpha, t / speed);

        // Set the new alpha
        Color color = image.color;
        color.a = newAlpha;
        image.color = color;
        yield return null; // Wait for the next frame
      }

      // Ensure the alpha ends up at exactly endAlpha
      Color finalColor = image.color;
      finalColor.a = endAlpha;
      image.color = finalColor;

      if (callback != null) { callback(); }
      // Disable the image if the alpha is zero
      if (endAlpha == 0f)
      {
        image.gameObject.SetActive(false);
      }
    }
    #endregion;

    #region Pop Up Animations
    public void OpenPanel(GameObject panel, Action callback = null)
    {
      TryStopTween(panel);

      panel.transform.localScale = Vector3.one * 0.75f; // Set initial scale
      panel.SetActive(true); // Set the panel active
      LTDescr ltDescr = LeanTween.scale(panel, Vector3.one, popUpSpeed)
            .setEaseOutBack()
            .setIgnoreTimeScale(true)
            .setOnComplete(() =>
            {
              if (callback != null) { callback(); }
              activeTweens.Remove(panel);
            }); // Animate scale to 1

      activeTweens.Add(panel, ltDescr); //Add it to active animations
    }

    public void ClosePanel(GameObject panel, Action callback = null)
    {
      TryStopTween(panel);

      LTDescr ltDescr = LeanTween.scale(panel, Vector3.one * 0.75f, popUpSpeed)
            .setEaseInBack()
            .setIgnoreTimeScale(true)
            .setOnComplete(() =>
            {
              panel.SetActive(false);
              if (callback != null) { callback(); }
              activeTweens.Remove(panel);
            }); // Deactivate panel after animation

      activeTweens.Add(panel, ltDescr); //Add it to active animations
    }

    public void PopUpObject(GameObject obj, Vector3 intialScale, Vector3 targetScale, float speed)
    {
      TryStopTween(obj);
      obj.transform.localScale = intialScale; // Set initial scale
      activeTweens[gameObject] = LeanTween.scale(obj, targetScale, speed).setEaseOutBack().setIgnoreTimeScale(true).setOnComplete(() => activeTweens.Remove(obj)); // Animate scale to 1
    }


    #endregion

    #region Slide

    public void Slide(RectTransform panelRectTransform, Vector3 targetLocation, float animationTime = .25f, Action callback = null, bool ignoreTimeScale = true)
    {
      LeanTween.move(panelRectTransform.gameObject, targetLocation, animationTime).setEase(LeanTweenType.easeOutSine).setIgnoreTimeScale(ignoreTimeScale).setOnComplete(callback);
    }

    public void SlideLocal(RectTransform panelRectTransform, Vector2 targetLocation, float animationTime = .25f, Action callback = null,
    LeanTweenType easeType = LeanTweenType.easeOutSine, Vector2? referenceLocation = null, bool ignoreTimeScale = true)
    {
      TryStopTween(panelRectTransform.gameObject); // Stop any animations on the element

      if (referenceLocation != null)
      {
        float distanceRatio = Mathf.Clamp
                                  (Vector2.Distance(panelRectTransform.localPosition, targetLocation) /
                                  Vector2.Distance((Vector2)referenceLocation, targetLocation), 0.1f, 1f); // Clamp to avoid division by zero
                                                                                                           // Adjust the duration based on the distance ratio
        animationTime *= distanceRatio;
      }

      if (!panelRectTransform.localPosition.Equals(targetLocation))
      {
        LTDescr descr = LeanTween.moveLocal(panelRectTransform.gameObject, targetLocation, animationTime)
                  .setEase(easeType)
                  .setIgnoreTimeScale(ignoreTimeScale)
                  .setOnComplete(() =>
                  {
                    activeTweens.Remove(panelRectTransform.gameObject);
                    if (callback != null) callback();
                  });

        activeTweens.Add(panelRectTransform.gameObject, descr);
      }
    }

    public void SlideAndBounce(RectTransform objectToSlide, Vector3 targetPos, Vector3 bounceVariance, float animationTime = .25f, Action callback = null)
    {

      LeanTween.moveLocal(objectToSlide.gameObject, targetPos + bounceVariance, animationTime)
          .setEase(LeanTweenType.easeOutSine)
          .setIgnoreTimeScale(true)
          .setOnComplete(() =>
          {
            LeanTween.moveLocal(objectToSlide.gameObject, targetPos, animationTime / 4f)
              .setEase(LeanTweenType.easeOutSine).setIgnoreTimeScale(true).setOnComplete(callback);
          });
    }

    // public void BounceAndSlide(RectTransform objectToSlide, Vector3 targetPos, Vector3 bounceVariance, float animationTime = .25f, Action callback = null)
    // {
    //   // First move to bounce position
    //   LeanTween.moveLocal(objectToSlide.gameObject, objectToSlide.localPosition + bounceVariance, animationTime)
    //       .setEase(LeanTweenType.easeOutSine)
    //       .setOnComplete(() =>
    //       {
    //         // After bouncing, slide to the target position
    //         LeanTween.moveLocal(objectToSlide.gameObject, targetPos, animationTime)
    //           .setEase(LeanTweenType.easeOutSine)
    //           .setOnComplete(callback);
    //       });
    // }

    // Method to slide UI elements in a curved path.
    public void CurvedSlide(GameObject uiElement, Vector3 targetPosition, float duration, Action callback, bool ignoreTimeScale = true)
    {
      Vector3 P0 = uiElement.transform.position;
      Vector3 P3 = targetPosition;

      // The control points P1 and P2 are at the specified controlPointPosition of the total distance, respectively.
      // The upward offset is proportional to the specified arcHeight.
      Vector3 P1 = P0 + (P3 - P0) * controlPointPosition + Vector3.up * arcHeight;
      Vector3 P2 = P0 + (P3 - P0) * (1f - controlPointPosition) + Vector3.up * arcHeight;

      // Create a Bezier Path from the starting position, through control points, and ending at targetPosition.
      LTBezierPath bezierPath = new(new Vector3[] { P0, P1, P2, P3 });

      // Start moving the UI element along the Bezier Path with the specified duration.
      // EaseInOutQuad makes the movement start and end slowly but move faster in the middle.
      LeanTween.move(uiElement, bezierPath.pts, duration).setEase(LeanTweenType.easeInOutQuad).setOnComplete(callback).setIgnoreTimeScale(ignoreTimeScale);
    }

    //Perfect UI Placement functions
    public void SlideUpPerfect(RectTransform panelRectTransform, float animationTime = .25f, Action callback = null, bool toTopOfScreen = false)
    {
      // Take into account the scale of the RectTransform
      float panelHeight = panelRectTransform.rect.height * panelRectTransform.lossyScale.y;

      // Calculate the height of the currency panel
      float currencyPanelHeight = currencyPanelRectTransform.rect.height * currencyPanelRectTransform.lossyScale.y;

      float finalPosition;

      if (toTopOfScreen)
      {
        // If toTopOfScreen is true, ignore the currency panel height
        finalPosition = 0;
      }
      else
      {
        // Calculate the final position considering the currency panel height
        finalPosition = -currencyPanelHeight;
      }

      LeanTween.moveLocalY(panelRectTransform.gameObject, finalPosition, animationTime).setEase(LeanTweenType.easeOutSine).setOnComplete(callback).setIgnoreTimeScale(true);
    }

    #endregion

    #region Sliders (Bars)
    private Dictionary<Slider, float> targetValues = new();

    public void AnimateSlider(Slider slider, float targetValue, bool isHealthBar)
    {
      // Update the current target value
      targetValues[slider] = targetValue;
      // If a coroutine for this slider is already running, stop it
      TryStopCoroutine(slider.gameObject);
      // Start a new coroutine for this slider
      activeCoroutines[slider.gameObject] = StartCoroutine(AnimateSliderCoroutine(slider, isHealthBar));
    }

    private IEnumerator AnimateSliderCoroutine(Slider slider, bool isHealthBar)
    {
      float minDuration = sliderDuration - minSliderVariation;
      float maxDuration = sliderDuration + maxSliderVariation;

      Image sliderFill = slider.fillRect.GetComponent<Image>(); // Assuming the fill is an Image component

      while (Mathf.Abs(slider.value - targetValues[slider]) > 0.01f)
      {
        // Calculate the absolute difference between the current and target values
        float valueDifference = Mathf.Abs(targetValues[slider] - slider.value);

        // Map the value difference to a duration
        float duration = Mathf.Lerp(minDuration, maxDuration, valueDifference / slider.maxValue);

        // Compute the new slider value
        float newValue = Mathf.Lerp(slider.value, targetValues[slider], Time.unscaledDeltaTime / duration);

        // Update the slider value
        slider.value = newValue;


        if (isHealthBar)
        {
          float percentage = slider.value / slider.maxValue;
          Color newColor = healthGradient.Evaluate(percentage);
          sliderFill.color = newColor;
        }

        yield return null;
      }

      // Ensure the target value is set exactly after animation
      slider.value = targetValues[slider];

      // Ensure Color is set
      if (isHealthBar)
      {
        float percentage = slider.value / slider.maxValue;
        Color newColor = healthGradient.Evaluate(percentage);
        sliderFill.color = newColor;
      }

      // Remove this coroutine from active coroutines
      activeCoroutines.Remove(slider.gameObject);
    }


    #endregion

    #region GameOverlayButtonPressedAnimation


    public void AnimateButton(GameObject button)
    {
      RectTransform buttonRectTransform = button.GetComponent<RectTransform>();
      StartCoroutine(AnimateButtonPress(buttonRectTransform));
    }

    public void AnimateButton(RectTransform button, Action callback)
    {
      StartCoroutine(AnimateButtonPress(button, callback));
    }

    IEnumerator AnimateButtonPress(RectTransform buttonRectTransform, Action callback = null)
    {
      // Store the original scale
      buttonRectTransform.localScale = new Vector3(1, 1, 1);
      Vector3 originalScale = buttonRectTransform.localScale;

      // Define the scale to which the button will shrink
      Vector3 pressedScale = originalScale * 0.9f;

      // Define the duration of the animation
      float duration = 0.05f;

      // Shrink the button
      for (float t = 0; t < duration; t += Time.unscaledDeltaTime)
      {
        buttonRectTransform.localScale = Vector3.Lerp(originalScale, pressedScale, t / duration);
        yield return null;
      }

      // Make sure the button is fully shrunk
      buttonRectTransform.localScale = pressedScale;

      // Expand the button back to its original size
      for (float t = 0; t < duration; t += Time.unscaledDeltaTime)
      {
        buttonRectTransform.localScale = Vector3.Lerp(pressedScale, originalScale, t / duration);
        yield return null;
      }

      // Make sure the button is fully expanded
      buttonRectTransform.localScale = originalScale;
      callback?.Invoke();
    }



    #endregion

    #region SetInAnimation

    public void SetImageIn(Image image)
    {
      image.transform.localScale = Vector3.one * 1.25f;

      LeanTween.scale(image.gameObject, Vector3.one, .25f).setEaseOutBack().setIgnoreTimeScale(true);
    }

    #endregion


    #region Change Size

    public void ChangeScale(RectTransform rectTransform, Vector2 targetScale, float duration, Vector2? referenceScale = null, bool ignoreTimeScale = true, LeanTweenType type = LeanTweenType.easeInQuad)
    {
      TryStopTween(rectTransform.gameObject);

      if (referenceScale != null)
        duration *= Mathf.Clamp(Vector2.Distance(rectTransform.localScale, targetScale) / Vector2.Distance((Vector2)referenceScale, targetScale), 0.1f, 1f); // Clamp to avoid division by zero
      // Adjust the duration based on the distance ratio

      activeTweens.Add(rectTransform.gameObject, LeanTween.scale(rectTransform, new Vector3(targetScale.x, targetScale.y, 1f), duration)
                                .setEase(type)
                                .setIgnoreTimeScale(ignoreTimeScale)
                                .setOnComplete(() => { activeTweens.Remove(rectTransform.gameObject); }));
    }

    #endregion

    #region Wizard Pressed

    public void OnWizardPressed() => ServiceLocator.Get<MilestoneHandler>().UpdateMileStone(MilestoneID.ClickOnWizard, 1);

    #endregion
  }








}
