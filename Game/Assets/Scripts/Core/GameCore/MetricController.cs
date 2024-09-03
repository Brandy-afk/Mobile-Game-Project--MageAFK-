using MageAFK.AI;
using MageAFK.Management;
using MageAFK.Player;
using MageAFK.Spells;

namespace MageAFK.Core
{
  public static class MetricController
  {

    public static void DamageEnemyMetrics(float actualDamage)
    {
      ServiceLocator.Get<PlayerData>().AddStatValue(PlayerStatisticEnum.Damage, actualDamage);
      ServiceLocator.Get<SiegeStatisticTracker>().ModifyDamage(actualDamage);
      ServiceLocator.Get<MilestoneHandler>().UpdateMileStone(MilestoneID.DoDamage, actualDamage);
    }

    public static void RecordSpellMetrics(Spell spell, float actualDamage, bool isDead)
    {
      ServiceLocator.Get<SiegeStatisticTracker>().ModifySpellMetric(spell.iD, actualDamage);
      spell.AppendRecord(SpellRecordID.Damage, actualDamage);

      if (isDead)
      {
        spell.AppendRecord(SpellRecordID.Kills, 1);

        if (spell.type == SpellType.Ultimate)
          ServiceLocator.Get<MilestoneHandler>().UpdateMileStone(MilestoneID.UltimateKills, 1);
      }
    }

    public static void RecordKillMetrics(EntityIdentification iD)
    {
      ServiceLocator.Get<SiegeStatisticTracker>().ModifiyMetric(PlayerStatisticEnum.Kills, 1);
      ServiceLocator.Get<PlayerData>().AddStatValue(PlayerStatisticEnum.Kills, 1);
      ServiceLocator.Get<MilestoneHandler>().UpdateMileStone(MilestoneID.KillEnemies, 1);

      Mob mob = ServiceLocator.Get<EntityHandler>().GetMob(iD);
      if (mob.grade == MobGrade.Boss)
      {
        ServiceLocator.Get<SiegeStatisticTracker>().ModifiyMetric(PlayerStatisticEnum.BossesKilled, 1);
      }
    }


    public static void EnemyDropMetrics(Drops drops)
    {

      PlayerStatisticEnum currencyMetric = drops.currencyType == CurrencyType.SilverCoins ? PlayerStatisticEnum.Silver : PlayerStatisticEnum.Gems;
      ServiceLocator.Get<SiegeStatisticTracker>().ModifiyMetric(currencyMetric, drops.currencyAmount);

      if (drops.item != null)
      {
        ServiceLocator.Get<SiegeStatisticTracker>().ModifiyMetric(PlayerStatisticEnum.ItemsGained, drops.itemAmount);
      }

    }

    public static void OnPlayerDamaged(float amount, EntityIdentification enemyID)
    {

      ServiceLocator.Get<SiegeStatisticTracker>().ModifyEnemyMetric(enemyID, amount);

      ServiceLocator.Get<PlayerData>().AddStatValue(PlayerStatisticEnum.DamageTaken, amount);
    }



  }

}