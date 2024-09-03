using System;
using MageAFK.Animation;
using MageAFK.Spells;
using TMPro;
using UnityEngine;

namespace MageAFK.UI
{
  public class ActiveSlot : SpellSlot
  {

    #region Objects
    [SerializeField] private TMP_Text spellName;
    [SerializeField] private TMP_Text statusName;
    [SerializeField] private TMP_Text bookName;

    [SerializeField] private SpellSlotIndex index;
    [SerializeField] private SpellType type;
    [SerializeField] private GameObject container;
    [SerializeField] private CanvasGroup[] alphaArray;
    public SpellType Type => type;
    public Spell Spell => content;

    protected override DragZoneIndentifier zone { get => DragZoneIndentifier.Spell_Slot; }

    #endregion

    [Header("References")]
    [SerializeField] private SpellSlotHandler handler;




    void OnEnable()
    {
      if (content != null) { content.SubscribeToLevelChanged(UpdateSlot, true); }
    }

    void OnDisable()
    {
      if (content != null) { content.SubscribeToLevelChanged(UpdateSlot, false); }
    }

    #region UI
    public override void SetUp(Spell newSpell)
    {
      //If slot is to be made empty / null
      bool isNull = newSpell == null || dragInfo.ReturnIfDragging(newSpell.iD, DragZoneIndentifier.Spell_Slot);

      if (content != null) content.SubscribeToLevelChanged(UpdateSlot, false);
      content = isNull ? null : newSpell;
      ToggleObjects(!isNull);
      UpdateSlot();

      if (!isNull)
      {
        content.SubscribeToLevelChanged(UpdateSlot, true);
        FillUI(content, true);
      }
    }

    private void ToggleObjects(bool state)
    {
      container.SetActive(state);
      image.gameObject.SetActive(state);
    }

    private SpellIdentification lastInput = SpellIdentification.None;
    /// <summary>
    /// Based on spell para, will fill UI with relevant info. 
    /// </summary>
    /// <param name="spell"></param>
    /// <param name="alphaState">If true, it will be full opacity, otherwise half.</param>
    private void FillUI(Spell spell, bool alphaState)
    {
      UIAnimations.Instance.SetImageIn(image);
      SetAlpha(alphaState ? 1f : .5f);

      if (lastInput == spell.iD) return;
      lastInput = spell.iD;

      image.sprite = spell.image;
      spellName.text = spell.spellName;

      string statusSprite = spell.effect == StatusType.None ? "NoEffect" : spell.effect.ToString();
      statusName.text = $"<sprite name={statusSprite}>{spell.effect}";

      string bookSprite = spell.book == SpellBook.All ? "AllBooks" : spell.book.ToString();
      bookName.text = $"<sprite name={bookSprite}>{spell.book}";
    }

    private void SetAlpha(float alpha)
    {
      for (int i = 0; i < alphaArray.Length; i++)
        alphaArray[i].alpha = alpha;
    }

    private void UpdateSlot() => level.text = content == null ? "" : $"Level {content.Level}";
    #endregion

    #region  Interaction

      protected override void OnSingleClick() => infoPopUp.InputAndOpen(content);
      protected override void OnDoubleClick() => infoPopUp.InputAndOpen(content);
      public void OnHovering(bool state)
      {
        if (state)
        {
          FillUI(dragInfo.Drag, false);
        }
        else
        {
          if (content != null)
            FillUI(content, true);
          else
            ToggleObjects(false);
        }
      }

    #endregion

  }


}
