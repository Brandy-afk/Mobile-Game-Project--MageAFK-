using UnityEngine;

namespace MageAFK.Animation
{
  public class BobbingUI : MonoBehaviour
  {
    public float amplitude = 10f; // The amplitude of the bobbing movement
    public float frequency = 1f; // The frequency of the bobbing movement

    private RectTransform rectTransform;
    private Vector3 initialPosition;

    private void Start()
    {
      SwapPositions();
    }
    public void SwapPositions()
    {
      rectTransform = GetComponent<RectTransform>();
      initialPosition = rectTransform.localPosition;
    }

    private void Update()
    {
      float newY = initialPosition.y + Mathf.Sin(Time.unscaledTime * frequency + Mathf.PI / 2) * amplitude;
      rectTransform.localPosition = new Vector3(initialPosition.x, newY, initialPosition.z);
    }
  }
}