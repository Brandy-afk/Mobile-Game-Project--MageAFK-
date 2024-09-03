using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class RainbowCycle : MonoBehaviour
{
    private Image imageComponent;

    // Speed of the color cycle
    public float cycleSpeed = 1.0f;

    // Current hue value
    private float currentHue = 0.0f;

    private void Awake()
    {
        imageComponent = GetComponent<Image>();
    }

    private void Update()
    {
        // Increment hue
        currentHue += cycleSpeed * Time.deltaTime;

        // Wrap around once hue goes beyond 1
        if (currentHue > 1.0f)
            currentHue -= 1.0f;

        // Convert hue to RGB color
        Color newColor = Color.HSVToRGB(currentHue, 1, 1);

        // Assign the new color to the image
        imageComponent.color = newColor;
    }
}