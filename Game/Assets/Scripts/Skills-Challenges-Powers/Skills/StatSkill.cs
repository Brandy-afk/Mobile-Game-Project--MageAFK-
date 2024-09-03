
using MageAFK.Management;
using MageAFK.Player;
using MageAFK.Stats;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MageAFK.Skills
{
    [CreateAssetMenu(fileName = "New Skill", menuName = "Skill/StatSkill")]
    public class StatSkill : Skill
    {

        [SerializeField, TabGroup("Skill")] private Stat statToAlter;
        [SerializeField, TabGroup("Skill")] private bool negative;
        public override void ApplySkillEffect()
        {
            float f = 0;

            if (currentRank > 0 && currentRank <= upgradeValues.Length)
            {
                f = currentRank == 1 ? upgradeValues[0] : upgradeValues[currentRank - 1] - upgradeValues[currentRank - 2];
            }
            else
                return;

            var value = negative ? -Mathf.Abs(f) : Mathf.Abs(f);
            ServiceLocator.Get<PlayerStatHandler>().ModifyStat(statToAlter, value, true);
        }


    }
}
