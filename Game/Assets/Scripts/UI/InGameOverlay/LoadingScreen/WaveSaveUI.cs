
using System;
using MageAFK.Animation;
using MageAFK.Core;
using MageAFK.Management;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MageAFK
{
    public class WaveSaveUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text wave, level, silver;
        [SerializeField] private Image locationImage;
        public Button close, load;

        private event Action<bool> OnSaveDecision;


        public void OnLoadPressed(GameObject obj) => ButtonPressed(true, obj);
        public void OnClosePressed(GameObject obj) => ButtonPressed(false, obj);
        private void ButtonPressed(bool isLoad, GameObject obj)
        {
            UIAnimations.Instance.AnimateButton(obj);
            UIAnimations.Instance.ClosePanel(gameObject, () => {Destroy(gameObject); transform.parent.gameObject.SetActive(false); });
            OnSaveDecision?.Invoke(isLoad);
        }
        public void SubToEvent(Action<bool> handler) => OnSaveDecision += handler;

        public void LoadUI(WaveSaveData data)
        {
            level.text = $"Level {data.levelData.currentLevel}";
            wave.text = $"Wave\n<color=#C3FF9C>{data.waveData.wave}";
            silver.text = data.silver.ToString("N0");
            locationImage.sprite = ServiceLocator.Get<LocationHandler>().ReturnLocationData().image;
        }


    }
}
