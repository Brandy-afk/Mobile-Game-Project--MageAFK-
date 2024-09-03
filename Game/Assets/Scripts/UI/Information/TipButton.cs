
using MageAFK.Animation;
using MageAFK.Management;
using MageAFK.Tutorial;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MageAFK.UI
{
    public class TipButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField, InfoBox("Leave as 'None' if current panel is intended target.")] private UIPanel tipToShow = UIPanel.None;

        public void OnPointerClick(PointerEventData eventData)
        {
            UIAnimations.Instance.AnimateButton(gameObject);
            ServiceLocator.Get<ITip>().ShowTip(tipToShow);
        }
    }
}
