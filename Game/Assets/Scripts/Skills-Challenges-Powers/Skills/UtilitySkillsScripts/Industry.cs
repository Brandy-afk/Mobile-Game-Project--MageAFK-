using MageAFK.Items;
using MageAFK.Management;
using UnityEngine;


namespace MageAFK.Skills
{
  [CreateAssetMenu(fileName = "New Skill", menuName = "Skill/Utility/Industry")]
  public class Industry : Skill
  {
    public override void ApplySkillEffect()
    {

      float f = 0;

      if (currentRank > 0 && currentRank <= upgradeValues.Length)
      {
        f = currentRank == 1 ? upgradeValues[0] : upgradeValues[currentRank - 1] - upgradeValues[currentRank - 2];
      }

      ServiceLocator.Get<CraftingHandler>().AlterWorkOrderSize((int)f);
    }


  }
}
