using System.Collections;
using MageAFK.Core;
using MageAFK.Management;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MageAFK.UI
{
    public class LoadingScreenUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text location;
        [SerializeField] private TMP_Text loadingText;
        [SerializeField] private Image locImage;
        [SerializeField] private GameObject wizard;
        [SerializeField] private float timeBetweenDots;
        [HideInInspector] public LoadingState loadingState = LoadingState.Initialization;

        public enum LoadingState
        {
            Initialization,
            LoadingInSiege,
            LoadingOutSiege
        }

        private Coroutine loadingCoroutine;

        private void OnEnable()
        {
            bool state = true;
            switch (loadingState)
            {
                case LoadingState.Initialization:
                    location.text = "Drunkards Defense";
                    break;

                case LoadingState.LoadingInSiege:
                    LoadLocationInformation();
                    state = false;
                    break;

                case LoadingState.LoadingOutSiege:
                    location.text = "Town";
                    break;
            }


            locImage.gameObject.SetActive(!state);
            wizard.SetActive(state);

            loadingCoroutine = StartCoroutine(LoadingTextAnimation());
        }

        private void OnDisable()
        {
            if (loadingCoroutine != null)
            {
                StopCoroutine(loadingCoroutine);
                loadingCoroutine = null;
            }
        }

        private void LoadLocationInformation()
        {
            locImage.sprite = ServiceLocator.Get<LocationHandler>().ReturnLocationData().image;
            location.text = LocationHandler.currentLocation.ToString();
        }

        private IEnumerator LoadingTextAnimation()
        {
            string baseText = "Loading";
            int dotCount = 0;
            int maxDots = 3;

            while (true)
            {
                loadingText.text = baseText + new string('.', dotCount);

                dotCount++;
                if (dotCount > maxDots)
                {
                    dotCount = 0;
                }

                // Wait for some time before the next change
                yield return new WaitForSecondsRealtime(timeBetweenDots);
            }
        }

    }
}