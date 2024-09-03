using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MageAFK.Core;
using UnityEngine;

namespace MageAFK.Combat
{
    public class DynamicActionExecutor : MonoBehaviour
    {
        public static DynamicActionExecutor Instance;

        private readonly HashSet<IWaveAction> waveActions = new HashSet<IWaveAction>();
        public List<ISpellCollisionEvent> spellCollisions = new List<ISpellCollisionEvent>();
        private bool sortSpellCollisions = false;
        public List<IPlayerCollisionEvent> playerCollisions = new List<IPlayerCollisionEvent>();

        private bool sortPlayerCollisions = false;

        private void Awake()
        {
            if (Instance != null)
                Destroy(this);
            else
                Instance = this;
        }
        
        private void Start() => WaveHandler.SubToWaveState(OnWaveStateChanged, true);

        #region Adding / Removing
        public void AddDynamicAction(IDynamicAction action)
        {
            switch (action)
            {
                case IWaveAction:
                    waveActions.Add(action as IWaveAction);
                    break;
                case ISpellCollisionEvent:
                    spellCollisions.Add(action as ISpellCollisionEvent);
                    sortSpellCollisions = true;
                    break;
                case IPlayerCollisionEvent:
                    playerCollisions.Add(action as IPlayerCollisionEvent);
                    sortPlayerCollisions = true;
                    break;
            }
        }

        public void RemoveDynamicAction(IDynamicAction action)
        {
            switch (action)
            {
                case IWaveAction:
                    waveActions.Remove(action as IWaveAction);
                    break;
                case ISpellCollisionEvent:
                    spellCollisions.Remove(action as ISpellCollisionEvent);
                    break;
                case IPlayerCollisionEvent:
                    playerCollisions.Remove(action as IPlayerCollisionEvent);
                    break;
            }
        }

        #endregion

        private void OnWaveStateChanged(WaveState state)
        {
            switch (state)
            {
                case WaveState.Wave:
                    OnWaveStarted();
                    break;
                case WaveState.Counter:
                    break;
                default:
                    OnWaveOrSiegeEnd();
                    break;
            }
        }

        private void OnWaveStarted()
        {
            if (sortSpellCollisions)
            {
                spellCollisions = spellCollisions.OrderBy(collisionEvent => collisionEvent.ReturnPriority()).ToList();
                sortSpellCollisions = false;
            }

            if (sortPlayerCollisions)
            {
                playerCollisions = playerCollisions.OrderBy(collisionEvent => collisionEvent.ReturnPriority()).ToList();
                sortPlayerCollisions = false;
            }

            foreach (IWaveAction action in waveActions)
            {
                if (action is IWaveStartAction startAction)
                    startAction.OnWaveStart();

                if (action is IWaveUpdateAction updateAction)
                    StartCoroutine(WaveActionUpdate(updateAction));
            }
        }

        private void OnWaveOrSiegeEnd()
        {
            StopAllCoroutines();

            foreach (IWaveAction action in waveActions)
            {
                if (action is IWaveEndAction endAction)
                    endAction.OnWaveEnd();
            }
        }

        #region IWaveAction
        private IEnumerator WaveActionUpdate(IWaveUpdateAction action)
        {
            while (WaveHandler.WaveState == WaveState.Wave)
            {
                action.UpdateAction();
                yield return new WaitForSeconds(action.ReturnTimeBetweenUpdates());
            }
        }
        #endregion

    }


}
