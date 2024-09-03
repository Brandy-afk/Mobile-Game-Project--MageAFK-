
using MageAFK.Animation;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MageAFK.UI
{
    public class TipUI : SerializedMonoBehaviour
    {
        [SerializeField] private TMP_Text title;
        [SerializeField] private TMP_Text desc;
        private Button blackMask;


        [System.Serializable]
        public class Tip
        {
            [TextArea(2, 2)]
            public string title;

            [TextArea(10, 20)]
            public string desc;
        }

        private void OnEnable() => blackMask.onClick.AddListener(Close);
        private void OnDisable() => blackMask.onClick.RemoveAllListeners();

        public void Open(Tutorial.Tip tip)
        {
            title.text = tip.ReturnTitle();
            desc.text = tip.ReturnDesc();

            if (blackMask == null) blackMask = transform.parent.GetComponent<Button>();

            blackMask.gameObject.SetActive(true);
            OverlayAnimationHandler.SetIsAnimating(true);
            UIAnimations.Instance.OpenPanel(gameObject, () => OverlayAnimationHandler.SetIsAnimating(false));
        }

        private void Close()
        {
            OverlayAnimationHandler.SetIsAnimating(true);
            UIAnimations.Instance.ClosePanel(gameObject, () =>
            {
                OverlayAnimationHandler.SetIsAnimating(false);
                blackMask.gameObject.SetActive(false);
            });
        }

    }
}
