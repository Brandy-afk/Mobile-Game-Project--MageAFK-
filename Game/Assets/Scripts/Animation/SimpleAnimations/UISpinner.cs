
using System.Collections;
using UnityEngine;


namespace MageAFK.UI
{
    public class UISpinner : MonoBehaviour
{
    [SerializeField]
    private float spinSpeed = 360f; // Default spin speed in degrees per second

    private Coroutine currentSpinCoroutine;

    private void OnEnable()
    {
        // Start the spin coroutine when the object is enabled
        currentSpinCoroutine = StartCoroutine(SpinCoroutine());
    }

    private void OnDisable()
    {
        // Stop the spin coroutine when the object is disabled
        if (currentSpinCoroutine != null)
        {
            StopCoroutine(currentSpinCoroutine);
            currentSpinCoroutine = null;
        }
    }

    private IEnumerator SpinCoroutine()
    {
        while (true)
        {
            // Rotate the object around its up vector at speed degrees/second
            transform.Rotate(new Vector3(0f, 0f, spinSpeed), spinSpeed * Time.unscaledDeltaTime);

            // Yield control back to Unity until the next frame
            yield return null;
        }
    }
}

}