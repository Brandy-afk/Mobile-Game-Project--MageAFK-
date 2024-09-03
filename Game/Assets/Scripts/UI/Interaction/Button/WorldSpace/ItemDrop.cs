using MageAFK.Core;
using MageAFK.Items;
using MageAFK.Management;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MageAFK.UI
{
  public class ItemDrop : MonoBehaviour, IPointerClickHandler
  {

    public Image image;

    public ItemIdentification iD;
    public ItemLevel level;
    public int amount;

    public Coroutine despawnRoutine;


    public void OnPointerClick(PointerEventData eventData)
    {
      DeactivateDrop();
      ServiceLocator.Get<InventoryHandler>().AddItem(iD, level, amount);
      ServiceLocator.Get<SiegeStatisticTracker>().ModifiyMetric(Player.PlayerStatisticEnum.ItemsGained, amount);
      ServiceLocator.Get<WorldSpaceUI>().RemoveActiveDrop(this);
    }

    public void OnWaveFinished()
    {
      StopAllCoroutines();
      DeactivateDrop();
      ServiceLocator.Get<InventoryHandler>().AddItem(iD, level, amount);
    }
    public void DeactivateDrop()
    {
      if (despawnRoutine != null) { StopCoroutine(despawnRoutine); }
      gameObject.SetActive(false);
    }

    public void SetItem(ItemIdentification iD, ItemLevel level, int amount)
    {
      this.amount = amount;
      this.iD = iD;
      this.level = level;
    }

    public Coroutine StartDespawnTimer(CanvasGroup group)
    {
      // Assuming ItemDespawnTimer method is available in this class, otherwise, you can pass the method as a delegate.
      return StartCoroutine(ServiceLocator.Get<WorldSpaceUI>().ItemDespawnTimer(group, this));
    }




  }

}