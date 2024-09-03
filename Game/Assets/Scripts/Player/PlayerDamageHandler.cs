using System;
using MageAFK.AI;
using MageAFK.Combat;
using MageAFK.Core;
using MageAFK.Management;
using MageAFK.Stats;

namespace MageAFK.Player
{
    public static class PlayerDamageHandler
  {
    public static float HandlePlayerDamaged(float flatDamage, NPEntity entity, bool blockDamage)
    {

      float modifier = 0;
      var playerStatHandler = ServiceLocator.Get<PlayerStatHandler>();
      var mileStoneHandler = ServiceLocator.Get<MilestoneHandler>();

      //Special collisions dependent on spells, items, skills, power.
      foreach (IPlayerCollisionEvent collisionEvent in
      DynamicActionExecutor.Instance.playerCollisions)
        collisionEvent.HandleCollision(entity, ref flatDamage, ref modifier);

      if (blockDamage)
      {
        //Damage resistence.
        modifier += playerStatHandler.ReturnModification(Stat.DamageReduction);
        var reducedDamage = flatDamage * modifier;
        mileStoneHandler.UpdateMileStone(MilestoneID.BlockDamage, reducedDamage);
        flatDamage -= reducedDamage;

        //Block Damage
        var amountToBlock = Math.Min(flatDamage, playerStatHandler.ReturnModification(Stat.Armour));
        mileStoneHandler.UpdateMileStone(MilestoneID.BlockDamage, amountToBlock);
        flatDamage -= amountToBlock;
      }


      return flatDamage;
    }
  }


}