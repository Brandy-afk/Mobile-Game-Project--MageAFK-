using MageAFK.Stats;
using MageAFK.Tools;
using UnityEngine;


namespace MageAFK.Spells
{

  public class VoidshadeProjectile : DefaultController
  {


    private float time;

    private float currentAngle;
    private float rotationDirection = 1f; // 1 for increasing angle, -1 for decreasing
    private float lastDecisionTime = 0f;

    private float minAngle;
    private float maxAngle;


    protected override void OnEnable()
    {
      if (spell == null) return;

      time = 0;
      currentAngle = transform.rotation.eulerAngles.z;
      minAngle = currentAngle - spell.ReturnStatValue(Stat.Amplitude, false);
      maxAngle = currentAngle + spell.ReturnStatValue(Stat.Amplitude, false);

      base.OnEnable();
    }

    private void Update()
    {
      // Update time
      if (!active) return;

      time += Time.deltaTime;
      // Decide whether to reverse direction
      if (time - lastDecisionTime > spell.ReturnStatValue(Stat.DecisionInterval, false))
      {
        if (Random.value < 0.5f) // 50% chance to reverse direction
        {
          rotationDirection *= -1;
          lastDecisionTime = time;
        }
      }

      // Update angle within min and max bounds
      currentAngle += rotationDirection * spell.ReturnStatValue(Stat.Frequency, false) * Time.deltaTime;
      currentAngle = Mathf.Clamp(currentAngle, minAngle, maxAngle);

      // Reverse direction if hitting min or max bounds
      if (currentAngle == minAngle || currentAngle == maxAngle)
      {
        rotationDirection *= -1;
      }

      // Apply the rotation
      transform.rotation = Quaternion.Euler(0, 0, currentAngle);
      Utility.FlipYSprite(Vector3.zero, Vector3.zero, transform, transform.right.x < 0);
      Utility.SetVelocity(transform.right, gameObject, spell.ReturnStatValue(Stat.SpellSpeed));

    }

  }

}
