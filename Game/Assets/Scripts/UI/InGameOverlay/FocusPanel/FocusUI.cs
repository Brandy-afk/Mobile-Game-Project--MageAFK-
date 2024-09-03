using System.Collections.Generic;
using MageAFK.Spells;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MageAFK.Core;
using MageAFK.Tools;
using System;
using Sirenix.OdinInspector;
using MageAFK.Animation;
using MageAFK.AI;

namespace MageAFK.UI
{
  public class FocusUI : SerializedMonoBehaviour
  {
    [SerializeField, BoxGroup("Basic")] private Dictionary<SpellSlotIndex, FocusUISlot> uiSlots;
    [SerializeField, BoxGroup("Basic")] private Dictionary<FocusEntity, Sprite> focusImages;
    [SerializeField, BoxGroup("Basic")] private GameObject[] buttonMasks;
    private CanvasGroup group;




    [SerializeField, BoxGroup("Animation"), ReadOnly] private Vector2 rightLocation;
    [SerializeField, BoxGroup("Animation"), ReadOnly] private Vector2 leftLocation;
    [SerializeField, BoxGroup("Animation"), ReadOnly] private Vector2 centerLocation;
    [SerializeField, BoxGroup("Animation")] private float animationSpeed;

    [SerializeField, BoxGroup("References")] private SpellCastHandler spellCH;


    #region Readonly Arrays
    private static readonly FocusEntity[] focusArray = new FocusEntity[]
    { FocusEntity.Enemy1, FocusEntity.Enemy2, FocusEntity.Enemy3, FocusEntity.Random, FocusEntity.ClosestTarget};
    private static readonly SpellSlotIndex[] spellArray = new SpellSlotIndex[]
    { SpellSlotIndex.Spell1, SpellSlotIndex.Spell2, SpellSlotIndex.Spell3};

    #endregion

    #region Classes
    [Serializable]
    public class FocusUISlot
    {
      public GameObject[] buttonMasks;
      public Image spell, monster;
      public TMP_Text focusName, spellName;

      public void SetMaskStates(bool state)
      {
        for (int i = 0; i < buttonMasks.Length; i++)
          buttonMasks[i].SetActive(state);
      }
    }

    #endregion

    private void Awake()
    {
      group = GetComponent<CanvasGroup>();
      WaveHandler.SubToWaveState((WaveState state) =>
      {
        if (state == WaveState.Counter)
          ResetFocusOnCounterStart();
      }, true);
    }

    private void OnEnable()
    {
      foreach (var pair in uiSlots)
      {
        if (!spellCH.TryGetSpell(pair.Key, out Spell spell)) continue;
        pair.Value.monster.rectTransform.anchoredPosition = centerLocation;
        pair.Value.monster.sprite = ReturnMobImage(spell.Focus);
      }

      WaveHandler.SubToWaveState(OnWaveStateChanged, true);
    }

    private void OnDisable() => WaveHandler.SubToWaveState(OnWaveStateChanged, false);

    private void OnWaveStateChanged(WaveState state)
    {
      bool isIntermission = state == WaveState.Intermission;

      if ((isIntermission && !buttonMasks[0].activeSelf) || (!isIntermission && buttonMasks[0].activeSelf))
      {
        foreach (var pair in uiSlots)
        {
          if (!spellCH.TryGetSpell(pair.Key, out Spell spell) || spell.forcedFocus != FocusEntity.None) continue;
          pair.Value.SetMaskStates(isIntermission);
        }

        for (int i = 0; i < buttonMasks.Length; i++)
          buttonMasks[i].SetActive(isIntermission);

        group.interactable = !isIntermission;
      }
    }

    public void SwapFocusSlot(Spell newSpell, SpellSlotIndex index)
    {
      bool isNull = newSpell == null;
      uiSlots[index].focusName.text = isNull ? "" : ReturnFocusText(newSpell);
      uiSlots[index].spellName.text = isNull ? "" : newSpell.spellName;
      uiSlots[index].spell.gameObject.SetActive(!isNull);
      uiSlots[index].monster.gameObject.SetActive(!isNull);
      if (isNull || newSpell.forcedFocus != FocusEntity.None) uiSlots[index].SetMaskStates(true);

      if (!isNull)
      {
        uiSlots[index].spell.sprite = newSpell.image;
        if (newSpell.forcedFocus == FocusEntity.None) newSpell.SetFocusEntitiy(FocusEntity.ClosestTarget);
        UpdateFocusSlot(index);
      }
    }

    private void UpdateFocusSlot(SpellSlotIndex index)
    {
      if (spellCH.TryGetSpell(index, out Spell spell))
      {
        uiSlots[index].focusName.text = ReturnFocusText(spell);
        uiSlots[index].monster.sprite = ReturnMobImage(spell.Focus);
      }
    }

    private string ReturnFocusText(Spell spell)
    {

      string focusText = null;
      var focusEntity = spell.Focus;

      if (spell.forcedFocus != FocusEntity.None)
      {
        var mobName = EntityHandler.ReturnMobWithFocus(focusEntity)?.name;
        if (!string.IsNullOrEmpty(mobName))
        {
          focusText = mobName;
        }
      }
      // If focusText is still null, it means either the focus entity was not Enemy1, Enemy2, Enemy3, the ReturnMobWithEntity returned null, or its a forced focus.
      return focusText != null ? focusText : StringManipulation.AddSpacesBeforeCapitals(focusEntity.ToString());
    }

    private Sprite ReturnMobImage(FocusEntity focusEntity)
    {
      Sprite sprite = EntityHandler.ReturnMobWithFocus(focusEntity)?.headShotImage;
      if (sprite == null)
      {
        focusImages.TryGetValue(focusEntity, out sprite);
      }
      return sprite;
    }


    #region Interaction
    public void OnLeftPressed(int index)
    {
      if (CheckSpellAndReturn(index, out Spell spell))
      {
        IterateFocus(false, spell);
      }
    }

    public void OnRightPressed(int index)
    {
      if (CheckSpellAndReturn(index, out Spell spell))
      {
        IterateFocus(true, spell);
      }
    }

    private void IterateFocus(bool isRight, Spell spell)
    {
      int currentIndex = Array.IndexOf(focusArray, spell.Focus);
      int nextIndex = (currentIndex + (isRight ? 1 : -1) + focusArray.Length) % focusArray.Length;
      spell.SetFocusEntitiy(focusArray[nextIndex]);
      //Animation here

      var index = spell.SlotIndex;
      StartAnimation(index, isRight);
    }

    private bool CheckSpellAndReturn(int index, out Spell spell)
    {
      if (spellCH.TryGetSpell(spellArray[index], out spell) && spell.forcedFocus == FocusEntity.None)
        return true;
      else
        return false;
    }

    /// <summary>
    /// For button interaction not for code use.
    /// </summary>
    /// <param name="index"></param>
    public void OnQuickAlterPressed(int index)
    {
      QuickAlter(focusArray[index]);
    }

    public void QuickAlter(FocusEntity focusEntity)
    {
      foreach (var slot in uiSlots)
      {
        if (spellCH.TryGetSpell(slot.Key, out Spell spell) && spell.forcedFocus == FocusEntity.None)
        {
          spell.SetFocusEntitiy(focusEntity);
          StartAnimation(slot.Key, true);
        }
      }
    }

    private void ResetFocusOnCounterStart()
    {
      foreach (var slot in uiSlots)
      {
        if (spellCH.TryGetSpell(slot.Key, out Spell spell) && spell.forcedFocus == FocusEntity.None)
        {
          var focus = spell.Focus;

          if (focus == FocusEntity.Enemy1 || focus == FocusEntity.Enemy2 || focus == FocusEntity.Enemy3)
          {
            spell.SetFocusEntitiy(FocusEntity.ClosestTarget);
            StartAnimation(slot.Key, true);
          }
        }
      }
    }


    #endregion

    #region Animations

    private void StartAnimation(SpellSlotIndex index, bool isRight)
    {
      if (gameObject.activeInHierarchy)
        UIAnimations.Instance.SlideLocal(uiSlots[index].monster.rectTransform,
                                        isRight ? leftLocation : rightLocation,
                                        animationSpeed,
                                        () =>
                                        {
                                          UpdateFocusSlot(index);
                                          uiSlots[index].monster.rectTransform.anchoredPosition = isRight ? rightLocation : leftLocation;
                                          UIAnimations.Instance.SlideLocal(uiSlots[index].monster.rectTransform, centerLocation, animationSpeed, null, LeanTweenType.easeInQuad);
                                        },
                                        LeanTweenType.easeOutExpo,
                                        centerLocation);

      else
        UpdateFocusSlot(index);
    }

    #endregion
  }

}

