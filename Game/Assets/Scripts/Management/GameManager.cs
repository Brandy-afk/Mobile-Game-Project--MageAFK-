
using MageAFK.AI;
using MageAFK.Core;
using MageAFK.UI;
using UnityEditor.EditorTools;
using UnityEngine;

namespace MageAFK.Management
{
  public class GameManager : MonoBehaviour
  {


    [Header("References")]
    [SerializeField] private TransitionHandler transitionHandler;
    [SerializeField, Tooltip("Temp")] private TestingGame testingGame;

    #region Load Mechanic
    private static bool isLoad = true;
    public static void SetIsLoad(bool state) => isLoad = state;
    public static bool IsLoad => isLoad;
    #endregion

    private void Start()
    {
      if (testingGame.isTesting) return;
      transitionHandler.OnGameStart(InitializeData);
    }

    private void InitializeData()
    {
      SaveManager.OnSaveDataLoaded += OnDataLoaded;
      SaveManager.LoadAllData();

    }
    private void OnDataLoaded()
    {
      //On all data finished being loading, fade in and check if there is a wave save.
      SaveManager.OnSaveDataLoaded -= OnDataLoaded;
      transitionHandler.OnDataLoaded(OnLoadPressed);
    }

    private void OnLoadPressed(bool isLoad)
    {
      //On a decision being made in the wave save panel, game will alter based on decision to continue
      if (ServiceLocator.TryGet(out WaveSaveHandler waveSaveHandler, false))
      {
        if (isLoad)
        {
          //Set global variable "isLoad" to true so event callers can respond on their own.
          SetIsLoad(true);
          LocationHandler.SetLocation(waveSaveHandler.waveSave.waveData.location);
        }
        else
        {
          /// Behaviour on deciding to not continue wave...
          /// Delete wave save, remove it from service locator, and set load to false.
          SaveManager.Delete(DataType.WaveSaveData);
          ServiceLocator.RemoveService<WaveSaveHandler>();
          SetIsLoad(false);
        }

        //Regardless this will decide wether the wave save transitions to main menu or the wave.
        transitionHandler.OnLoadInteraction(isLoad ? () =>
            {
              waveSaveHandler?.LoadData();
              OnWaveDataLoaded();
            }
        : null, isLoad);
      }
    }

    private void OnWaveDataLoaded()
    {
      //This will occur only when the player has decided to reload their save.

      // Remove wave save, since it has been loaded.
      ServiceLocator.RemoveService<WaveSaveHandler>();
      // Scale AI according to loaded data
      ServiceLocator.Get<EntityDataManager>().ScaleAI();
      // Start siege.
      ServiceLocator.Get<WaveHandler>().StartSeige();
    }
  }

  #region Interfaces
  public interface IInitializer
  {
    void Initialization();
  }

  public interface IPostDataInitializer
  {
    void InitializePostDataLoaded();
  }
  #endregion

}
