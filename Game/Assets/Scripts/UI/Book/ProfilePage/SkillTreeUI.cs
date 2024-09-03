using System.Collections.Generic;
using MageAFK.Skills;
using MageAFK.Tools;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MageAFK.UI
{
    public class SkillTreeUI : MonoBehaviour, ISkillSprites
  {

    [Header("Objects")]
    [SerializeField] private UpgradePopUp popUp;

    [System.Serializable]
    private class UpgradePopUp
    {
      public TMP_Text name, desc, cost, rank, currentValue, NextValue;
      public GameObject currencyImage;

      public Image item, itemPanel;
    }

    //Node panel images
    [BoxGroup("Sprites")] public Sprite upgradedPanelImage;
    [BoxGroup("Sprites")] public Sprite maxedPanelImage;
    [BoxGroup("Sprites")] public Sprite lockedPanelImage;

    [BoxGroup("Sprites")] public Sprite unlockedTurnImage;
    [BoxGroup("Sprites")] public Sprite unlockedLineImage;

    public Sprite UpgradedPanelImage => upgradedPanelImage;
    public Sprite MaxedPanelImage => maxedPanelImage;
    public Sprite LockedPanelImage => lockedPanelImage;
    public Sprite UnlockedTurnImage => unlockedTurnImage;
    public Sprite UnlockedLineImage => unlockedLineImage;


    [Header("References")]
    [SerializeField] private SkillTabGroup skillTabGroup;

    private Skill currentSkill;


    public void InputSkillPopUp(Skill skill)
    {
      currentSkill = skill;
      popUp.name.text = StringManipulation.AddSpacesBeforeCapitals(currentSkill.skillName.ToString());
      popUp.desc.text = currentSkill.desc;
      UpdatePopUpFields();
    }

    public void UpdateNodeState(Skill skill)
    {
      if (skillTabGroup.skillNodes == null || skillTabGroup.skillNodes.Count <= 0) return;
      try
      {
        SkillNode node = skillTabGroup.skillNodes[skill.skillName];
        node.UpdateNode();
      }
      catch (KeyNotFoundException)
      {
        Debug.Log($"Key not found  : {skill.skillName}");
      }
    }

    public void UpdatePopUpFields()
    {
      popUp.itemPanel.sprite = currentSkill.state == 0 ? lockedPanelImage :
             ((int)currentSkill.state == 1 ? lockedPanelImage :
             ((int)currentSkill.state == 2 ? upgradedPanelImage : maxedPanelImage));

      popUp.item.sprite = currentSkill.state == 0 ? currentSkill.lockedIcon :
      ((int)currentSkill.state == 1 ? currentSkill.unlockedIcon :
      ((int)currentSkill.state == 2 ? currentSkill.unlockedIcon : currentSkill.maxedIcon));

      popUp.currentValue.text = currentSkill.ValueToString(true);

      bool state = currentSkill.state == SkillState.Maxed;

      popUp.NextValue.text = state ? "N/A" : currentSkill.ValueToString(false);  // If the skill is maxed, we don't need the next value.
      popUp.rank.text = state ? "MAX" : $"{currentSkill.currentRank}/{currentSkill.maxRank}";
      if (!state) popUp.cost.text = currentSkill.GetCost().ToString();
      popUp.cost.gameObject.SetActive(!state);
      popUp.currencyImage.SetActive(!state);

    }

  }

  public interface ISkillSprites
  {
    Sprite UpgradedPanelImage { get; }
    Sprite MaxedPanelImage { get; }
    Sprite LockedPanelImage { get; }
    Sprite UnlockedTurnImage { get; }
    Sprite UnlockedLineImage { get; }
  }

}
