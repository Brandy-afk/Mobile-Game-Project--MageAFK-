using System.Collections.Generic;
using MageAFK.Spells;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MageAFK.Animation;
using MageAFK.Core;
using MageAFK.Management;
using Sirenix.OdinInspector;

namespace MageAFK.UI
{
  //TODO this needs a data field to save spell unlocks
  public class SpellTypeGroup : SerializedMonoBehaviour
  {
    [Header("Objects")]
    [SerializeField] private Dictionary<SpellType, TypeRef> typeFields;

    [System.Serializable]
    public class TypeRef
    {
      public Button button;
      public Image image;
      public GameObject lockObj, tabName;

      public CanvasGroup panel;
    }

    [Header("Animations")]
    [SerializeField] private float animationSpeed;
    [SerializeField] private float timeBetweenPanels;

    [SerializeField] private Sprite onImage;
    [SerializeField] private Sprite offImage;


    [Header("References")]
    [SerializeField] private SpellBookUI spellBookUI;
    private static SpellType currentType = SpellType.Spell;
    public static SpellType CurrentType => currentType;
    private SpellType incomingType;


    public void ToggleTab(bool state, SpellType type)
    {
      var tab = typeFields[type];
      tab.button.interactable = state;
      tab.tabName.SetActive(state);
      tab.lockObj.SetActive(!state);
    }

    public void OnTabSelected(int index)
    {
      incomingType = (SpellType)index;
      if (incomingType == currentType) return;
      ResetTabs();

      TypeRef refs = typeFields[incomingType];
      refs.image.sprite = onImage;
      SwapObjects();
    }

    public void ResetTabs()
    {
      foreach (TypeRef reference in typeFields.Values)
        reference.image.sprite = offImage;
    }

    private void SwapObjects()
    {
      OverlayAnimationHandler.SetIsAnimating(true);
      UIAnimations.Instance.FadeOut(typeFields[currentType].panel, animationSpeed, FadeInNewPanel);
    }

    private void FadeInNewPanel()
    {
      typeFields[currentType].panel.gameObject.SetActive(false);
      typeFields[incomingType].panel.gameObject.SetActive(true);
      currentType = incomingType;
      spellBookUI.Filter(true);
      UIAnimations.Instance.FadeIn(typeFields[currentType].panel, animationSpeed, timeBetweenPanels, () => OverlayAnimationHandler.SetIsAnimating(false));
    }


  }
}