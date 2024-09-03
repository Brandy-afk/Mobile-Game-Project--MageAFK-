using UnityEngine;

namespace MageAFK.UI
{
  [System.Serializable]
  public class UIPlacementInfo
  {
    public Vector2 size;
    public Vector2 position;
    public Vector2 anchorMin;
    public Vector2 anchorMax;
  }
  public static class UITools
  {
    public static void SetPanel(UIPlacementInfo loc, RectTransform rect)
    {
      rect.anchoredPosition = loc.position;
      rect.sizeDelta = loc.size;
      rect.anchorMin = loc.anchorMin;
      rect.anchorMax = loc.anchorMax;
    }

  }


  public interface IOnTabSelected<T>
  {
    void OnTabSelected(T tab);
  }

  public interface ISpriteGetter<Key>
  {
    Sprite GetSprite(Key key);
  }



}
