
using MageAFK.Animation;
using MageAFK.Core;
using MageAFK.Management;
using MageAFK.Stats;
using MageAFK.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MageAFK
{
    public class PowerSlotUI : MonoBehaviour, IPointerClickHandler
    {

        [SerializeField] private TMP_Text title;
        [SerializeField] private Image image;
        [SerializeField] private TMP_Text val;
        [SerializeField] private TMP_Text cost;
        [SerializeField] private GameObject blackMask;
        [SerializeField] private GameObject upgradedObject;

        [SerializeField] private int index;
        private IOnTabSelected<int> group;

        public void SetGroup(IOnTabSelected<int> group) => this.group = group;

        public void FillUI(Elixir elixir, ShopElixir shopElixir)
        {
            bool state = shopElixir != null;
            gameObject.SetActive(state);
            if (!state) return;

            ToggleUpgradedItem(shopElixir.isPurchased);
            title.text = elixir.title;
            title.color = elixir.titleColor;
            image.sprite = elixir.sprite;
            val.text = elixir.FormatValue(shopElixir.value);
            cost.text = $"<sprite name=Gold>{shopElixir.cost}";
        }

        public void ToggleBlackMask(bool state) => blackMask.SetActive(state);

        public void ToggleUpgradedItem(bool state)
        {
            upgradedObject.SetActive(state);
            cost.gameObject.SetActive(!state);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            UIAnimations.Instance.AnimateButton(gameObject);
            group.OnTabSelected(index);
        }
    }
}
