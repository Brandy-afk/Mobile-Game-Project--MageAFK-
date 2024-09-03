using MageAFK.Core;
using MageAFK.Management;
using UnityEngine;

namespace MageAFK.Skills
{
  [CreateAssetMenu(fileName = "Potential", menuName = "Skill/Combat/Potential")]
  public class Potential : Skill
  {
    public override void ApplySkillEffect()
    {
      float f = 0;

      if (currentRank > 0 && currentRank <= upgradeValues.Length)
      {
        f = currentRank == 1 ? upgradeValues[0] : upgradeValues[currentRank - 1] - upgradeValues[currentRank - 2];
      }

      ServiceLocator.Get<LevelProgressHandler>().IncreaseDamagePerLevel(f);
    }


  }
}