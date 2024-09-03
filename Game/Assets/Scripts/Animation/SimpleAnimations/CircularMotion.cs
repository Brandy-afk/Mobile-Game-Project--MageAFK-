using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MageAFK.Animation
{
    public class CircularMotion : MonoBehaviour
  {
    public List<RectTransform> rectTransforms;
    public float radius = 10f;
    public float speed = 1f;
    public float delayRange = 1f; // Max delay before starting each coroutine

    private void OnEnable()
    {
      foreach (RectTransform rectTransform in rectTransforms)
      {
        // Start each coroutine with a random delay
        float randomDelay = Random.Range(0f, delayRange);
        StartCoroutine(DelayedCircularMotion(rectTransform, randomDelay));
      }
    }

    private void OnDisable()
    {
      // Stop all coroutines when the script is disabled to stop the motion
      StopAllCoroutines();
    }

    private IEnumerator DelayedCircularMotion(RectTransform rectTransform, float delay)
    {
      yield return new WaitForSeconds(delay);
      StartCoroutine(CircularMotionCoroutine(rectTransform));
    }

    private IEnumerator CircularMotionCoroutine(RectTransform rectTransform)
    {
      float angle = 0f;
      while (true)
      {
        // Increment the angle based on the speed set
        angle += Time.deltaTime * speed;

        // Calculate the new position of the UI object using the angle and radius
        float x = Mathf.Cos(angle) * radius;
        float y = Mathf.Sin(angle) * radius;
        Vector2 offset = new(x, y);

        // Set the new position of the UI object
        rectTransform.anchoredPosition = offset;

        yield return null;
      }
    }
  }
}
