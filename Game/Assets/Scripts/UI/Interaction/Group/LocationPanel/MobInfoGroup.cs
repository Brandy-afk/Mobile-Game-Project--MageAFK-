using UnityEngine;

namespace MageAFK.UI
{
    public class MobInfoGroup : TabGroup
  {
    [SerializeField] LocationUI locationUI;


    public override void OnTabSelected(TabButton button)
    {
      MobInfoButton b = button as MobInfoButton;
      if (b.ReturnMob() == null) { return; }
      selectedTab = button;


      // locationUI.MobInfoPopUp(b.ReturnMob());

    }






  }
}