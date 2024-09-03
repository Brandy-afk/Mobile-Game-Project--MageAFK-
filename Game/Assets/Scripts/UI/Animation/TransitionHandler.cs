using System;
using System.Collections;
using MageAFK.Animation;
using MageAFK.Cam;
using MageAFK.Core;
using MageAFK.Management;
using UnityEngine;
using UnityEngine.UI;

namespace MageAFK.UI
{
    public class TransitionHandler : MonoBehaviour
    {


        [SerializeField] private Image loadingBackground, topBlackBackground;
        [SerializeField] private CanvasGroup loadingPanel, constantCanvasGroup, endScreenCanvasGroup;
        [SerializeField] private GameObject siegeObjects, nonSiegeObjects, waveDataPrefab, panelBlackBackground;
        [SerializeField] private float loadingTimer, targetTintedAlpha, genericFadeSpeed, endScreenBFS, loadingFadeSpeed, constFadeSpeed, endUIFadeSpeed;

        [Header("References")]
        [SerializeField] private SceneObjectLoader objectLoader;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private InGamePopUpHandler inGamePopUpHandler;

        #region Game Loading 

        public void OnGameStart(Action callback)
        {
            OverlayAnimationHandler.SetIsAnimating(true);
            loadingPanel.GetComponent<LoadingScreenUI>().loadingState = LoadingScreenUI.LoadingState.Initialization;
            loadingPanel.gameObject.SetActive(true);
            UIAnimations.Instance.FadeIn(loadingPanel, loadingFadeSpeed, 0, callback);
        }

        public void OnDataLoaded(Action<bool> callback)
        {
            var state = ServiceLocator.TryGet(out WaveSaveHandler waveSaveHandler, false);

            if (state)
            {
                var ui = Instantiate(waveDataPrefab, panelBlackBackground.transform).GetComponent<WaveSaveUI>();
                panelBlackBackground.SetActive(true);
                ui.LoadUI(waveSaveHandler.waveSave);
                ui.SubToEvent(callback);
            }

            StartCoroutine(LoadingTimer(null, 1f, 0f, state));
        }

        public void OnLoadInteraction(Action callback, bool state)
        {
            if (state) OnSiegeStarted(() => { panelBlackBackground.SetActive(false); callback(); });
            else
            {
                loadingBackground.gameObject.SetActive(false);
                OverlayAnimationHandler.SetIsAnimating(false);
            }
        }


        #endregion


        #region SiegeAnimations


        /// <summary>
        /// When location had been confirmed begin loading animations
        /// </summary>
        /// <param name="callback"></param>
        public void OnSiegeStarted(Action callback, float start = 0f, float end = 1f)
        {
            OverlayAnimationHandler.SetIsAnimating(true);
            loadingBackground.gameObject.SetActive(true);
            UIAnimations.Instance.TransitionUIAlpha(loadingBackground, start, end, genericFadeSpeed, () =>
            {
                inGamePopUpHandler.ForceCloseMap();
                SetOverlayObjects(false);
                loadingPanel.GetComponent<LoadingScreenUI>().loadingState = LoadingScreenUI.LoadingState.LoadingInSiege;
                loadingPanel.gameObject.SetActive(true);
                UIAnimations.Instance.FadeIn(loadingPanel, loadingFadeSpeed);
                StartCoroutine(LoadingTimer(callback));

                //Do loading of objects
                objectLoader.LoadSiegeObjects(ServiceLocator.Get<LocationHandler>().ReturnCurrentLocation());
            });
        }

        /// <summary>
        /// Intial animation lock and fade out of const ui
        /// </summary>
        public void OnSiegeEnded()
        {
            OverlayAnimationHandler.ToggleLock(true);
            OverlayAnimationHandler.SetIsAnimating(true);
            UIAnimations.Instance.FadeOut(constantCanvasGroup, constFadeSpeed);
            loadingBackground.gameObject.SetActive(true);
        }

        /// <summary>
        /// Pops up siege end screen.
        /// </summary>
        public void OnSiegeEndIdleComplete()
        {
            UIAnimations.Instance.TransitionUIAlpha(loadingBackground, 0f, targetTintedAlpha, genericFadeSpeed, () =>
            {
                endScreenCanvasGroup.gameObject.SetActive(true);
                endScreenCanvasGroup.alpha = 0f;
                UIAnimations.Instance.FadeIn(endScreenCanvasGroup, endUIFadeSpeed, 0, () =>
                {
                    OverlayAnimationHandler.ToggleLock(false);
                    OverlayAnimationHandler.SetIsAnimating(false);
                });
            });
        }


        /// <summary>
        /// Load out of game.
        /// </summary>
        /// <param name="callback"></param>
        public void OnSiegeEndConfirmed(Action callback)
        {
            OverlayAnimationHandler.SetIsAnimating(true);
            topBlackBackground.gameObject.SetActive(true);
            UIAnimations.Instance.TransitionUIAlpha(topBlackBackground, 0f, 1f, endScreenBFS, () =>
            {
                Color color = loadingBackground.color;
                color.a = 1f;
                loadingBackground.color = color;

                endScreenCanvasGroup.gameObject.SetActive(false);
                topBlackBackground.gameObject.SetActive(false);

                SetOverlayObjects(true);
                loadingPanel.GetComponent<LoadingScreenUI>().loadingState = LoadingScreenUI.LoadingState.LoadingOutSiege;
                loadingPanel.gameObject.SetActive(true);
                UIAnimations.Instance.FadeIn(loadingPanel, loadingFadeSpeed);
                StartCoroutine(LoadingTimer(callback));

                //On siege ending behaviour

                objectLoader.UnloadSiegeObjects();

            });
        }


        private IEnumerator LoadingTimer(Action callback, float start = 1f, float end = 0f, bool backgroundState = false)
        {
            yield return new WaitForSecondsRealtime(loadingTimer);
            UIAnimations.Instance.FadeOut(loadingPanel, loadingFadeSpeed, () =>
            {
                loadingPanel.gameObject.SetActive(false);
                if (callback != null) { callback(); }
                UIAnimations.Instance.TransitionUIAlpha(loadingBackground, start, end, genericFadeSpeed, () =>
                {
                    loadingBackground.gameObject.SetActive(backgroundState);
                    OverlayAnimationHandler.SetIsAnimating(false);
                });
            });
        }

        private void SetOverlayObjects(bool nonSiege)
        {
            nonSiegeObjects.SetActive(nonSiege);
            siegeObjects.SetActive(!nonSiege);

            constantCanvasGroup.alpha = 1f;

            CameraLoc cam = nonSiege ? CameraLoc.Start : CameraLoc.Game;
            cameraController.SwapToCam(cam);
        }

        #endregion

    }
}
