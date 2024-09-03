using MageAFK.Core;
using MageAFK.Management;
using MageAFK.Player;
using MageAFK.Skills;
using TMPro;
using UnityEngine;


namespace MageAFK.UI
{
  public class ProfileUI : MonoBehaviour
  {


    [Header("Objects")]
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text damageMod;


    [SerializeField] private TMP_Text milestoneText;
    [SerializeField] private TMP_Text skillText;
    [SerializeField] private TMP_Text sPmodText;

    [Header("References")]
    [SerializeField] private MilestoneUI milestoneUI;
    [SerializeField] private SkillTabGroup skillTabGroup;




    private void OnEnable()
    {
      ServiceLocator.Get<LevelHandler>().SubscribeToLevelChanged(UpdatePlayerLevelUI, true);
      ServiceLocator.Get<PlayerData>().SubscribeToStatAltered(PlayerStatisticEnum.SkillsMaxed, UpdateSkillsText, true);
      ServiceLocator.Get<PlayerData>().SubscribeToStatAltered(PlayerStatisticEnum.MilestonesComplete, UpdateMilestoneText, true);
      ServiceLocator.Get<LevelProgressHandler>().OnValueAltered += UpdateInformationFields;
      UpdateInformationFields();

      levelText.gameObject.SetActive(WaveHandler.WaveState != WaveState.None);
    }

    private void OnDisable()
    {
      ServiceLocator.Get<LevelHandler>().SubscribeToLevelChanged(UpdatePlayerLevelUI, false);
      ServiceLocator.Get<PlayerData>().SubscribeToStatAltered(PlayerStatisticEnum.SkillsMaxed, UpdateSkillsText, false);
      ServiceLocator.Get<PlayerData>().SubscribeToStatAltered(PlayerStatisticEnum.MilestonesComplete, UpdateMilestoneText, false);
      ServiceLocator.Get<LevelProgressHandler>().OnValueAltered -= UpdateInformationFields;
    }


    public void UpdatePlayerLevelUI(int level)
    {
      if (levelText.gameObject.activeSelf)
        levelText.text = $"Level {level}";
    }

    public void UpdateInformationFields()
    {
      var levelProgressHandler = ServiceLocator.Get<LevelProgressHandler>();

      damageMod.text = WaveHandler.WaveState == WaveState.None ?
                                            $"{levelProgressHandler.damagePerlevel * 100}% per level" :
                                            $"+{levelProgressHandler.damagePerlevel * ServiceLocator.Get<LevelHandler>().ReturnCurrentLevel() * 100}%";

      sPmodText.text = levelProgressHandler.skillPointMod.ToString();
    }

    public void UpdateSkillsText()
    {
      skillText.text =
      $"{ServiceLocator.Get<PlayerData>().GetStatValue(PlayerStatisticEnum.SkillsMaxed)}/{ServiceLocator.Get<SkillTreeHandler>().ReturnSkillAmount()} Unlocked";
    }

    public void UpdateMilestoneText()
    {
      milestoneText.text =
      $"{ServiceLocator.Get<PlayerData>().GetStatValue(PlayerStatisticEnum.MilestonesComplete)}/{ServiceLocator.Get<MilestoneHandler>().ReturnMilestoneCount()} Completed";
    }








  }
}
