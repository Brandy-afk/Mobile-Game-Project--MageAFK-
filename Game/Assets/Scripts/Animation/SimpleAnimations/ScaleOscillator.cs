using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleOscillator : MonoBehaviour
{
  public List<GameObject> objectsToScale; // The objects that will be scaled
  public Vector3 minScale; // The smallest scale size
  public Vector3 maxScale; // The largest scale size
  public float duration = 2f; // Duration for one complete cycle (from minScale to maxScale and back)
  public bool delayStart = false;
  private void OnEnable()
  {
    foreach (GameObject obj in objectsToScale)
    {
      if (!obj.activeInHierarchy) { continue; }
      StartCoroutine(StartScaleObject(obj));
    }
  }

  IEnumerator StartScaleObject(GameObject obj)
  {
    // Add a random delay between 0 and 1 second
    if (delayStart)
    {
      yield return new WaitForSeconds(Random.Range(0f, 1f));
    }

    // Then start scaling
    StartCoroutine(ScaleObject(obj));
  }

  private void OnDisable()
  {
    StopAllCoroutines();
  }

  IEnumerator ScaleObject(GameObject obj)
  {
    float elapsed = 0f;
    while (true)
    {
      elapsed += Time.deltaTime / duration;
      // PingPong will make the value go back and forth between 0 and 1
      float scaleValue = Mathf.PingPong(elapsed, 1);
      // Use SmoothStep to create a smoother transition
      Vector3 currentScale = new(
          Mathf.SmoothStep(minScale.x, maxScale.x, scaleValue),
          Mathf.SmoothStep(minScale.y, maxScale.y, scaleValue),
          Mathf.SmoothStep(minScale.z, maxScale.z, scaleValue)
      );
      obj.transform.localScale = currentScale;
      yield return null;
    }
  }
}
