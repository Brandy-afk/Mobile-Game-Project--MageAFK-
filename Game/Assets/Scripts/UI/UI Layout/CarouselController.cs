using UnityEngine;
using UnityEngine.UI;

namespace MageAFK.UI
{
  [RequireComponent(typeof(ScrollRect))]
  public class CarouselController : MonoBehaviour
  {
    public float maxScale = 1.5f;
    public float minScale = 0.5f;
    public float maxAlpha = 1f;
    public float minAlpha = 0.2f;
    public bool useXAxis = false; // Add this option to switch between Y and X axes

    private ScrollRect scrollRect;
    private RectTransform contentRectTransform;

    private void Awake()
    {
      scrollRect = GetComponent<ScrollRect>();
      contentRectTransform = scrollRect.content;
    }

    private void Update()
    {
      // Calculate the position of the center of the viewport in the local coordinates of the content
      float viewportCenter = useXAxis ? -contentRectTransform.localPosition.x : -contentRectTransform.localPosition.y;

      foreach (RectTransform childRectTransform in contentRectTransform)
      {
        // Calculate the distance from the center of the viewport
        float distanceFromCenter = Mathf.Abs(viewportCenter - (useXAxis ? childRectTransform.localPosition.x : childRectTransform.localPosition.y));

        // Calculate the lerp reference depending on the axis of use
        float lerpReference = useXAxis ? (scrollRect.GetComponent<RectTransform>().rect.width / 2) : (scrollRect.GetComponent<RectTransform>().rect.height / 2);


        // Based on this distance, calculate the desired scale and alpha
        float scale = Mathf.Lerp(maxScale, minScale, distanceFromCenter / lerpReference);
        float alpha = Mathf.Lerp(maxAlpha, minAlpha, distanceFromCenter / lerpReference);

        // Apply the scale and alpha
        childRectTransform.localScale = new Vector3(scale, scale, 1f);
        CanvasGroup canvasGroup = childRectTransform.GetComponent<CanvasGroup>();
        if (canvasGroup)
        {
          canvasGroup.alpha = alpha;
        }
      }
    }
  }
}
