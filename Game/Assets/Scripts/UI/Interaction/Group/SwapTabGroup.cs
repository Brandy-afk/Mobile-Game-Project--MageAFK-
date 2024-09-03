
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace MageAFK.UI
{
    public class SwapTabGroup : TabGroup
    {
        [SerializeField] private GameObject[] swaps;
        [SerializeField] private bool imageSwap;
        private bool ReturnIfImageSwap() => imageSwap;
        [SerializeField, HideIf("ReturnIfImageSwap")] private Color onColor;
        [SerializeField, HideIf("ReturnIfImageSwap")] private Color offColor;

        [SerializeField, ShowIf("ReturnIfImageSwap")] private Sprite onSprite;
        [SerializeField, ShowIf("ReturnIfImageSwap")] private Sprite offSprite;


        public override void OnTabSelected(TabButton button)
        {
            if (button == selectedTab) return;
            ResetTabs();
            selectedTab = button;
            ToggleSelectedButton(true);
        }


        public override void ResetTabs()
        {
            foreach (var button in tabButtons)
            {
                if (button == selectedTab)
                {
                    ToggleSelectedButton(false);
                }
            }
        }

        private void ToggleSelectedButton(bool state)
        {
            if (imageSwap)
                selectedTab.GetComponent<Image>().sprite = state ? onSprite : offSprite;
            else
                selectedTab.GetComponent<Image>().color = state ? onColor : offColor;

            swaps[selectedTab.GetID()].SetActive(state);
        }
    }
}
