using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MageAFK.Core;
using MageAFK.Player;
using MageAFK.Stats;
using MageAFK.Management;
using MageAFK.UI;
using MageAFK.AI;
using MageAFK.TimeDate;
using MageAFK.Tools;
using MageAFK.Pooling;
using MageAFK.Combat;

namespace MageAFK.Spells
{
  public class SpellCastHandler : MonoBehaviour
  {

    [SerializeField] private GameObject ultIndicatorPrefab;

    [Header("References")]
    [SerializeField] private PlayerStateManager playerStateManager;
    [SerializeField] private SiegeOverlayUI overlayUI;
    private EntityTracker entityTracker;
    private Transform indicatorObject;

    //Spell slots (index 0-2 is spells, 3-4 passives, 5 is ultimate)
    private static Dictionary<SpellSlotIndex, Spell> slots = new()
    {
     {SpellSlotIndex.Spell1, null},
     {SpellSlotIndex.Spell2, null},
     {SpellSlotIndex.Spell3, null},
     {SpellSlotIndex.Passive1, null},
     {SpellSlotIndex.Passive2, null},
     {SpellSlotIndex.Ult, null},
    };

    private void Awake() => ServiceLocator.RegisterService<SpellCastHandler>(this);

    private void Start()
    {
      WaveHandler.SubToWaveState(OnWaveStateChanged, true);
      entityTracker = ServiceLocator.Get<EntityTracker>();
    }


    private void OnWaveStateChanged(WaveState state)
    {
      if (state == WaveState.Wave)
        StartCasting();
      else if ((state == WaveState.None || state == WaveState.Intermission) && Ultimate.placableMode)
        ServiceLocator.Get<TimeTaskHandler>().EndTask(Ultimate.ultTimeKey, false);
    }


    private void Update()
    {
      if (Ultimate.placableMode) PlacingUltBehaviour();
    }


    #region HelperFunctions
    public List<Spell> ReturnPoolableSpells()
    {
      List<Spell> spells = new();
      foreach (Spell spell in slots.Values)
      {
        if (spell == null) { continue; }

        spells.Add(spell);

      }
      return spells;
    }
    public void SwapSpell(SpellSlotIndex index, Spell spell)
    {
      //Set it for the focusHandler as well
      if (slots[index] is not null)
      {
        slots[index].SetSlotIndex(SpellSlotIndex.None);
        if (slots[index] is IDynamicAction action) DynamicActionExecutor.Instance.RemoveDynamicAction(action);
      }

      slots[index] = spell;

      if (spell is not null)
      {
        spell.SetSlotIndex(index);
        if (spell is IDynamicAction action) DynamicActionExecutor.Instance.AddDynamicAction(action);
      }
    }
    private void StartCasting()
    {
      foreach (Spell spell in slots.Values)
      {
        if (spell == null || spell.type == SpellType.Ultimate) { continue; }
        if (spell.type == SpellType.Passive) { StartCoroutine(CastPassiveCoroutine(spell)); }
        if (spell.type == SpellType.Spell) { StartCoroutine(CastSpellCoroutine(spell)); }
      }
    }
    public bool TryGetSpell(SpellSlotIndex index, out Spell spell)
    {
      spell = slots[index];
      return spell != null;
    }

    #endregion

    #region Misc Manipulation

    public static Spell ReturnRandomActiveSpell()
    {
      List<Spell> spells = new();
      foreach (var spell in slots.Values)
      {
        if (spell != null && spell.type == SpellType.Spell)
        {
          spells.Add(spell);
        }
      }

      return spells[Random.Range(0, spells.Count)];
    }


    #endregion

    #region Casting

    #region Spells/Passive
    IEnumerator CastSpellCoroutine(Spell spell)
    {
      try
      {
        while (WaveHandler.WaveState == WaveState.Wave)
        {
          if (entityTracker.IsTarget)
          {
            spell.Activate();
            playerStateManager.ChangeCurrentState(EntityAnimation.SpellCast);
            spell.OnCast();

            float cooldown = spell.ReturnStatValue(Stat.Cooldown);
            float elapsedTime = 0;
            while (elapsedTime < cooldown)
            {
              elapsedTime += Time.deltaTime;
              overlayUI.UpdateSpellSlot(spell, 1 - (elapsedTime / cooldown));
              yield return null;
            }
          }
          yield return null;
        }
      }
      finally
      {
        Debug.Log("Finally Called");
        overlayUI.UpdateSpellSlot(spell, 0);
        spell.OnWaveOver();
      }
    }

    IEnumerator CastPassiveCoroutine(Spell spell)
    {
      try
      {
        while (WaveHandler.WaveState == WaveState.Wave)
        {
          if (entityTracker.IsTarget)
          {
            spell.Activate();
            playerStateManager.ChangeCurrentState(EntityAnimation.SpellCast);
            spell.OnCast();


            float cooldown = spell.ReturnStatValue(Stat.Cooldown);
            float elapsedTime = 0;
            while (elapsedTime < cooldown && WaveHandler.WaveState == WaveState.Wave)
            {
              elapsedTime += Time.deltaTime;
              overlayUI.UpdateSpellSlot(spell, 1 - (elapsedTime / cooldown));
              yield return null;
            }
          }
          yield return null;
        }
      }
      finally
      {
        Debug.Log("Finally called");
        overlayUI.UpdateSpellSlot(spell, 0);
        spell.OnWaveOver();
      }
    }

    #endregion

    #region Ultimate

    private const float ULT_PLACABLE_TIME = 30f;
    public void UltPressed()
    {
      if (WaveHandler.WaveState != WaveState.Wave) return;

      var ult = slots[SpellSlotIndex.Ult] as Ultimate;
      if (Ultimate.ultTimeKey != -1 || ult == null || WaveHandler.WaveState == WaveState.None) return;

      ult.Activate();
      ult.OnCast();

      if (ult is IPlacableUlt)
      {
        ServiceLocator.Get<ObjectPooler>().CreatePool(PoolingObjects.UltimateIndicator);
        overlayUI.TogglePlaceableUltUI(true, ult);
        Ultimate.ultTimeKey = ServiceLocator.Get<TimeTaskHandler>().GetUniqueTimeKey();
        ServiceLocator.Get<TimeTaskHandler>().AddTimer(OnPlacableTimeOver, overlayUI.UpdateUltimateUseTimer, ULT_PLACABLE_TIME, Ultimate.ultTimeKey, true);
        Ultimate.placableMode = true;
        ServiceLocator.Get<TimeScaleHandler>().SetTimeScale(0.5f);
      }
      else
      {
        HandleUltTimer(ult.ReturnStatValue(Stat.Cooldown));
      }
    }



    public void HandleUltTimer(float duration)
    {

      var timeTaskHandler = ServiceLocator.Get<TimeTaskHandler>();
      Ultimate.ultTimeKey = timeTaskHandler.GetUniqueTimeKey();
      timeTaskHandler.AddTimer(OnUltCooldownOver
                                         , overlayUI.UpdateUltimateSlot
                                         , duration
                                         , Ultimate.ultTimeKey);
    }

    private void PlacingUltBehaviour()
    {
      if (Input.touchCount > 0)
      {
        Touch touch = Input.GetTouch(0);

        // Convert screen position of the touch into world position
        Vector2 touchPosition = Utility.MainCam.ScreenToWorldPoint(new Vector2(touch.position.x, touch.position.y));

        switch (touch.phase)
        {
          case TouchPhase.Began:
            indicatorObject = ServiceLocator.Get<ObjectPooler>().GetFromPool(PoolingObjects.UltimateIndicator).transform;
            indicatorObject.GetComponent<SpriteRenderer>().sprite = (slots[SpellSlotIndex.Ult] as Ultimate).visual;
            indicatorObject.gameObject.SetActive(true);
            break;

          case TouchPhase.Moved:
            if (!indicatorObject) return;
            indicatorObject.position = touchPosition;
            break;
          case TouchPhase.Stationary:
            if (!indicatorObject) return;
            indicatorObject.position = touchPosition;
            break;

          case TouchPhase.Ended:
            if (!indicatorObject) return;
            indicatorObject.gameObject.SetActive(false);

            var spawnCap = slots[SpellSlotIndex.Ult].ReturnStatValue(Stat.SpawnCap, false);
            var val = (slots[SpellSlotIndex.Ult] as IPlacableUlt).OnPlaced(touchPosition);

            if (val <= 0)
              ServiceLocator.Get<TimeTaskHandler>().EndTask(Ultimate.ultTimeKey, false);
            else
              overlayUI.UpdateUltimateUses(val, (int)spawnCap);

            break;
        }
      }
    }

    private void OnPlacableTimeOver()
    {
      Ultimate.placableMode = false;
      ServiceLocator.Get<ObjectPooler>().Clear(PoolingObjects.UltimateIndicator);
      overlayUI.TogglePlaceableUltUI(false);
      ServiceLocator.Get<TimeScaleHandler>().SetScaleToCurrentIndex();

      if (WaveHandler.WaveState != WaveState.None)
        HandleUltTimer(ServiceLocator.Get<PlayerStatHandler>().ReturnModifiedValue(Stat.Cooldown, slots[SpellSlotIndex.Ult].ReturnStatValue(Stat.Cooldown)));
    }

    private void OnUltCooldownOver()
    {
      Ultimate.ultTimeKey = -1;
      overlayUI.UpdateUltimateSlot();
      //Maybe some animation to signify it being up now
    }

    #endregion

    #endregion

  }


}