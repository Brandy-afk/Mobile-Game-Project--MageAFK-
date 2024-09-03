using Sirenix.OdinInspector;
using UnityEngine;

namespace MageAFK.Testing
{

  [CreateAssetMenu(fileName = "DenseVoidData", menuName = "DENSEVOIDDATA")]
  public class SuctionSpellTesting : ScriptableObject
  {
    public float damageThreshold = 0.5f; // Distance from the center within which damage is applied
    public float suctionPower = 1.0f;
    public float maxForce = 4f;

    public float timeBetweenDamage = .10f;

    [BoxGroup("Tornado")]
    public float travelDistance = 5.0f;
    [BoxGroup("Tornado")]
    public float speed = 5.0f;
    [BoxGroup("Tornado")]
    public float sineWaveMagnitude = 1.0f; // Magnitude of the sine wave (width of the 'S' pattern)
    [BoxGroup("Tornado")]
    public float sineWaveFrequency = 1.0f; // Frequency of the sine wave (how many 'S' curves there will be)


  }

}