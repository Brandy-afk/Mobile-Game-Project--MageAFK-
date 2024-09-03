using System.Collections.Generic;
using System.Linq;
using MageAFK.Core;
using MageAFK.Management;
using Sirenix.OdinInspector;
using UnityEngine;


namespace MageAFK.Items
{

  public class GradeStatHandler : SerializedMonoBehaviour, IData<GradeStatData>
  {

    //Amount base stat will be upgraded for each level
    [SerializeField] private float[] statUpgradeModifiers = { 0f, 0.2f, 0.5f, 0.9f };

    [Header("Ensure Values are in whole form.")]
    [SerializeField] private float feedChanceMultiplier;
    public float FeedChanceMultiplier => feedAmountMultiplier / 100;
    [SerializeField] private float feedAmountMultiplier;
    public float FeedAmountMultiplier => feedAmountMultiplier / 100;

    [SerializeField] private Dictionary<ItemGrade, SalvageGradeInformation> gradeSalvageInformation = new();
    [SerializeField] private Dictionary<ItemGrade, UpgradeGradeInfo> upgradeInfoDict = new();


    #region Data/Initialization

    private void Awake()
    {
      ServiceLocator.RegisterService(this);
      ServiceLocator.RegisterService<IData<GradeStatData>>(this);
    }

    public void InitializeData(GradeStatData data)
    {
      foreach (var element in data.gemsUnlocked)
      {
        if (gradeSalvageInformation.TryGetValue(element.Item1, out SalvageGradeInformation gradeInformation))
          gradeInformation.gemsUnlocked = element.Item2;
      }
    }

    public GradeStatData SaveData() => new GradeStatData(gradeSalvageInformation
                                                   .Select(pair => (pair.Key, pair.Value.gemsUnlocked))
                                                   .ToArray());

    #endregion

    public SalvageGradeInformation ReturnSalvageInformation(ItemGrade grade) => gradeSalvageInformation[grade];
    public UpgradeGradeInfo ReturnUpgradeInformation(ItemGrade grade) => upgradeInfoDict[grade];
    public float ReturnModForUpgrade(int level) => statUpgradeModifiers[level];
  }







  [System.Serializable]
  public class SalvageGradeInformation
  {
    public bool gemsUnlocked;

    public RewardInfo[] potentialRewards;

  }

  [System.Serializable]
  public class UpgradeGradeInfo
  {
    [Header("ONLY 3 ELEMENTS (amount of levels)")]
    public List<LevelCosts> upgradeCosts;
  }

  [System.Serializable]
  public class LevelCosts
  {
    public int stage1Cost;
    public int stage2Cost;

  }



  [System.Serializable]
  public class RewardInfo
  {
    public RewardType type;
    [Header("Ex: 30 for 30%, Ex: .4 for 4%")]
    public float chance;
    public int min;
    public int max;
    public float ReturnChance(bool isFeed) =>
    isFeed ? chance + (chance * ServiceLocator.Get<GradeStatHandler>().FeedChanceMultiplier) : chance;

  }

  [System.Serializable]
  public class GradeStatData
  {
    public (ItemGrade, bool)[] gemsUnlocked;

    public GradeStatData((ItemGrade, bool)[] gemsUnlocked)
    {
      this.gemsUnlocked = gemsUnlocked;
    }
  }

}