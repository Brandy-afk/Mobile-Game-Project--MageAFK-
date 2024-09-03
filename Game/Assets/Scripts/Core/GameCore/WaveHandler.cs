using UnityEngine;
using System;
using MageAFK.UI;
using MageAFK.TimeDate;
using MageAFK.Player;
using MageAFK.Pooling;
using System.Collections.Generic;
using MageAFK.Spells;
using MageAFK.AI;
using MageAFK.Management;
using MageAFK.Items;
using Sirenix.OdinInspector;
using System.Linq;
using MageAFK.Tools;
using System.Collections;

namespace MageAFK.Core
{

  public class WaveHandler : SerializedMonoBehaviour, IData<WaveData>
  {

    [Header("INFORMATION - Time is all in minutes")]
    //Location Management
    [SerializeField] private Dictionary<Location, WaveInformation> waveInfoDict;
    [SerializeField, Tooltip("The time idled at the end of this state")] private float endWaveIdle, endSiegeIdle;

    [Header("References")]
    //References
    [SerializeField] private SiegeOverlayUI overlayUI;
    [SerializeField] private TransitionHandler transitionHandler;
    [SerializeField] private LocationUI locationUI;

    private static int waveTimeKey = -1;
    public static int WaveTimeKey => waveTimeKey;
    private static int wave = 0;
    public static int Wave => wave;

    #region Events
    private static event Action<WaveState> OnWaveStateChanged;
    private static WaveState _waveState = WaveState.None;
    public static WaveState WaveState
    {
      get { return _waveState; }

      private set
      {
        if (value != _waveState)
        {
          _waveState = value;
          OnWaveStateChanged?.Invoke(_waveState);
        }

      }
    }

    public static void SubToWaveState(Action<WaveState> handler, bool state, Priority priority = Priority.Middle)
    {
      if (state)
      {
        OnWaveStateChanged = Utility.InsertHandler(OnWaveStateChanged, handler, priority);
        handler(WaveState);
      }
      else
        OnWaveStateChanged -= handler;
    }

    private static event Action<Status> SiegeEvent;
    /// <summary>
    /// Subscribe to siege event for the start and end of sieges.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="state"></param>
    public static void SubToSiegeEvent(Action<Status> handler, bool state, Priority priority = Priority.Middle)
    {
      if (state)
        SiegeEvent = Utility.InsertHandler(SiegeEvent, handler, priority);
      else
        SiegeEvent -= handler;
    }
    private void InvokeSiegeEvent(Status state) => SiegeEvent?.Invoke(state);


    private static event Action<Status> WaveEvent;
    /// <summary>
    /// Sub to understand when waves(specific) start and end.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="state"></param>
    public static void SubToWaveEvent(Action<Status> handler, bool state, Priority priority = Priority.Middle)
    {
      if (state)
        WaveEvent = Utility.InsertHandler(WaveEvent, handler, priority);
      else
        WaveEvent -= handler;
    }
    private void InvokeWaveEvent(Status state) => WaveEvent?.Invoke(state);

    #endregion

    #region Initialization - Load/Save

    private void Awake() => ServiceLocator.RegisterService(this);

    public void InitializeData(WaveData data)
    {
      ServiceLocator.Get<EnemyPooler>().InitializeData(data.mobsIDs);
      wave = data.wave;
    }

    public WaveData SaveData() =>
                      new WaveData(wave,
                      EnemyPooler.currentMobs.Select(mob => mob.iD).ToList(),
                      ServiceLocator.Get<LocationHandler>().ReturnCurrentLocation());


    #endregion

    #region Script

    /// <summary>
    /// When Wave is being started intially from the menu.
    /// </summary>
    public void InitiateSiege() => locationUI.choosingLocation = true;

    /// <summary>
    /// Sets up a new siege.
    /// </summary>
    public void StartSeige()
    {
      ServiceLocator.RegisterService(new SiegeStatisticTracker());

      wave = 0;
      //Invoke siege start event.
      InvokeSiegeEvent(Status.Start);

      //If siege is not starting from load, transition regularly.
      if (!GameManager.IsLoad)
        transitionHandler.OnSiegeStarted(StartCounterPhase);
      else
      {
        //Else start on siege load.
        StartWave();
        GameManager.SetIsLoad(false);
      }

    }

    #region Intermission

    /// <summary>
    /// Start the intermission phase. (Invokes wave state)
    /// </summary>
    public void StartIntermissionPhase()
    {
      SetTimer(StartCounterPhase, waveInfoDict[ServiceLocator.Get<LocationHandler>().ReturnCurrentLocation()].intermissionTime * 60);
      WaveState = WaveState.Intermission;
    }

    #endregion

    #region Counter

    /// <summary>
    /// Starts counter phase and increments wave. (Invokes wave state)
    /// </summary>
    private void StartCounterPhase()
    {
      wave++;
      SetTimer(() => StartWave(), GetCounterTime());
      WaveState = WaveState.Counter;
    }

    #endregion

    #region Wave

    /// <summary>
    ///    Starts wave phase. (Invokes wave state and event.)
    /// </summary>
    public void StartWave()
    {
      SetTimer(EndWave, GetWaveTime());
      InvokeWaveEvent(Status.Start);
      WaveState = WaveState.Wave;
    }

    /// <summary>
    /// Ends wave phase, waiting till all enemies are dead and idle has been complete to move on.
    /// </summary>
    private void EndWave()
    {
      InvokeWaveEvent(Status.End); //Invoke end of wave event
      ServiceLocator.Get<EntityDataManager>().SubToEntityCount((int enemyCount) => //Sub to entitiy count to ensure all enemies are dead
      {
        if (enemyCount <= 0) // Once below 0, unsubscribe to event and begin idle. 
        {
          ServiceLocator.Get<EntityDataManager>().SubToEntityCount(null, false);
          if (WaveState != WaveState.None) StartCoroutine(Idle(() => // Once idle complete, add wave as finished, and begin intermission.
          {
            ServiceLocator.Get<PlayerData>().AddStatValue(PlayerStatisticEnum.Waves, 1);
            StartIntermissionPhase();
          }, endWaveIdle));
        }
      }, true);
    }

    #endregion

    #region Siege End


    /// <summary>
    /// Ends siege (player death.)
    /// </summary>
    public void EndSiege()
    {
      InvokeSiegeEvent(Status.End); //Invoke siege end event

      ServiceLocator.Get<PlayerData>().AddStatValue(PlayerStatisticEnum.Sieges, 1); // Update milestone
      ServiceLocator.Get<TimeTaskHandler>().EndTask(waveTimeKey, true); // End wave time task

      transitionHandler.OnSiegeEnded(); //Lock UI and close out of other UI
      StartCoroutine(Idle(transitionHandler.OnSiegeEndIdleComplete, endSiegeIdle)); //Start IDLE

      waveTimeKey = -1;
      WaveState = WaveState.None;
    }

    /// <summary>
    /// Called from UI endscreen. Loads out, and then cleans up scene.
    /// </summary>
    public void FinishSiege() => transitionHandler.OnSiegeEndConfirmed(() => InvokeSiegeEvent(Status.End_CleanUp));

    #endregion

    #region Time

    private IEnumerator Idle(Action callback, float timer)
    {
      yield return new WaitForSecondsRealtime(timer);
      callback?.Invoke();
    }

    private void SetTimer(Action callback, float duration)
    {
      var timeTaskHandler = ServiceLocator.Get<TimeTaskHandler>();
      waveTimeKey = timeTaskHandler.GetUniqueTimeKey();
      timeTaskHandler.AddTimer(callback
                      , overlayUI.UpdateWaveTime
                      , duration
                      , waveTimeKey);
    }

    private float GetCounterTime()
    {
      var currentLocation = ServiceLocator.Get<LocationHandler>().ReturnCurrentLocation();
      var counterDuration = waveInfoDict[currentLocation].baseCounterTime - (waveInfoDict[currentLocation].counterTimeVariance * wave);
      counterDuration = counterDuration < waveInfoDict[currentLocation].minCounterTime ? waveInfoDict[currentLocation].minCounterTime : counterDuration;
      return counterDuration *= 60;
    }

    private float GetWaveTime()
    {
      var currentLocation = ServiceLocator.Get<LocationHandler>().ReturnCurrentLocation();
      var waveDuration = waveInfoDict[currentLocation].baseWaveTime + (waveInfoDict[currentLocation].waveLengthVariance * wave);
      waveDuration = waveDuration > waveInfoDict[currentLocation].maxWaveTime ? waveInfoDict[currentLocation].maxWaveTime : waveDuration;
      return waveDuration *= 60;
    }
    #endregion



    #endregion

  }

  [Serializable]
  public class WaveData
  {
    //Wave information
    public int wave;
    public Location location;
    public List<EntityIdentification> mobsIDs;

    public WaveData(int wave,
    List<EntityIdentification> currentMobs, Location location)
    {
      this.wave = wave;
      mobsIDs = currentMobs;
      this.location = location;
    }
    public WaveData() { }
  }



  [System.Serializable]
  public class WaveInformation
  {

    public float intermissionTime;
    public float baseWaveTime;
    public float baseCounterTime;
    public float minCounterTime;
    public float maxWaveTime;
    [Header("Amount to be increased every wave onto wave time")]
    public float waveLengthVariance;
    [Header("Amount to decrease counter time every wave")]
    public float counterTimeVariance;
  }


  public enum WaveState
  {
    Wave,
    Counter,
    Intermission,
    None,
  }

  public enum Status
  {
    Start,
    End,
    End_CleanUp
  }


}

