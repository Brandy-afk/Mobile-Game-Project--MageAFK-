using System.Collections;
using MageAFK.Animation;
using MageAFK.Management;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MageAFK.UI
{
  public abstract class DragButton<Content, ID> : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
  {
    protected abstract DragZoneIndentifier zone { get; }
    protected IDragActions<Content> dragActions;
    protected IDragInfo<Content, ID> dragInfo;
    protected virtual Content content { get; set; }

    protected virtual void Awake()
    {
      dragActions = ServiceLocator.Get<IDragActions<Content>>();
      dragInfo = ServiceLocator.Get<IDragInfo<Content, ID>>();
    }

    #region Virtual

    protected const float DOUBLE_THRESHOLD = 0.3f; // Adjust this value as needed
    protected bool isWaitingForSecondClick = false;
    public virtual void OnPointerClick(PointerEventData eventData)
    {
      if (!ReturnIfInteractable()) return;

      if (eventData.clickCount == 1)
      {
        if (!isWaitingForSecondClick)
        {
          isWaitingForSecondClick = true;
          StartCoroutine(WaitForSecondClick());
        }
      }
      else if (eventData.clickCount == 2)
      {
        if (ReturnIfAlterable())
          OnDoubleClick();
        else
          OnSingleClick();

        isWaitingForSecondClick = false;
        StopAllCoroutines();
      }
    }

    protected IEnumerator WaitForSecondClick()
    {
      yield return new WaitForSeconds(DOUBLE_THRESHOLD);

      if (isWaitingForSecondClick)
      {
        OnSingleClick();
        isWaitingForSecondClick = false;
      }
    }
    public virtual void OnBeginDrag(PointerEventData eventData)
    {
      if (ReturnIfInteractable())
        dragActions.OnDragStart(eventData, content, () => SetUp(content), zone);
      else
        eventData.pointerDrag = null;
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
      if (eventData.pointerDrag != null && ReturnIfInteractable())
      {
        dragActions.DuringDrag(eventData);
      }
    }

    public virtual void OnEndDrag(PointerEventData eventData) => dragActions.DragEnd();

    protected virtual bool ReturnIfInteractable() => content != null;
    protected virtual bool ReturnIfAlterable() => ReturnIfInteractable() && DragHandler.IsAlterableState();

    #endregion



    #region Abstract

    protected abstract void OnSingleClick();
    protected abstract void OnDoubleClick();
    public abstract void SetUp(Content spell);

    #endregion
  }
}