
using UnityEngine;
using MageAFK.AI;
using MageAFK.Stats;
using MageAFK.Tools;


namespace MageAFK.Spells
{

  public class VeilProjectile : SpellProjectile
  {

    private float spawnTime;
    private float duration;

    private void OnEnable()
    {
      spawnTime = Time.time;
      duration = spell.ReturnStatValue(Stat.SpellDuration);
    }

    private void Update()
    {
      if (Time.time > (spawnTime + duration))
      {
        Disable();
      }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
      if (Utility.VerifyTags(targetTags, other))
      {
        other.GetComponent<EnemyProjectile>().OnHit();
      }
    }
  }

}
