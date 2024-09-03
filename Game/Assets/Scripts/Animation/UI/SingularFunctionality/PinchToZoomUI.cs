using UnityEngine;
using UnityEngine.UI;

public class PinchToZoomUI : MonoBehaviour
{
  public ScrollRect scrollRect;
  public float zoomSpeed = 0.1f;
  public float maxZoom = 2f;
  public float minZoom = 1f;

  private RectTransform content;
  private float initialDistance;
  private Vector2 initialScale;
  private bool isPinching;

  private void Start()
  {
    if (scrollRect == null)
    {
      Debug.LogWarning("ScrollRect not assigned. Please assign a ScrollRect component.");
      return;
    }

    content = scrollRect.content;
  }

  private void Update()
  {
    if (Input.touchCount == 2)
    {
      Touch touch1 = Input.GetTouch(0);
      Touch touch2 = Input.GetTouch(1);

      if (touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began)
      {
        initialDistance = Vector2.Distance(touch1.position, touch2.position);
        initialScale = content.localScale;
        isPinching = true;

        // Disable ScrollRect component
        scrollRect.enabled = false;
      }
      else if ((touch1.phase == TouchPhase.Ended || touch1.phase == TouchPhase.Canceled) || (touch2.phase == TouchPhase.Ended || touch2.phase == TouchPhase.Canceled))
      {
        isPinching = false;

        // Enable ScrollRect component
        scrollRect.enabled = true;
      }

      if (isPinching)
      {
        float currentDistance = Vector2.Distance(touch1.position, touch2.position);
        float delta = (currentDistance - initialDistance) * Time.deltaTime;

        Vector3 newScale = content.localScale + Vector3.one * delta * zoomSpeed;
        newScale.x = Mathf.Clamp(newScale.x, minZoom, maxZoom);
        newScale.y = Mathf.Clamp(newScale.y, minZoom, maxZoom);

        content.localScale = newScale;
      }
    }
  }
}
