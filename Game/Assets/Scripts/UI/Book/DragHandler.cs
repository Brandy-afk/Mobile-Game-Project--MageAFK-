using System;
using System.Collections.Generic;
using System.Linq;
using MageAFK.Core;
using MageAFK.Items;
using MageAFK.Management;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MageAFK.UI
{
  public abstract class DragHandler : MonoBehaviour
  {
    [SerializeField] protected Image dragItem;
    [SerializeField] protected RectTransform parent;

    #region Classes
    [Serializable]
    protected class DragZone
    {
      public DragZoneIndentifier indentifier;
      public RectTransform zone;
      public Action<bool> update;
      public Func<bool> dropped;

      public DragZone(RectTransform zone, Action<bool> update, Func<bool> dropped, DragZoneIndentifier indentifier)
      {
        this.zone = zone;
        this.update = update;
        this.dropped = dropped;
        this.indentifier = indentifier;
      }
    }
    #endregion

    protected List<DragZone> dragZones;
    //For tracking the current zone the item is in.
    protected DragZone currentZone;
    //All potential Zones
    protected Action resetCallBack;
    protected bool isDragging = false;


    #region Game Based
    private static readonly WaveState[] nonAlterableStates = {
            WaveState.Wave,
            WaveState.None
        };

    public static bool IsAlterableState() => !nonAlterableStates.Contains(WaveHandler.WaveState);
    #endregion
    // private void Awake()
    // {
    //   ServiceLocator.RegisterService<IDragInfo>(this);
    //   ServiceLocator.RegisterService<IDragZoneCreator>(this);
    //   ServiceLocator.RegisterService<IDragActions>(this);
    // }

    #region IItemDragInfo
    protected DragZoneIndentifier dragOrgin;
    public DragZoneIndentifier Orgin => dragOrgin;

    #endregion

    public void AddDragZone(RectTransform zone, Action<bool> update, Func<bool> drop, DragZoneIndentifier indentifier)
    {
      dragZones ??= new List<DragZone>();
      dragZones.Add(new DragZone(zone, update, drop, indentifier));
    }

    #region Dragging Action
    public void OnDragStart(PointerEventData eventData, Action reset, DragZoneIndentifier orgin)
    {
      resetCallBack = reset;
      dragOrgin = orgin;
      isDragging = true;
      SetDragLocation(eventData);
      resetCallBack();
    }

    public virtual void DuringDrag(PointerEventData eventData)
    {
      SetDragLocation(eventData);

      foreach (var dragZone in dragZones)
      {
        if (dragZone.zone.gameObject.activeInHierarchy && dragZone.indentifier != dragOrgin &&
        RectTransformUtility.RectangleContainsScreenPoint(dragZone.zone, eventData.position, null))
        {
          dragZone.update(true);
          currentZone = dragZone;
          return;
        }
      }

      ResetCurrentZone();
    }

    protected void SetDragLocation(PointerEventData eventData)
    {
      RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, eventData.position, null, out var localPoint);
      dragItem.rectTransform.anchoredPosition = localPoint;
    }

    public virtual void DragEnd()
    {
      isDragging = false;
      if (currentZone != null)
      {
        if (currentZone.dropped())
        {
          //When equipping gear, you are assigning only one of its value, meaning it needs to update the slot regardless.
          CleanUp(currentZone.indentifier == DragZoneIndentifier.Gear ? true : false);
          return;
        }
      }

      CleanUp(true);
    }
    #endregion

    #region Clean Up
    protected virtual void CleanUp(bool isInvalid)
    {
      ResetCurrentZone();
      dragOrgin = DragZoneIndentifier.None;
      dragItem.gameObject.SetActive(false);
      if (isInvalid && resetCallBack != null) resetCallBack();
      resetCallBack = null;
    }

    protected void ResetCurrentZone()
    {
      if (currentZone != null)
      {
        currentZone.update(false);
        currentZone = null;
      }
    }

    #endregion
  }

  public interface IDragInfo<Obj, Key>
  {
    DragZoneIndentifier Orgin { get; }
    Obj Drag { get; }
    bool ReturnIfDragging(Key key, DragZoneIndentifier loc);
  }

  public interface IDragZoneCreator<Obj>
  {
    void AddDragZone(RectTransform zone, Action<bool> update, Func<bool> drop, DragZoneIndentifier indentifier);
  }

  public interface IDragActions<Obj>
  {
    void OnDragStart(PointerEventData eventData, Obj value, Action reset, DragZoneIndentifier orgin);
    void DuringDrag(PointerEventData eventData);
    void DragEnd();
  }


  public enum DragZoneIndentifier
  {
    None,
    Gear,
    Inventory,
    Void,
    Spell_List,
    Spell_Slot
  }

}
