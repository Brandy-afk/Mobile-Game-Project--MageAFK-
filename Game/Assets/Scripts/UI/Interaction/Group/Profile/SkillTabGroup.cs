using System.Collections.Generic;
using MageAFK.Animation;
using MageAFK.Core;
using MageAFK.Management;
using MageAFK.Skills;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace MageAFK.UI
{
  public class SkillTabGroup : SerializedMonoBehaviour, IOnTabSelected<SkillNode>
  {
    [SerializeField] public Dictionary<PlayerSkills, SkillNode> skillNodes;
    [Header("WARNING - Manually add all buttons")]
    [SerializeField] private Transform highlighter;
    [SerializeField] private GameObject popUp;
    [SerializeField] private ScrollRect[] scrolls;
    [SerializeField] private ButtonUpdateClass upgradeButton;
    [SerializeField] private Button blackButton;

    [Header("References")]
    [SerializeField] private SkillTreeUI skillTreeUI;
    [SerializeField] private PopUpPanelGroup panelGroup;
    [SerializeField] private SkillTreeHandler skillTreeHandler;

    private Skill currentSkill;
    private SkillNode selectedNode;

    // [Button("SetUp")]
    // public void SetDict()
    // {
    //   if (skillNodes == null) skillNodes = new Dictionary<PlayerSkills, SkillNode>();
    //   SkillNode[] nodes = FindObjectsOfType<SkillNode>(true);
    //   foreach (var skillNode in nodes)
    //   {
    //     skillNodes[skillNode.skillID] = skillNode;
    //   }
    // }

    private void Awake()
    {
      foreach (var pair in skillNodes)
      {
        pair.Value.SetUpNode(skillTreeUI, this);
        pair.Value.UpdateNode();
      }
    }

    private void OnEnable()
    {
      blackButton.onClick.AddListener(() => ClosePopUp());
      panelGroup.OnTabPanelChanged += OnTabPanelChanged;
      ServiceLocator.Get<CurrencyHandler>().SubscribeToCurrencyEvent(OnSkillPointsChanged, CurrencyType.SkillPoints, true);
    }

    private void OnDisable()
    {
      blackButton.onClick.RemoveListener(() => ClosePopUp());
      if (popUp.activeSelf) ClosePopUp();
      panelGroup.OnTabPanelChanged -= OnTabPanelChanged;
      ServiceLocator.Get<CurrencyHandler>().SubscribeToCurrencyEvent(OnSkillPointsChanged, CurrencyType.SkillPoints, false);
    }


    private void OnTabPanelChanged(UIPanel panel)
    {
      int index = panel == UIPanel.Book_Profile_SkillTree_Combat ? 0 : 1; //combat should be index 0
      ResetScrolls(index);
    }

    private void OnSkillPointsChanged(int amount)
    {
      if (popUp.activeInHierarchy && currentSkill != null)
      {
        UpdateUpgradeButton();
      }
    }

    public void OnTabSelected(SkillNode node)
    {
      var skill = skillTreeHandler.ReturnSkill(node.skillID);
      if (skill.state == 0) { return; }
      selectedNode = node;
      currentSkill = skill;

      HighlightButton();
      UpdateUpgradeButton();
      OpenPopUp();

    }

    private void OpenPopUp()
    {
      skillTreeUI.InputSkillPopUp(currentSkill);
      blackButton.gameObject.SetActive(true);
      UIAnimations.Instance.OpenPanel(popUp);
    }

    public void ClosePopUp()
    {
      UIAnimations.Instance.ClosePanel(popUp, () => { blackButton.gameObject.SetActive(false); });
      currentSkill = null;
      highlighter.gameObject.SetActive(false);
    }

    private void HighlightButton()
    {
      highlighter.SetParent(selectedNode.transform, false);
      highlighter.SetAsFirstSibling();
      highlighter.localPosition = Vector3.zero;
      highlighter.gameObject.SetActive(true);
    }

    private void ResetScrolls(int index)
    {
      scrolls[index].horizontalNormalizedPosition = .5f;
      scrolls[index].verticalNormalizedPosition = 1f;
    }


    public void UpgradeSkillPressed()
    {
      if (currentSkill == null) return;
      if (ServiceLocator.Get<CurrencyHandler>().SubtractCurrency(CurrencyType.SkillPoints, currentSkill.GetCost()))
      {
        ServiceLocator.Get<SkillTreeHandler>().UpgradeSkill(currentSkill);
        UpdateUpgradeButton();
      }
      else
        Debug.Log("Not Enough SP buddy");

    }

    private void UpdateUpgradeButton()
    {
      if (currentSkill == null) { return; }
      bool state = currentSkill.GetCost() > ServiceLocator.Get<CurrencyHandler>().GetCurrencyAmount(CurrencyType.SkillPoints) || currentSkill.state == SkillState.Maxed;
      upgradeButton.black.SetActive(state);
      upgradeButton.button.interactable = !state;
    }

  }


}