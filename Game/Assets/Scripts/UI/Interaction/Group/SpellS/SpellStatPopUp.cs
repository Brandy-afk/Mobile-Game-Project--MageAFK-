
using MageAFK.Management;
using MageAFK.Spells;
using MageAFK.Stats;
using MageAFK.Tools;
using MageAFK.UI;
using TMPro;
using UnityEngine;

namespace MageAFK
{
    public class SpellStatPopUp : LonePopUp
    {
        [SerializeField] private TMP_Text title, level, max, desc;

        public void InputAndOpen(SpellStat stat)
        {
            title.text = StringManipulation.AddSpacesBeforeCapitals(stat.statType.ToString());
            level.text = stat.level.ToString();
            max.text = stat.maxLevel != -1 ? stat.maxLevel.ToString() : "None";
            desc.text = ServiceLocator.Get<StatInformation>().ReturnStatInformation(stat.statType).desc;
            Open();
        }

    }
}
