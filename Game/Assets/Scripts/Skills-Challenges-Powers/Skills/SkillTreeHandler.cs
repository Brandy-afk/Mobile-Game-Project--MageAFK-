using System;
using MageAFK.Management;
using System.Collections.Generic;
using MageAFK.Player;
using MageAFK.UI;
using UnityEngine;
using System.Linq;
using MageAFK.Core;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using MageAFK.Tools;

namespace MageAFK.Skills
{
  public class SkillTreeHandler : SerializedMonoBehaviour, IData<SkillData>
  {
    [SerializeField] private Dictionary<PlayerSkills, Skill> skills = new();
    [SerializeField] private SkillTreeUI uI;

    private void Awake()
    {
      ServiceLocator.RegisterService(this);
      ServiceLocator.RegisterService<IData<SkillData>>(this);
    }

    [Button("Reset")]
    private void ResetSkills()
    {
      foreach (var skill in skills)
      {
        skill.Value.Reset();
      }
    }

    public void InitializeData(SkillData data)
    {
      foreach (SkillDataField field in data.skillData)
      {
        if (skills.ContainsKey(field.skillID))
        {
          skills[field.skillID].LoadData(field);
        }
      }
    }

    public SkillData SaveData()
    {
      List<SkillDataField> data = new();
      foreach (var skill in skills)
      {
        data.Add(skill.Value.SaveData());
      }
      return new SkillData(data);
    }


    public void UpgradeSkill(Skill skill)
    {
      skill.currentRank += 1;
      skill.state = SkillState.Upgraded;
      if (skill.currentRank == skill.maxRank)
      {
        skill.state = SkillState.Maxed;
        ServiceLocator.Get<PlayerData>().AddStatValue(PlayerStatisticEnum.SkillsMaxed, 1);
      }

      uI.UpdatePopUpFields();
      uI.UpdateNodeState(skill);

      if (skill.applyOnUpgrade)
        skill.ApplySkillEffect();

      if (skill.currentRank == 1)
        skill.TryAddDynamicAction();

      UnlockPrerequisiteMetSkills();

      if (WaveHandler.WaveState == WaveState.None)
        SaveManager.Save(SaveData(), DataType.SkillData);
    }

    private void UnlockPrerequisiteMetSkills()
    {
      foreach (var skill in skills.Values)
      {
        if (skill.state == SkillState.Maxed) { continue; }
        if (skill.prerequisites == null
                  && skill.state == SkillState.Locked)
        {
          skill.state = SkillState.Unlocked;
          uI.UpdateNodeState(skill);
        }
        else if (skill.prerequisites.All(prereq => (int)prereq.state > 1)
                  && skill.state == SkillState.Locked)
        {
          skill.state = SkillState.Unlocked;
          uI.UpdateNodeState(skill);
        }

      }
    }

    public Skill ReturnSkill(PlayerSkills skill) => skills.ContainsKey(skill) ? skills[skill] : null;
    public int ReturnSkillAmount() => skills.Count;

  }

  [Serializable]
  public class SkillData
  {
    public List<SkillDataField> skillData;

    public SkillData(List<SkillDataField> data)
    {
      skillData = data;
    }

  }

  [Serializable]
  public class SkillDataField
  {

    public PlayerSkills skillID;
    public SkillState state;
    public int currentRank;
    public SkillDataField(PlayerSkills id, SkillState state, int currentRank)
    {
      skillID = id;
      this.state = state;
      this.currentRank = currentRank;
    }

  }

}