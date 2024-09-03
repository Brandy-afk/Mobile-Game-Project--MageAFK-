using System;
using System.Collections.Generic;
using MageAFK.Items;
using MageAFK.Management;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MageAFK.UI
{
    public class InventoryDragHandler : DragHandler, IDragInfo<Item, (ItemIdentification, ItemLevel)>, IDragZoneCreator<Item>, IDragActions<Item>
    {
        private void Awake()
        {
            ServiceLocator.RegisterService<IDragInfo<Item, (ItemIdentification, ItemLevel)>>(this);
            ServiceLocator.RegisterService<IDragZoneCreator<Item>>(this);
            ServiceLocator.RegisterService<IDragActions<Item>>(this);
        }

        #region IItemDragInfo
        private Item draggingItem;
        public Item Drag => draggingItem;

        public bool ReturnIfDragging((ItemIdentification, ItemLevel) key, DragZoneIndentifier loc)
        {
            var isDrag = draggingItem != null && isDragging && key.Equals((draggingItem.iD, draggingItem.ReturnLevel())) && dragOrgin == loc;
            return isDrag;
        }

        #endregion

        #region Dragging Action
        public void OnDragStart(PointerEventData eventData, Item item, Action reset, DragZoneIndentifier orgin)
        {
            dragItem.sprite = ServiceLocator.Get<IItemGetter>().ReturnItemData(item.iD).image;
            draggingItem = item;
            dragItem.gameObject.SetActive(true);
            OnDragStart(eventData, reset, orgin);
        }

        #endregion

        #region Clean Up
        protected override void CleanUp(bool isInvalid)
        {
            draggingItem = null;
            base.CleanUp(isInvalid);
        }

        #endregion
    }

}
