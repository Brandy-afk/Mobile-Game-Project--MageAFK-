using System.Collections.Generic;
using MageAFK.Management;
using MageAFK.UI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MageAFK.Tutorial
{
    public class TipHandler : SerializedMonoBehaviour, ITip
    {


        [SerializeField] private Dictionary<UIPanel, Tip> tips;
        [SerializeField] private Tip[] tip_adder;
        [SerializeField] private TipUI tipUI;

        private void OnValidate()
        {
            if (tip_adder != null && tip_adder.Length > 0)
            {
                if (tips == null) tips = new Dictionary<UIPanel, Tip>();
                foreach (var tip in tip_adder)
                {
                    if (tips.ContainsKey(tip.Target))
                    {
                        Debug.Log($"Target {tip.Target}, already added.");
                        continue;
                    }
                    else
                    {
                        tips.Add(tip.Target, tip);
                    }
                }
            }

            tip_adder = null;
        }

        private void Awake() => ServiceLocator.RegisterService<ITip>(this);

        public void ShowTip(UIPanel uIPanel = UIPanel.None)
        {
            var tip = GetTip(uIPanel);
            if (tip != null)
                tipUI.Open(tip);
        }


        public Tip GetTip(UIPanel uIPanel = UIPanel.None)
        {
            var panel = uIPanel != UIPanel.None ? uIPanel : UIPanelHandler.ReturnCurrentPanel();

            try
            {
                return tips[panel];
            }
            catch (KeyNotFoundException)
            {
                Debug.Log($"Tip not found for {panel}");
                return null;
            }
        }
    }

    public interface ITip
    {
        /// <summary>
        /// Get tip for panel(default == current), and open UI.
        /// </summary>
        /// <param name="uIPanel">default value will open current tip.</param>
        public void ShowTip(UIPanel uIPanel = UIPanel.None);

    }

}
