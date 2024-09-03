using System.Linq;
using MageAFK.Management;
using MageAFK.Spells;

namespace MageAFK.Core
{
    public static class CustomMilestoneBehaviour
    {
        //TODO return here after challenge revision

        public static void CheckEndOfWaveMilestones(WaveStatistics waveStatistics)
        {

            //Check location wave was complete
            MilestoneID iD = MilestoneID.None;
            var location = ServiceLocator.Get<LocationHandler>().ReturnCurrentLocation();
            switch (location)
            {
                case Location.Woods: iD = MilestoneID.CompleteWavesInWoods; break;
                case Location.Deadlands: iD = MilestoneID.CompleteWavesInDesert; break;
                case Location.Ridge: iD = MilestoneID.CompleteWavesInMountains; break;
            }
            ServiceLocator.Get<MilestoneHandler>().UpdateMileStone(iD, 1);

            //Check if damage had been taken
            if (waveStatistics.nemesisPair.Key == EntityIdentification.None)
                ServiceLocator.Get<MilestoneHandler>().UpdateMileStone(MilestoneID.FinishWavesWithoutTakingDamage, 1);

        }


        public static void CheckIfSpellMaxed(Spell spell)
        {
            int maxed = spell.spellStats.Values.Count(stat => stat.upgradable && stat.level >= stat.maxLevel);
            int count = spell.spellStats.Values.Count(stat => stat.upgradable);
            if (maxed >= count) ServiceLocator.Get<MilestoneHandler>().UpdateMileStone(MilestoneID.MaxOutSpells, 1);
        }

        public static void CheckEndOfSiegeMilestones()
        {

        }

    }
}
