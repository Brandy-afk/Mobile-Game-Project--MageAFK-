using UnityEngine;
using UnityEngine.UI;
using MageAFK.Skills;
using TMPro;
using MageAFK.Management;
using UnityEngine.EventSystems;

namespace MageAFK.UI
{
  public class SkillNode : MonoBehaviour, IPointerClickHandler
  {
    [SerializeField] public PlayerSkills skillID;
    [SerializeField] private Image[] lines;
    [SerializeField] private Image[] turns;
    [SerializeField] private Image skillImage;
    [SerializeField] private Image panelImage;
    [SerializeField] private TMP_Text rank;
    private ISkillSprites skillSprites;
    private IOnTabSelected<SkillNode> interaction;

    public void OnPointerClick(PointerEventData eventData) => interaction.OnTabSelected(this);
    public void SetUpNode(ISkillSprites skillSprites, IOnTabSelected<SkillNode> interaction)
    {
      var skill = ServiceLocator.Get<SkillTreeHandler>().ReturnSkill(skillID);
      this.skillSprites = skillSprites;
      this.interaction = interaction;
      skillImage.sprite = skill.lockedIcon;
      rank.transform.parent.gameObject.SetActive(false);
      panelImage.sprite = skillSprites.LockedPanelImage;
    }

    public void UpdateNode()
    {
      var skill = ServiceLocator.Get<SkillTreeHandler>().ReturnSkill(skillID);
      if (skill.state == SkillState.Locked) return;

      Sprite holder = null;
      if ((int)skill.state >= 1)
      {
        rank.transform.parent.gameObject.SetActive(true);
        holder = skill.unlockedIcon;
        UpdateLines();
      }

      if (skill.state == SkillState.Upgraded)
      {
        panelImage.sprite = skillSprites.UpgradedPanelImage;
      }

      if (skill.state == SkillState.Maxed)
      {
        panelImage.sprite = skillSprites.MaxedPanelImage;
        holder = skill.maxedIcon;
      }

      skillImage.sprite = holder ? holder : skillImage.sprite;

      rank.text = $"{skill.currentRank}/{skill.maxRank}";
    }

    private void UpdateLines()
    {
      for (int i = 0; i < lines.Length; i++)
      {
        lines[i].sprite = skillSprites.UnlockedLineImage;
      }

      for (int i = 0; i < turns.Length; i++)
      {
        turns[i].sprite = skillSprites.UnlockedTurnImage;
      }
    }
  }
}
