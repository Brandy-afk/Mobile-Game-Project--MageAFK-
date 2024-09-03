using System.Collections.Generic;
using System.Linq;
using MageAFK.Animation;
using MageAFK.Core;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MageAFK.UI
{
    public class MilestoneUI : SerializedMonoBehaviour
  {

    #region PopUp

    [SerializeField, TabGroup("PopUp")] private TMP_Text title;
    [SerializeField, TabGroup("PopUp")] private TMP_Text desc;
    [SerializeField, TabGroup("PopUp")] private Slider slider;
    [SerializeField, TabGroup("PopUp")] private TMP_Text value;

    [SerializeField, TabGroup("PopUp")] private Image[] skillPoints;
    [SerializeField, TabGroup("PopUp")] private GameObject popUp;
    [SerializeField, TabGroup("PopUp")] private Button rewardButton;

    [SerializeField, TabGroup("PopUp")] private Dictionary<RewardType, RewardPanels> rewardPanels;

    [SerializeField, TabGroup("PopUp")] private CanvasGroup animationGroup;

    [SerializeField, TabGroup("PopUp")] private float fadeSpeed = 1f;
    [SerializeField, TabGroup("PopUp")] private float waitTime = .25f;



    [System.Serializable]
    private class RewardPanels
    {
      public GameObject panel;
      public TMP_Text value;
      public Image image;
    }

    #endregion

    #region  Variables
    [SerializeField, TabGroup("Variables")] private GameObject mileStonePrefab;
    [SerializeField, TabGroup("Variables")] private Transform parent;
    [SerializeField, TabGroup("Variables")] public Color baseFill, rewardFill, finishedFill;

    [SerializeField, TabGroup("Variables")] private Button blackButton;
    #endregion

    [SerializeField] private MilestoneHandler milestoneHandler;
    private Dictionary<MilestoneID, MilestoneNode> milestonesUIDict;
    private HashSet<MilestoneID> toUpdate = new();
    private Milestone current = null;

    #region Cycle / Helpers

    private void Awake()
    {
      milestonesUIDict = new Dictionary<MilestoneID, MilestoneNode>();

      var milestones = milestoneHandler.milestones.Values.ToList();
      CreateMilestones(milestones);
      OrganizeMilestoneUI(milestones);
    }

    private void CreateMilestones(List<Milestone> milestones)
    {
      foreach (Milestone milestone in milestones)
      {
        var node = Instantiate(mileStonePrefab, parent).GetComponent<MilestoneNode>();
        milestonesUIDict.Add(milestone.iD, node);
        node.InputUI(milestone, this);
      }
    }

    private void OrganizeMilestoneUI(List<Milestone> milestones)
    {
      var unmaxed = milestones.Where(m => !m.isMaxed).OrderBy(m => m.title).Select(m => m.iD).ToList();
      var maxed = milestones.Where(m => m.isMaxed).OrderBy(m => m.title).Select(m => m.iD).ToList();

      unmaxed.AddRange(maxed);
      var organizedList = unmaxed;

      foreach (var milestone in milestones)
      {
        UpdateLevelUI(milestone);
        UpdateMilestoneNodeValue(milestone);
      }

      for (int i = 0; i < organizedList.Count; i++)
        milestonesUIDict[organizedList[i]].transform.SetSiblingIndex(i);

      organizedList = null;
      unmaxed = null;
      maxed = null;
    }

    private void OnEnable()
    {
      if (toUpdate != null && toUpdate.Count > 0)
      {
        foreach (var iD in toUpdate)
        {
          var node = milestonesUIDict[iD];
          var milestone = milestoneHandler.GetMilestone(iD);
          UpdateNodeValueFields(milestone, node.value, node.slider, true);
          UpdateNodeRewardFields(milestone, node.slider, node.rewardText);
        }

        toUpdate.Clear();
      }

      blackButton.onClick.AddListener(() => ClosePopUp());
    }

    private void OnDisable()
    {
      blackButton.onClick.RemoveListener(() => ClosePopUp());
      if (popUp.activeSelf) ClosePopUp();
    }
    #endregion

    #region Updating
    public void UpdateLevelUI(Milestone milestone)
    {
      if (ScriptStateCheck(milestone.iD)) return;

      MilestoneNode node = milestonesUIDict[milestone.iD];
      UpdateNodeLevelFields(milestone, node.skillPoints);
      UpdateNodeRewardFields(milestone, node.slider, node.rewardText);

      if (current && current.iD == milestone.iD)
      {
        UpdateNodeLevelFields(milestone, skillPoints);
        UpdateNodeRewardFields(milestone, slider, null);
      }
    }

    public void UpdateMilestoneNodeValue(Milestone milestone)
    {
      if (ScriptStateCheck(milestone.iD)) return;

      MilestoneNode node = milestonesUIDict[milestone.iD];
      UpdateNodeValueFields(milestone, node.value, node.slider);

      if (current && current.iD == milestone.iD)
        UpdateNodeValueFields(milestone, value, slider);
    }

    private bool ScriptStateCheck(MilestoneID iD)
    {
      if (!gameObject.activeInHierarchy)
      {
        if (toUpdate == null) toUpdate = new();
        else toUpdate.Add(iD);
        return true;
      }

      return false;
    }

    public void UpdateElementsOrder(MilestoneID iD) => milestonesUIDict[iD].transform.SetAsLastSibling();

    #endregion

    #region PopUp

    public void OpenPopUp(MilestoneID iD)
    {
      Milestone milestone = milestoneHandler.GetMilestone(iD);
      current = milestone;

      PopUpInput(milestone);

      blackButton.gameObject.SetActive(true);
      UIAnimations.Instance.OpenPanel(popUp);
    }

    public void ClosePopUp()
    {
      if (current == null) return;
      current = null;
      if (popUp.activeInHierarchy)
        UIAnimations.Instance.ClosePanel(popUp, () => { blackButton.gameObject.SetActive(false); });
      else
      {
        popUp.SetActive(false);
        blackButton.gameObject.SetActive(false);
      }
    }

    private void PopUpInput(Milestone milestone)
    {
      title.text = milestone.title;
      desc.text = milestone.desc;
      rewardButton.interactable = true;

      UpdateRewards();
      UpdateNodeRewardFields(milestone, slider, null);
      rewardButton.gameObject.SetActive(current.CheckRewardPoolSize());
      animationGroup.alpha = 1f;

      UpdateNodeLevelFields(milestone, skillPoints);
      UpdateNodeValueFields(milestone, value, slider, true);
    }

    public void RewardRequested()
    {
      rewardButton.interactable = false;

      current.ClaimNextReward();
      UpdateLevelUI(current);
      if (current.isMaxed && !current.CheckRewardPoolSize()) UpdateElementsOrder(current.iD);

      UIAnimations.Instance.FadeOut(animationGroup, fadeSpeed, FadeRewardsBackIn);
    }

    private void FadeRewardsBackIn()
    {
      if (current == null) return;
      UpdateRewards();
      rewardButton.gameObject.SetActive(current.CheckRewardPoolSize());
      UIAnimations.Instance.FadeIn(animationGroup, fadeSpeed, waitTime, () => { rewardButton.interactable = true; });
    }

    private void UpdateRewards()
    {
      foreach (KeyValuePair<RewardType, RewardPanels> panel in rewardPanels)
      {
        panel.Value.panel.SetActive(false);
      }

      if (current == null || (current.isMaxed && !current.CheckRewardPoolSize()))
        return;

      Reward[] toDisplay = current.CheckRewardPoolSize() ? current.rewardPool[0] : current.rewardValues[current.rank].rewards;

      foreach (Reward reward in toDisplay)
      {
        if (!rewardPanels.TryGetValue(reward.rewardType, out RewardPanels panel))
        {
          Debug.LogError($"No Reward UI component found for reward type: {reward.rewardType}");
          continue;
        }

        switch (reward.rewardType)
        {
          case RewardType.Recipe:
            var recipeReward = reward as RecipeReward;
            // panel.image.sprite = ServiceLocator.Get<ItemDataBase>().GetGradeInformation(recipeReward.ReturnRecipeGrade()).scroll;
            //TODO come back to later when its being better designed
            break;
          default:
            panel.value.text = reward.ReturnAmount().ToString("N0");
            break;
        }

        panel.panel.SetActive(true);
      }


    }
    #endregion

    #region GenericUpdating

    private void UpdateNodeRewardFields(Milestone milestone, Slider slider, TMP_Text rewardText = null)
    {
      bool state = milestone.CheckRewardPoolSize();

      slider.fillRect.GetComponent<Image>().color = state ? rewardFill : milestone.isMaxed ? finishedFill : baseFill;

      if (rewardText) rewardText.text = state ? "Rewards\nAvailable" : "Press for details";
    }

    private void UpdateNodeLevelFields(Milestone milestone, Image[] sp)
    {
      for (int i = 0; i < milestone.rankCap; i++)
      {
        Color color = sp[i].color;
        color.a = milestone.rank - milestone.rewardPool.Count > i ? 1f : .3f;
        sp[i].color = color;
      }
    }

    private void UpdateNodeValueFields(Milestone milestone, TMP_Text value, Slider slider, bool state = false)
    {
      value.text = milestone.isMaxed ? "Finished" : $"{milestone.currentValue:N0} / {milestone.goalValues[milestone.rank]:N0}";

      slider.maxValue = milestone.isMaxed ? 1 : milestone.goalValues[milestone.rank];

      if (!milestone.isMaxed)
      {
        if (state) slider.value = milestone.currentValue;
        else UIAnimations.Instance.AnimateSlider(slider, milestone.currentValue, false);
      }
      else
        slider.value = slider.maxValue;

    }

    #endregion

  }

}
