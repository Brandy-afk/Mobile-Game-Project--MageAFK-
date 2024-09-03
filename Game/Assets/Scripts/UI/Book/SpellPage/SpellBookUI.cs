using UnityEngine;
using TMPro;
using UnityEngine.UI;
using MageAFK.Core;
using MageAFK.Spells;
using MageAFK.Animation;
using MageAFK.Stats;
using MageAFK.Player;
using Sirenix.OdinInspector;
using System.Collections;
using System;
using MageAFK.Management;
using MageAFK.Tools;
using System.Linq;

namespace MageAFK.UI
{
  public class SpellBookUI : SerializedMonoBehaviour, IPagination<Spell>
  {

    [SerializeField] private CatalogSlot[] slots;
    [SerializeField] private GameObject hoverIndicator;
    [SerializeField] private ButtonUpdateClass[] pageButtons;
    [SerializeField] private SpellHandler spellHandler;
    [SerializeField] private SpellSlotHandler spellSlotHandler;
    private Pagination<Spell> pagination;

    private void Awake()
    {
      IDragZoneCreator<Spell> creator = ServiceLocator.Get<IDragZoneCreator<Spell>>();
      creator.AddDragZone(GetComponent<RectTransform>(),
                          (bool state) => OnHovering(state),
                          OnDroppedInList,
                          DragZoneIndentifier.Spell_List);
    }

    private void Start() => Filter(true);


    private void OnHovering(bool state)
    {
      if (!DragHandler.IsAlterableState()) return;
      hoverIndicator.SetActive(state);
    }

    private bool OnDroppedInList()
    {
      if (!DragHandler.IsAlterableState()) return false;
      var draggedSpell = ServiceLocator.Get<IDragInfo<Spell, SpellIdentification>>().Drag;
      spellSlotHandler.EquipSpell(null, draggedSpell.SlotIndex, draggedSpell.type);
      Filter();
      return true;
    }

    public void Filter(bool isNewFilter = false)
    {
      SpellType type = SpellTypeGroup.CurrentType;
      Spell[] filteredSpells = spellHandler.ReturnSpellDict().Values
                                           .Where(spell => spell.type == type && spell.SlotIndex == SpellSlotIndex.None)
                                           .OrderBy(spell => spell.book)
                                           .ToArray();

      var page = isNewFilter ? 1 : pagination.ReturnCurrentPage();
      pagination = new Pagination<Spell>(filteredSpells, this, 6, page);
      pagination.UpdateDisplay();
      UpdatePageButtons();
    }

    public void UpdateSlot(Spell spell, int index) => slots[index].SetUp(spell);
    public void UpdatePageButtons() => Pagination<Spell>.UpdatePageButtons(pagination, pageButtons);
    public void AlterPagePressed(bool isNext)
    {
      if (isNext) pagination.NextPage();
      else pagination.PreviousPage();
    }
    public void CustomPaginationBehaviour()
    {
      return;
    }

  }

  public enum SpellSlotIndex
  {
    None,
    Spell1,
    Spell2,
    Spell3,
    Passive1,
    Passive2,
    Ult
  }


}