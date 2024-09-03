using MageAFK.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MageAFK.UI
{
    public class MilestoneNode : MonoBehaviour
  {

    public Slider slider;
    public TMP_Text value, rewardText;
    public Image[] skillPoints;


    private MilestoneUI milestoneUI;
    private MilestoneID milestoneID;

    public void InputUI(Milestone milestone, MilestoneUI script)
    {
      milestoneID = milestone.iD;
      milestoneUI = script;

      gameObject.transform.GetChild(0).GetComponent<TMP_Text>().text = milestone.title;
      gameObject.transform.GetChild(3).GetComponent<TMP_Text>().text = milestone.desc;
    }

    public void OnPressed()
    {
      milestoneUI.OpenPopUp(milestoneID);
    }
  }
}