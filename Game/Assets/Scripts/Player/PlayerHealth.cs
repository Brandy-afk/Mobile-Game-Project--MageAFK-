using System;
using System.Collections;
using MageAFK.AI;
using MageAFK.Core;
using MageAFK.Management;
using MageAFK.Stats;
using UnityEngine;

namespace MageAFK.Player
{
  public class PlayerHealth : MonoBehaviour
  {

    [SerializeField] private float baseHealth;
    [SerializeField] private float increasedHealthPerLevel = 25;

    private Coroutine healthRegen;



    //Health Regen  
    private const int REGEN_TICK_RATE = 5;



    #region Initialization

    private void Awake() => ServiceLocator.RegisterService(this);

    private void Start()
    {
      ServiceLocator.Get<PlayerStatHandler>().SubscribeToStatEvent(Stat.Health, RecalculateHealth, true);
      ServiceLocator.Get<LevelHandler>().SubscribeToLevelChanged(OnLevelUp, true);
      WaveHandler.SubToWaveState(OnWaveStateChanged, true);
    }

    #endregion

    #region Event
    // Event declaration
    public event Action<float, float> OnHealthChanged;
    public event Action<float> OnHealthChangedByAmount;

    // Field and property for healthPoints
    private float _healthPoints = 100;
    public float Health
    {
      get { return _healthPoints; }
      set
      {
        // Store the old value to calculate the change
        float oldValue = _healthPoints;

        // Update the health points
        _healthPoints = value;

        // Calculate the change in health
        float healthChange = _healthPoints - oldValue;

        // Invoke the event with the current health and max health
        OnHealthChanged?.Invoke(_healthPoints, _maxHealthPoints);

        // Invoke another event with the change in health (e.g., -20 if 20 health points were lost)
        if (healthChange != 0)
          OnHealthChangedByAmount?.Invoke(healthChange);
      }
    }

    // Field and property for maxHealthPoints
    private float _maxHealthPoints = 100;
    public float MaxHealth
    {
      get { return _maxHealthPoints; }
      set
      {
        _maxHealthPoints = value;
        OnHealthChanged?.Invoke(_healthPoints, _maxHealthPoints);  // Invoke event when MaxHealthPoints changes
      }
    }

    public void SubscribeToHealthChanged(Action<float, float> handler, bool state)
    {
      if (state)
      {
        OnHealthChanged += handler;
        OnHealthChanged?.Invoke(_healthPoints, _maxHealthPoints);
      }
      else
        OnHealthChanged -= handler;
    }


    private void OnWaveStateChanged(WaveState state)
    {
      if (state == WaveState.Wave)
      {
        healthRegen = StartCoroutine(HealthRegenRoutine());
      }
      else if (state == WaveState.Intermission)
      {
        if (healthRegen != null) { StopCoroutine(healthRegen); healthRegen = null; }
        Health = MaxHealth;
      }
    }

    private void OnLevelUp(int level) => RecalculateHealth();

    #endregion

    #region Helpers
    public void SetHealthToBase()
    {
      MaxHealth = baseHealth;
      Health = baseHealth;
    }

    public void RecalculateHealth()
    {
      MaxHealth = ServiceLocator.Get<PlayerStatHandler>().ReturnModifiedValue(Stat.Health, baseHealth + (ServiceLocator.Get<LevelHandler>().ReturnCurrentLevel() * increasedHealthPerLevel));

      if (WaveHandler.WaveState == WaveState.Wave)
        Health += ServiceLocator.Get<PlayerStatHandler>().ReturnModifiedValue(Stat.Health, increasedHealthPerLevel);
      else
        Health = MaxHealth;
    }

    private IEnumerator HealthRegenRoutine()
    {
      Health = Mathf.Min(MaxHealth, Health += ServiceLocator.Get<PlayerStatHandler>().ReturnModification(Stat.HealthRegen, MaxHealth));

      yield return new WaitForSeconds(REGEN_TICK_RATE);
    }

    #endregion

    public (bool, float) Damage(float flatDamage, NPEntity entity = null, bool resistDamage = true)
    {
      //Handles all neccesary computations upon getting hit. 
      float modifiedDmg = PlayerDamageHandler.HandlePlayerDamaged(flatDamage, entity, resistDamage);
      float actualDamage = Mathf.Min(modifiedDmg, Health);
      Health -= actualDamage;

      return (Health <= 0, actualDamage);
    }




  }
}
