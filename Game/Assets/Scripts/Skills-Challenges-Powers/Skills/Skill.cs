using MageAFK.Combat;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MageAFK.Skills
{
  public class Skill : ScriptableObject
  {

    [TabGroup("Skill")]
    public PlayerSkills skillName;

    [TextArea(15, 20)]
    [TabGroup("Skill")]
    public string desc;
    [TabGroup("Skill")]
    public bool isPercentage;
    [TabGroup("Skill")]
    public bool applyOnUpgrade = true;
    [TabGroup("Skill"), PreviewField]
    public Sprite lockedIcon;
    [TabGroup("Skill"), PreviewField]
    public Sprite unlockedIcon;
    [TabGroup("Skill"), PreviewField]
    public Sprite maxedIcon;
    [TabGroup("Skill")]
    public Skill[] prerequisites;


    [TabGroup("Upgrades"), Header("Decimal for percentage, whole for rounded modification")]
    public float[] upgradeValues;

    [TabGroup("Upgrades")]
    public int startingCost = 1;
    [TabGroup("Upgrades")]
    public int increasedCostPerRank = 1;
    [TabGroup("Upgrades")]
    public int maxRank = 3;

    [TabGroup("Data")]
    public SkillState state = SkillState.Locked;
    [TabGroup("Data")]
    public int currentRank = 0;

    public virtual void ApplySkillEffect() { }
    public virtual int GetCost() => (increasedCostPerRank * currentRank) + startingCost;
    public virtual string ValueToString(bool isCurrent)
    {
      int index = isCurrent ? currentRank - 1 : currentRank;

      // If current rank is 0, return "0" or "0%" based on the isPercentage flag.
      if (currentRank == 0 && isCurrent)
      {
        return isPercentage ? "0%" : "0";
      }

      // This check is to prevent index out of range exceptions.
      if (index < 0 || index >= upgradeValues.Length)
      {
        Debug.LogError("Invalid Index");
        return "Index Error";  // You might want a default value or error message here.
      }

      float value = upgradeValues[index];

      // If the value represents a percentage, modify it and append the '%' symbol.
      if (isPercentage)
      {
        value *= 100;
        return $"{value:N1}%";
      }

      return $"{value:N1}";
    }



    private void OnValidate()
    {
      if (upgradeValues == null || upgradeValues.Length != maxRank)
      {
        System.Array.Resize(ref upgradeValues, maxRank);
      }
    }
    //TODO for testing
    private void OnEnable() => Reset();
    public void Reset()
    {
      if (prerequisites == null || prerequisites.Length <= 0)
      {
        currentRank = 0;
        state = SkillState.Unlocked;
      }
      else
      {
        currentRank = 0;
        state = SkillState.Locked;
      }
    }
    public virtual void LoadData(SkillDataField fields)
    {
      currentRank = fields.currentRank;
      state = fields.state;
      if (currentRank > 0) TryAddDynamicAction();
    }

    public void TryAddDynamicAction()
    {
      if (this is IDynamicAction action) DynamicActionExecutor.Instance.AddDynamicAction(action);
    }



    public SkillDataField SaveData() => new SkillDataField(skillName, state, currentRank);

  }



  public enum PlayerSkills
  {
    None = 0,
    CraftersCoin = 2,
    DoubleUp = 3,
    FastLearner = 4,
    FreeReturns = 5,
    GoodMeal = 6,
    Industry = 7,
    InsiderDeal = 8,
    Professional = 10,
    MagicShield = 11,
    Introductions = 12,
    Lightweight = 13,
    TimeOut = 14,
    Prepared = 15,
    MagicThorns = 16,
    ChaoticAura = 17,
    Surging = 18,
    Potential = 19,
    Insurmountable = 20,
    SilverEyes = 21,
    Wisdom = 22,
    Cultist = 23,
    Supreme = 24,
    Artisan = 25
  }

  public enum SkillState
  {
    Locked = 0,
    Unlocked = 1,
    Upgraded = 2,
    Maxed = 3
  }


}