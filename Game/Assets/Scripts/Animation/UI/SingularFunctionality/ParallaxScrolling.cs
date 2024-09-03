using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MageAFK.UI
{
  public class ParallaxScrolling : MonoBehaviour
  {
    [System.Serializable]
    public class ScrollableImage
    {
      public RawImage image;
      public Vector2 scrollSpeed = new(0.5f, 0.5f);
    }

    [SerializeField]
    private List<ScrollableImage> scrollableImages;

    void Update()
    {
      for (int i = 0; i < scrollableImages.Count; i++)
      {
        float offsetX = Time.time * scrollableImages[i].scrollSpeed.x % 1;
        float offsetY = Time.time * scrollableImages[i].scrollSpeed.y % 1;
        scrollableImages[i].image.uvRect = new Rect(offsetX, offsetY, 1, 1);
      }
    }
  }

}