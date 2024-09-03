
using System.Collections.Generic;
using UnityEngine;
using MageAFK.Spells;
using Sirenix.OdinInspector;
using MageAFK.Management;
using Unity.VisualScripting;
using MageAFK.Core;
using System.Linq;

namespace MageAFK.UI

{
  public class SpellSlotHandler : SerializedMonoBehaviour
  {

    [SerializeField] private Dictionary<SpellSlotIndex, ActiveSlot> slots;

    [Header("Spell Swap Control Options")]
    [SerializeField] private SiegeOverlayUI overlayUI;
    [SerializeField] private SpellBookUI spellBookUI;
    [SerializeField] private FocusUI focusUI;



    private void Awake()
    {
      IDragZoneCreator<Spell> creator = ServiceLocator.Get<IDragZoneCreator<Spell>>();
      foreach (var slot in slots)
        creator.AddDragZone(slot.Value.transform.GetComponent<RectTransform>(),
                                         (bool state) => OnHovering(slot.Key, state),
                                         () => TryEquippingSpell(slot.Key),
                                         DragZoneIndentifier.Spell_Slot);
    }



    private void OnHovering(SpellSlotIndex type, bool state)
    {
      if (!SpellDragHandler.IsAlterableState()) return;

      try
      {
        slots[type].OnHovering(state);
      }
      catch (KeyNotFoundException)
      {
        Debug.LogWarning($"Key not found : {type}");
      }
    }


    public bool TryEquippingSpell(SpellSlotIndex index)
    {
      if (!SpellDragHandler.IsAlterableState()) return false;

      var incomingSpell = ServiceLocator.Get<IDragInfo<Spell, SpellIdentification>>().Drag;
      var slot = slots[index];

      if (incomingSpell.SlotIndex == index) return false;

      if (incomingSpell.SlotIndex == SpellSlotIndex.None && slot.Spell != null)
      {
        EquipSpell(slot.Spell, incomingSpell.SlotIndex, slot.Type);
      }
      EquipSpell(incomingSpell, index, slot.Type);
      return true;
    }

    //Call with the index and spell for swapping, but make spell null if its being removed.
    public void EquipSpell(Spell spell, SpellSlotIndex index, SpellType type)
    {
      //Update filter ui view
      ServiceLocator.Get<SpellCastHandler>().SwapSpell(index, spell);

      //Update overlay ui 
      overlayUI.FillSpellSlot(spell, index);

      //Change slot ui
      slots[index].SetUp(spell);

      if (type == SpellType.Spell) focusUI.SwapFocusSlot(spell, index);

      if (spell != null && spell.SlotIndex == SpellSlotIndex.None)
        spellBookUI.Filter();
    }

  }
}

