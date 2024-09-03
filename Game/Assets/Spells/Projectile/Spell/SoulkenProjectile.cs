using MageAFK.AI;
using MageAFK.Combat;
using System.Collections.Generic;


namespace MageAFK.Spells
{

  public class SoulkenProjectile : DefaultController
  {

    private static Dictionary<NPEntity, int> countTracker = new();

    protected override CollisionInformation HandleDamage(NPEntity entity)
    {
      base.HandleDamage(entity);

      if (!countTracker.ContainsKey(entity))
      {
        countTracker[entity] = 1;
        (entity as Enemy).SubscribeToEnemyDeath(OnEnemiesDeath, true);
      }
      else if (countTracker.ContainsKey(entity))
      {
        countTracker[entity]++;
        if (countTracker[entity] > 2)
        {
          (spell as Soulken).SpawnExplosion(entity);
          countTracker[entity] = 0;
        }
      }

      return default;
    }

    private static void OnEnemiesDeath(Enemy enemy) => countTracker.Remove(enemy);

    private void OnDestroy()
    {
      if (countTracker.Count > 0)
        countTracker.Clear();
    }
  }

}
