using MageAFK.Core;
using MageAFK.Management;
using MageAFK.UI;
using UnityEngine;

public class TimeScaleHandler : MonoBehaviour
{

    [SerializeField] private SiegeOverlayUI overlayUI;
    public float[] timeScales = { 0.5f, 1f, 2f, 4f, 6f }; // Your predefined time scales. Ensure this array is sorted.
    private int currentIndex; // Index of the current time scale in the array

    private void Awake() => ServiceLocator.RegisterService<TimeScaleHandler>(this);


    void Start()
    {
        // Find the index of the default time scale (1f) and set it as the current index
        currentIndex = System.Array.IndexOf(timeScales, 1f);
        if (currentIndex == -1)
        {
            Debug.LogError("Default time scale (1f) not found in the array. Ensure timeScales contains 1f.");
            currentIndex = 0; // Fallback to the first index if 1f is not found
        }

        // Set the initial time scale
        SetTimeScale(timeScales[currentIndex]);


        WaveHandler.SubToWaveState((WaveState state) =>
                                             {
                                                 if (state == WaveState.Intermission) SetTimeScale(1f);
                                                 else if (state == WaveState.Counter) SetScaleToCurrentIndex();
                                             }, true);
    }

    // Increase the time scale to the next value in the array
    public void IncreaseTimeScale()
    {
        if (currentIndex < timeScales.Length - 1)
        {
            currentIndex++;
            SetTimeScale(timeScales[currentIndex]);
        }
    }

    // Decrease the time scale to the previous value in the array
    public void DecreaseTimeScale()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            SetTimeScale(timeScales[currentIndex]);
        }
    }

    // Reset the time scale to default (value of 1)
    public void ResetTimeScale()
    {
        currentIndex = System.Array.IndexOf(timeScales, 1f);
        SetScaleToCurrentIndex();
    }

    public void SetScaleToCurrentIndex() => SetTimeScale(timeScales[currentIndex]);

    // Set the time scale to a specific value
    public void SetTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
        overlayUI.UpdateTimeScaleUI(timeScale);
    }
}