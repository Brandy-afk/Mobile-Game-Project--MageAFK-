using MageAFK.Stats;
using UnityEngine;
using System.Collections.Generic;
using MageAFK.Core;
using MageAFK.Tools;
using MageAFK.Management;
using MageAFK.Player;

namespace MageAFK.Spells
{

    [CreateAssetMenu(fileName = "New Skill", menuName = "Spells/EtherealSpike")]
  public class EtherealSpike : Spell
  {
    [Header("Spike variables")]
    [SerializeField]
    private readonly Vector3[] scales =
    {
      new(.75f, .75f ,1),
      new(.5f, .5f ,1),
    };

    private readonly float[] speed =
    {
      .75f,
      .5f
    };

    public override void Activate()
    {
      GameObject instance = SpellSpawn(iD, PlayerController.Positions.SpellSpawn);
      var targetPos = ServiceLocator.Get<EntityTracker>().ReturnBestTarget(Focus).Body;
      var direction = Utility.SetRotation(instance, targetPos, ReturnStatValue(Stat.AimVariance));
      Utility.SetVelocity(direction, instance, ReturnStatValue(Stat.SpellSpeed));

      instance.transform.localScale = Vector3.one;
      instance.GetComponent<EtherealSpikeController>().SetUp(PierceStage.First, null);
    }

    public void OnPierce(Transform transform, PierceStage nextStage, Collider2D collider)
    {
      List<GameObject> projectiles = new();
      for (int i = 0; i < 3; i++)
      {
        GameObject instance = SpellSpawn(iD, transform.position, false);
        instance.GetComponent<EtherealSpikeController>().SetUp(nextStage, collider);
        instance.transform.localScale = scales[(int)nextStage];
        projectiles.Add(instance);
      }

      Vector2 direction = transform.right;
      float angle = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) - 30;

      for (int i = 0; i < 3; i++)
      {
        projectiles[i].SetActive(true);
        projectiles[i].transform.rotation = Quaternion.Euler(0, 0, angle);
        Utility.SetVelocity(direction, projectiles[i], (ReturnStatValue(Stat.SpellSpeed) * speed[(int)nextStage]));

        angle += 30;
      }
    }

  }


  public enum PierceStage
  {
    First = -1,
    Second = 0,
    Third = 1,
  }

}
