using UnityEngine;

namespace MageAFK.Animation
{
  public class WavingScript : MonoBehaviour
  {
    public float maxRotation = 20f;
    public float speed = 2f;
    private float offset;  // New offset variable


    private void OnEnable() => offset = Random.Range(0f, 1f);
    private void OnDisable() => transform.rotation = Quaternion.Euler(0, 0, 0);

    private void Update()
    {
      // Calculates a rotation angle based on the sine of the (time + offset) multiplied by speed. 
      // This will make the object oscillate between -maxRotation and maxRotation over time.
      float rotationZ = maxRotation * Mathf.Sin((Time.unscaledTime + offset) * speed);

      // Applies the rotation to the GameObject
      transform.rotation = Quaternion.Euler(0, 0, rotationZ);
    }
  }
}