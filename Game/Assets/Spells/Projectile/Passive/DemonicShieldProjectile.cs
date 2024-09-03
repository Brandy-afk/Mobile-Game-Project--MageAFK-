
using UnityEngine;
using MageAFK.AI;
using MageAFK.Stats;
using MageAFK.Player;
using MageAFK.Tools;


namespace MageAFK.Spells
{

  public class DemonicShieldProjectile : SpellProjectile
  {

    private DemonicShield demonicSpell;
    private float spawnTime;
    private bool active = false;
    private float angle = 0f;
    private bool movingRight = true;

    private float duration;


    private void OnEnable()
    {
      if (demonicSpell == null) demonicSpell = spell as DemonicShield;
      angle = 0f;
      duration = spell.ReturnStatValue(Stat.SpellDuration);
      spawnTime = Time.time;
    }

    public void SetActive() => active = true;

    private void Update()
    {
      if (!active) return;
      if (movingRight)
      {
        angle += demonicSpell.angularSpeed * Time.deltaTime;
        if (angle > 180) movingRight = false;
      }
      else
      {
        angle -= demonicSpell.angularSpeed * Time.deltaTime;
        if (angle < 0) movingRight = true;
      }

      // Calculate position
      float x = Mathf.Cos(Mathf.Deg2Rad * angle) * demonicSpell.radius;
      float y = Mathf.Sin(Mathf.Deg2Rad * angle) * demonicSpell.radius;

      // Apply position (assuming y is up/down and x is left/right)
      transform.position = PlayerController.Positions.Pivot + new Vector2(x, -Mathf.Abs(y));

      if (Time.time > spawnTime + duration)
      {
        GetComponent<Animator>().Play("End");
        active = false;
      }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
      if (active && Utility.VerifyTags(targetTags, other))
      {
        var projectile = other.GetComponent<EnemyProjectile>();
        projectile.OnHit(null);
        demonicSpell.Retaliate(projectile.source, transform);
      }
    }
  }

}
