using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace MageAFK.Animation
{
  public class ImageSwitcher : MonoBehaviour
  {
    public Image image;
    public Sprite sprite1;
    public Sprite sprite2;
    public float switchInterval = 2f;

    private void Start()
    {
      StartCoroutine(SwitchSprites());
    }

    private IEnumerator SwitchSprites()
    {
      while (true)
      {
        // Set the image's sprite to sprite1
        image.sprite = sprite1;
        yield return new WaitForSeconds(switchInterval);

        // Set the image's sprite to sprite2
        image.sprite = sprite2;
        yield return new WaitForSeconds(switchInterval);
      }
    }
  }

}