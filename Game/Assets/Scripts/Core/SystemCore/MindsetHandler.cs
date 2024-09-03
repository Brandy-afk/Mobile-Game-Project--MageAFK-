using System.Linq;
using MageAFK.Management;
using MageAFK.Stats;
using MageAFK.Tools;
using MageAFK.UI;
using UnityEngine;


namespace MageAFK.Core
{
  public class MindsetHandler : MonoBehaviour, IData<MindsetData>
  {

    [SerializeField] private int costToRefreshPerWave = 1000;
    [SerializeField] private MindsetBlueprint[] mindsetBlueprints;
    [SerializeField] private MindsetUI mindsetUI;

    [Header("Dont alter")]
    private Mindset[] mindsets = new Mindset[3];
    private int chosenIndex = -1;

    #region Initialization
    private void Awake() => ServiceLocator.RegisterService(this);
    private void Start() => WaveHandler.SubToWaveState(OnWaveStateChanged, true);
    public void InitializeData(MindsetData dataCollection)
    {
      for (int i = 0; i < 3 /* Should always be three since 3 mindsets */; i++)
      {
        var blueprint = mindsetBlueprints.FirstOrDefault(bp => bp.iD == dataCollection.IDs[i]);
        if (blueprint is not null)
        {
          var newMindSet = new Mindset(dataCollection.values[i], blueprint)
          {
            isChosen = i == dataCollection.chosenIndex
          };

          mindsets[i] = newMindSet;
        }
        else
          Debug.LogWarning("Error - Bad Key, Should not occur");
      }

      chosenIndex = -2; /* Represents loaded data */
      mindsetUI.SetUpSlots(mindsets);
      mindsetUI.SetUpSlotsForWave(dataCollection.chosenIndex);
    }
    public MindsetData SaveData() => new MindsetData(mindsets.Select(m => m.value).ToArray(), mindsets.Select(m => m.iD).ToArray(), chosenIndex);

    #endregion

    private void OnWaveStateChanged(WaveState state)
    {
      switch (state)
      {
        case WaveState.Wave:
          if (chosenIndex != -2)
          {
            ToggleChosenMindState(true);
            mindsetUI.SetUpSlotsForWave(chosenIndex);
          }
          break;

        case WaveState.Counter:
          CreateNewMindStates();
          break;

        case WaveState.Intermission:
          ToggleChosenMindState(false);
          break;
      }
    }

    private void ToggleChosenMindState(bool state)
    {
      var chosenMindset = mindsets.FirstOrDefault(mindset => mindset.isChosen);
      if (chosenMindset == null)
      {
        ChangeChosenMindState(Random.Range(0, 3));
        chosenMindset = mindsets[chosenIndex];
      }


      StatHandlerBase.AlterEntityStats(chosenMindset.stat, chosenMindset.value / 100, chosenMindset.isForEnemy, state);
    }

    public void CreateNewMindStates()
    {
      // Ensure the currentMindSets list is initialized
      chosenIndex = -1;

      // Make sure there are at least 3 mindsets available
      if (mindsetBlueprints.Length < 3)
      {
        Debug.LogError("Not enough mindsets to choose from!");
        return;
      }

      // Shuffle the blueprints list
      Utility.ShuffleCollection(mindsetBlueprints);

      // Take the first 3 shuffled blueprints and create mindsets from them
      for (int i = 0; i < mindsets.Length; i++)
      {
        Mindset newMindset = new(mindsetBlueprints[i]);
        mindsets[i] = newMindset;
      }

      mindsetUI.SetUpSlots(mindsets);
    }

    public bool ChangeChosenMindState(int index)
    {
      if (chosenIndex == index) { return false; }

      if (chosenIndex != -1 && chosenIndex != -2)
      { mindsets[chosenIndex].isChosen = false; }

      chosenIndex = index;
      mindsets[chosenIndex].isChosen = true;
      return true;
    }

    public int ReturnCostToRefresh(int counter = 1)
    {
      return counter * costToRefreshPerWave * WaveHandler.Wave;
    }


  }

  #region Classes
  [System.Serializable]
  public class MindsetBlueprint
  {
    public MindsetIdentification iD;
    public string desc;

    [Header("For modification that reduces do this --> '-.2'")]
    public FloatMinMax values;

    public Stat stat;



    public bool isPercentage;
    public bool isForEnemy;

    [Header("Implies that the modification is negative(not good) for the player or entity")]
    public bool isNegativeForEntity;

  }

  [System.Serializable]
  public class Mindset
  {
    public MindsetIdentification iD;
    public string desc;
    public float value;

    public Stat stat;

    public bool isPercentage;
    public bool isForEnemy;
    public bool isNegativeForPlayer;

    //Data
    public bool isChosen = false;

    public Mindset(MindsetBlueprint blueprint)
    {
      InputBluePrintFields(blueprint);
      int minInt = (int)(blueprint.values.min * 100);
      int maxInt = (int)(blueprint.values.max * 100);
      int randomInt = Random.Range(minInt, maxInt + 1); // Include max in the range
      value = randomInt / 100.0f;
    }

    public Mindset(float value, MindsetBlueprint blueprint)
    {
      this.value = value;
      InputBluePrintFields(blueprint);

    }

    private void InputBluePrintFields(MindsetBlueprint blueprint)
    {
      iD = blueprint.iD;
      desc = blueprint.desc;
      stat = blueprint.stat;
      isPercentage = blueprint.isPercentage;
      isForEnemy = blueprint.isForEnemy;
      isNegativeForPlayer = blueprint.isNegativeForEntity;

    }
  }


  [System.Serializable]
  public class MindsetData
  {
    public MindsetIdentification[] IDs;
    public float[] values;

    public int chosenIndex;

    public MindsetData(float[] value, MindsetIdentification[] iD, int index)
    {
      values = value;
      IDs = iD;
      chosenIndex = index;
    }
  }
  #endregion

  public enum MindsetIdentification
  {
    Hasteful,
    Resolved,
    Sinister,
    Motivated,
    Desire,
    Intention
  }
}