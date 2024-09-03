
using MageAFK.Animation;

using MageAFK.Spells;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MageAFK.UI
{
    public abstract class SpellSlot : DragButton<Spell, SpellIdentification>
    {

        [SerializeField, Tooltip("Spell Image")] protected Image image;
        [SerializeField] protected TMP_Text level;

        [SerializeField] protected SpellPopUpUI infoPopUp;

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            if (ReturnIfInteractable())
                UIAnimations.Instance.AnimateButton(image.gameObject);
        }

    }
}
