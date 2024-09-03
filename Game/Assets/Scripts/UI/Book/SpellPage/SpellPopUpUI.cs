
using System.Collections.Generic;
using System.Linq;
using MageAFK.Animation;
using MageAFK.Core;
using MageAFK.Spells;
using MageAFK.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MageAFK.UI
{
  public class SpellPopUpUI : WavePopUp
  {
    [SerializeField] private TMP_Text desc, statusDesc, statCountText, bookText, statusText, damageTypeText, spellName, spellLevel;

    [SerializeField] private Image bookImage, statusImage;

    [SerializeField] private Animator animator;

    [SerializeField] private (TMP_Text record, TMP_Text value)[] records;


    //Private Variables
    public static Spell currentSpell;

    private Dictionary<SpellRecordID, TMP_Text> recordDict;




    //Call before opening panel
    public void InputAndOpen(Spell spell)
    {
      if (recordDict == null) recordDict = new Dictionary<SpellRecordID, TMP_Text>();

      if (spell != currentSpell)
      {
        currentSpell = spell;
        InputNewSpell();
      }
      Open();
    }

    public void ClosePanel() => Close();


    protected override void OnEnable()
    {
      base.OnEnable();
      bool levelState = WaveHandler.WaveState != WaveState.None;
      spellLevel.transform.parent.gameObject.SetActive(levelState);
      if (levelState)
        currentSpell.SubscribeToLevelChanged(UpdateLevel, true);



      currentSpell.SubscribeToRecordEvent(UpdateRecord, true);
    }

    protected override void OnDisable()
    {
      base.OnDisable();
      if (WaveHandler.WaveState != WaveState.None)
        currentSpell.SubscribeToLevelChanged(UpdateLevel, false);
      currentSpell.SubscribeToRecordEvent(UpdateRecord, false);
    }

    private void InputNewSpell()
    {
      InputRecords();
      InputInfoUI();
    }
    private void InputInfoUI()
    {

      bookText.text = currentSpell.book.ToString();
      statusText.text = currentSpell.effect.ToString();
      damageTypeText.text = currentSpell.damageType.ToString();

      desc.text = currentSpell.desc;
      statusDesc.text = GameResources.Spell.
      ReturnStatusDesc(currentSpell.effect);

      bookImage.sprite = GameResources.Spell.ReturnBookImage(currentSpell.book);
      statusImage.sprite = GameResources.Spell.ReturnStatusSprite(currentSpell.effect);

      spellName.text = currentSpell.spellName;
      spellName.colorGradient = GameResources.Spell.ReturnElementGradient(currentSpell.element);

      animator.runtimeAnimatorController = currentSpell.controller;

      statCountText.text = currentSpell.spellStats.Count.ToString();
    }

    private void UpdateLevel() => spellLevel.text = currentSpell.Level.ToString();
    private void InputRecords()
    {
      recordDict.Clear();
      SpellRecordID[] ids = currentSpell.recordDict.Keys.ToArray();
      for (int i = 0; i < records.Length; i++)
      {
        if (ids.Length - 1 >= i)
        {
          recordDict[ids[i]] = records[i].value;
          records[i].record.text = StringManipulation.AddSpacesBeforeCapitals(ids[i].ToString());
          UpdateRecord(ids[i], currentSpell.recordDict[ids[i]]);
        }
        else
        {
          records[i].record.text = "";
          records[i].value.text = "";
        }
      }
    }

    private void UpdateRecord(SpellRecordID type, float value)
    {
      if (recordDict.ContainsKey(type))
      {
        recordDict[type].text = type >= 0 ? value.ToString("N0") : $"{value:N0}%";
      }
      else
      {
        Debug.Log($"Bad Key : {type}");
      }
    }

  }

}