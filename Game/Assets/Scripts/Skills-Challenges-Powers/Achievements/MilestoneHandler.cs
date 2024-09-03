using System;
using System.Collections.Generic;
using System.Linq;
using MageAFK.Management;
using MageAFK.Tools;
using MageAFK.UI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MageAFK.Core
{
  public class MilestoneHandler : SerializedMonoBehaviour, IData<MilestoneData>
  {
    [SerializeField] public Dictionary<MilestoneID, Milestone> milestones;
    [SerializeField] private MilestoneUI milestoneUI;



    private void Awake()
    {
      ServiceLocator.RegisterService<IData<MilestoneData>>(this);
      ServiceLocator.RegisterService(this);
    }

    private void Start()
    {
      //Testing
      foreach (var pair in milestones)
      {
        pair.Value.ResetMilestone();
      }
    }

    #region Intialization

    public void InitializeData(MilestoneData data)
    {
      foreach (var field in data.fields)
        if (milestones.TryGetValue(field.milestoneID, out Milestone milestone))
          milestone.LoadData(field);
    }

    public MilestoneData SaveData() => new MilestoneData(milestones.Select(pair => pair.Value.SaveData()).ToList());

    #endregion
    public void UpdateMileStone(MilestoneID ID, float quantity)
    {
      if (!milestones.TryGetValue(ID, out Milestone milestone)) return;
      if (milestone.isMaxed) return;

      bool state = milestones[ID].AdvanceValue(quantity);
      milestoneUI.UpdateMilestoneNodeValue(milestones[ID]);
      if (state)
      {
        ServiceLocator.Get<IndicatorHandler>().SetUIChain(UIPanel.Book_Profile_Milestone);
        milestoneUI.UpdateLevelUI(milestone);
      }

      if (WaveHandler.WaveState == WaveState.None)
        SaveManager.Save(SaveData(), DataType.MilestoneData);
    }

    public Milestone GetMilestone(MilestoneID iD)
    {
      if (milestones.ContainsKey(iD))
        return milestones[iD];
      else
        return null;
    }

    public int ReturnMilestoneCount()
    {
      return milestones.Count;
    }

  }


  [Serializable]
  public class MilestoneData
  {
    public List<MilestoneDataFields> fields;
    public MilestoneData(List<MilestoneDataFields> milestones)
    {
      fields = milestones;
    }
  }

  [Serializable]
  public class MilestoneDataFields
  {
    public MilestoneID milestoneID;
    public int rank;
    public bool isMaxed;
    public float currentValue;
    public int rewardPoolSize;

    public MilestoneDataFields(MilestoneID id, int rank, bool isMaxed, float currentValue, int rewardPoolSize)
    {
      milestoneID = id;
      this.rank = rank;
      this.isMaxed = isMaxed;
      this.currentValue = currentValue;
      this.rewardPoolSize = rewardPoolSize;
    }
  }

}