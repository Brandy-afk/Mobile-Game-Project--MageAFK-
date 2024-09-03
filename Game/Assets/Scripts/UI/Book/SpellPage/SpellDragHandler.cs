using System;
using System.Linq;
using MageAFK.Core;
using MageAFK.Items;
using MageAFK.Management;
using MageAFK.Spells;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MageAFK.UI
{
  public class SpellDragHandler : DragHandler, IDragInfo<Spell, SpellIdentification>, IDragZoneCreator<Spell>, IDragActions<Spell>
  {

    private void Awake()
    {
      ServiceLocator.RegisterService<IDragInfo<Spell, SpellIdentification>>(this);
      ServiceLocator.RegisterService<IDragZoneCreator<Spell>>(this);
      ServiceLocator.RegisterService<IDragActions<Spell>>(this);
    }

    private Spell draggingItem;
    public Spell Drag => draggingItem;


    public bool ReturnIfDragging(SpellIdentification key, DragZoneIndentifier loc)
    {
      var isDrag = draggingItem != null && isDragging && draggingItem.iD == key && dragOrgin == loc;
      return isDrag;
    }

    public void OnDragStart(PointerEventData eventData, Spell spell, Action reset, DragZoneIndentifier orgin)
    {
      dragItem.sprite = spell.image;
      draggingItem = spell;
      dragItem.gameObject.SetActive(true);
      OnDragStart(eventData, reset, orgin);
    }

    protected override void CleanUp(bool isInvalid)
    {
      draggingItem = null;
      base.CleanUp(isInvalid);
    }

  }
}