using MageAFK.Core;
using MageAFK.Items;
using MageAFK.Management;
using UnityEngine;


namespace MageAFK.Skills
{
  [CreateAssetMenu(fileName = "New Skill", menuName = "Skill/Utility/GoodMeal")]
  public class GoodMeal : Skill
  {
    [Header("Upgrade Values Determined in Code")]
    private ItemGrade[] goodMealUpgrades = { ItemGrade.Common, ItemGrade.Unique, ItemGrade.Rare, ItemGrade.Artifact, ItemGrade.Corrupt };
    public override void ApplySkillEffect()
    {
      var gradeStatHandler = ServiceLocator.Get<GradeStatHandler>();

      gradeStatHandler.ReturnSalvageInformation(goodMealUpgrades[currentRank - 1]).gemsUnlocked = true;

      if (WaveHandler.WaveState == WaveState.None)
        SaveManager.Save(gradeStatHandler.SaveData(), DataType.GradeData);
    }

    public override string ValueToString(bool isCurrent)
    {
      return isCurrent ? currentRank <= 0 ? "None" : goodMealUpgrades[currentRank - 1].ToString() : goodMealUpgrades[currentRank].ToString();
    }


  }
}
