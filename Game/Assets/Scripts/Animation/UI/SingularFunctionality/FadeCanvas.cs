using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MageAFK.UI
{
    //Script Summary:
    //Fades out canvas groups if the user has not interacted with screen based on a certain amount of time. 
    //DEPENDANT
    //This script is specifically for game overlay. 
    public class FadeCanvas : MonoBehaviour
  {
    [SerializeField] private OverlayAnimationHandler gameOverlayGroup;
    [SerializeField] private List<CanvasGroup> canvasGroups;
    [SerializeField] private float idleTime = 5f; // Time in seconds to wait before fade out starts
    [SerializeField] private float fadeOutTime = 2f; // Time in seconds over which fade out happens
    [SerializeField] private float fadeInTime = 2f; // Time in seconds over which fade in happens

    [SerializeField] private float minAlpha = .25f;

    private Coroutine fadeCoroutine;

    private void OnEnable()
    {
      fadeCoroutine = StartCoroutine(FadeOutAfterDelay());
    }

    private void OnDisable()
    {
      foreach (var canvasGroup in canvasGroups)
        canvasGroup.alpha = 1;
    }

    private void Update()
    {
      // If the screen was touched, fade in
      if (Input.GetMouseButtonDown(0))
      {
        // If a fade coroutine was running, stop it
        if (fadeCoroutine != null)
          StopCoroutine(fadeCoroutine);

        // Start a new fade coroutine
        fadeCoroutine = StartCoroutine(FadeIn());

      }
    }

    IEnumerator FadeOutAfterDelay()
    {
      // Wait for idle time
      yield return new WaitForSeconds(idleTime);

      // Start fade out
      for (float t = 0; t < fadeOutTime; t += Time.deltaTime)
      {
        foreach (var canvasGroup in canvasGroups)
          canvasGroup.alpha = Mathf.Lerp(1, minAlpha, t / fadeOutTime);
        yield return null;
      }

      // Make sure canvas groups are fully transparent at the end of the fade
      foreach (var canvasGroup in canvasGroups)
        canvasGroup.alpha = minAlpha;
    }

    IEnumerator FadeIn()
    {
      // Current alpha is at minAlpha, we start fading in from here
      float startAlpha = canvasGroups[0].alpha;

      // Start fade in
      for (float t = 0; t < fadeInTime; t += Time.deltaTime)
      {
        foreach (var canvasGroup in canvasGroups)
          canvasGroup.alpha = Mathf.Lerp(startAlpha, 1, t / fadeInTime);
        yield return null;
      }

      // Make sure canvas groups are fully visible at the end of the fade
      foreach (var canvasGroup in canvasGroups)
        canvasGroup.alpha = 1;

      // Once the fade in is complete, we start the fade out after delay again
      fadeCoroutine = StartCoroutine(FadeOutAfterDelay());
    }
  }
}
