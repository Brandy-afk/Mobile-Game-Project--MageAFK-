using Sirenix.OdinInspector;
using UnityEngine;


namespace MageAFK.Tools
{

    [RequireComponent(typeof(RectTransform))]
  public class RectTools : MonoBehaviour
  {
    [Button("Set Anchors to Size")]
    public void SetAnchorsToCurrentSize()
    {
      RectTransform rectTransform = GetComponent<RectTransform>();

      // Assuming the parent is the canvas or has the same size as the canvas
      Rect parentSize = rectTransform.parent.GetComponent<RectTransform>().rect;

      Vector2 lowerLeft = new(
          rectTransform.localPosition.x - rectTransform.rect.width / 2,
          rectTransform.localPosition.y - rectTransform.rect.height / 2);

      Vector2 upperRight = new(
          rectTransform.localPosition.x + rectTransform.rect.width / 2,
          rectTransform.localPosition.y + rectTransform.rect.height / 2);

      // Convert to relative position
      rectTransform.anchorMin = new Vector2(lowerLeft.x / parentSize.width + 0.5f, lowerLeft.y / parentSize.height + 0.5f);
      rectTransform.anchorMax = new Vector2(upperRight.x / parentSize.width + 0.5f, upperRight.y / parentSize.height + 0.5f);

      // Resetting the anchored position and the sizeDelta to maintain the current size and position
      rectTransform.anchoredPosition = Vector2.zero;
      rectTransform.sizeDelta = Vector2.zero;
    }
  }

}