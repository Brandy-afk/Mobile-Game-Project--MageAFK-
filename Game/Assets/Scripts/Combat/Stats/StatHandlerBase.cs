using System.Collections.Generic;
using MageAFK.AI;
using MageAFK.Core;
using MageAFK.Management;
using MageAFK.Player;
using MageAFK.Stats;

namespace MageAFK
{
    public abstract class StatHandlerBase
    {

        protected virtual Dictionary<Stat, float> tempStatValues { get; set; } = new();
        protected virtual Dictionary<Stat, float> permStatValues { get; set; } = new();


        public StatHandlerBase()
        {
            WaveHandler.SubToSiegeEvent((Status status) =>
                {
                    if (status == Status.End_CleanUp)
                    {
                        OnSiegeEnd();
                    }
                }, true);
        }

        public virtual void ModifyStat(Stat stat, float amount, bool isPerm)
        {
            Dictionary<Stat, float> d = isPerm ? permStatValues : tempStatValues;

            if (!d.ContainsKey(stat))
                d[stat] = amount;
            else
                d[stat] += amount;

            if (d[stat] == 0) d.Remove(stat);
        }

        public float ReturnModifiedValue(Stat stat, float value)
        {
            // Initialize modifiers
            float tempModifier = 0f;
            float permModifier = 0f;

            // Check if tempStatValues contains the stat and retrieve its value if it does
            if (tempStatValues.ContainsKey(stat))
                tempModifier = tempStatValues[stat];


            // Check if permStatValues contains the stat and retrieve its value if it does
            if (permStatValues.ContainsKey(stat))
                permModifier = permStatValues[stat];

            // If neither dictionary contains the stat, return the original value
            if (tempModifier == 0f && permModifier == 0f)
                return value;

            // Check if the stat is a percentage modifier
            if (ServiceLocator.Get<StatInformation>().ReturnStatInformation(stat).percentageModification)
                // Apply percentage modification using both modifiers
                return value * (1 + tempModifier + permModifier);
            else
                // Apply direct modification using both modifiers
                return value + tempModifier + permModifier;
        }

        public float ReturnModification(Stat stat, float value = -0.1f)
        {
            float tempModifier = 0f;
            float permModifier = 0f;

            bool isPercentageModification = ServiceLocator.Get<StatInformation>().ReturnStatInformation(stat).percentageModification;

            if (tempStatValues.ContainsKey(stat))
                tempModifier = tempStatValues[stat];

            // Assuming you have a dictionary called permStatValues for permanent stat values
            if (permStatValues.ContainsKey(stat))
                permModifier = permStatValues[stat];

            if (value != -0.1f && isPercentageModification)
                // Apply percentage-based modifiers to value and return it
                return value * (tempModifier + permModifier);
            else
                // If value == -0.1f, it means we're not applying it to a specific value and just want the total modifiera
                return tempModifier + permModifier;
        }

        /// <summary>
        /// For Temp stats only
        /// </summary>
        /// <param name="values"></param>
        /// <param name="state"></param>
        public static void AlterEntityStats((Stat stat, float value, bool isForEnemy)[] values, bool state)
        {
            foreach (var trait in values)
            {
                float value = state ? trait.value : trait.value * -1;

                if (trait.isForEnemy)
                    ServiceLocator.Get<EnemyStatHandler>().ModifyStat(trait.stat, value, false);
                else
                    ServiceLocator.Get<PlayerStatHandler>().ModifyStat(trait.stat, value, false);
            }
        }
        /// <summary>
        /// For Temp stats only
        /// </summary>
        /// <param name="stat"></param>
        /// <param name="value"></param>
        /// <param name="isForEnemy"></param>
        /// <param name="state"></param>
        public static void AlterEntityStats(Stat stat, float value, bool isForEnemy, bool state)
        {
            var newArray = new (Stat, float, bool)[] { (stat, value, isForEnemy) };
            AlterEntityStats(newArray, state);
        }

        protected abstract void OnSiegeEnd();
    }
}
