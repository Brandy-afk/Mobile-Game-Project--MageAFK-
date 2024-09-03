
using System;
using MageAFK.Stats;
using MageAFK.Tools;
using UnityEngine;

namespace MageAFK.Spells
{

  [CreateAssetMenu(fileName = "SeaCyclone", menuName = "Spells/SeaCyclone")]
  public class SeaCyclone : Ultimate, IPlacableUlt
  {
    private System.Random random = new();
    public override void Activate()
    {
      uses = (int)ReturnStatValue(Stat.SpawnCap);
    }

    public int OnPlaced(Vector2 pos)
    {
      SpellSpawn(iD, pos);
      AppendRecord(SpellRecordID.VortexesConjured, 1);
      return --uses;
    }


    public int ShootWaterShot(int shotNumber, Vector2 pos)
    {
      var instance = SpellSpawn(SpellIdentification.SeaCyclone_WaterBlast, pos, true, iD).transform;
      AppendRecord(SpellRecordID.WaterBlasts, 1);

      int angleMin = 90 * shotNumber;
      int angleMax = angleMin + 90;
      int angle = random.Next(angleMin, angleMax);
      // Convert angle to radians
      float radians = angle * (float)Math.PI / 180;

      // Calculate direction vector
      float x = (float)Math.Cos(radians);
      float y = (float)Math.Sin(radians);

      float finalAngle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;

      instance.rotation = Quaternion.AngleAxis(finalAngle, Vector3.forward);
      Utility.SetVelocity(instance.transform.right, instance.gameObject, ReturnStatValue(Stat.SpellSpeed));

      // Prepare for the next shot
      return (shotNumber + 1) % 4;
    }

    public override void OnCast() { }

  }
}
