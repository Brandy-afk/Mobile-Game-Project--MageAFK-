
using System.Collections.Generic;
using UnityEngine;
using MageAFK.Management;
using MageAFK.Items;
using UnityEngine.UI;
using TMPro;

namespace MageAFK.UI
{
    public class ScavengerUI : MonoBehaviour, IPagination<(ItemIdentification, ItemLevel)>
  {

    [Header("Objects")]
    [SerializeField] private List<ScavengeItemSlot> scavengeSlots;
    [SerializeField] private ButtonUpdateClass[] alterationButtons;
    [SerializeField] private TMP_Text page;
    [SerializeField] private int itemsPerPage = 12;

    public Dictionary<(ItemIdentification, ItemLevel), ScavengeItemSlot> itemSlotsDict = new();
    private Pagination<(ItemIdentification, ItemLevel)> currentPagination;

    private ScavengerHandler scavengerHandler;

    private void Awake()
    {
      scavengerHandler = ServiceLocator.Get<ScavengerHandler>();
      scavengerHandler.InputScavengerUI(this);
    }


    private void OnEnable()
    {
      var scavengerHandler = ServiceLocator.Get<ScavengerHandler>();

      scavengerHandler.UpdateUI();
    }


    public void UpdateItemToUI((ItemIdentification, ItemLevel) key, int quantity)
    {
      if (itemSlotsDict.ContainsKey(key))
        itemSlotsDict[key].amount.text = quantity.ToString();
      else
        Debug.Log("Key not found");
    }

    public void UpdateSlot((ItemIdentification, ItemLevel) key, int index)
    {
      int amount = scavengerHandler.ReturnAmount(key);
      var state = amount != 0;
      var uISlot = scavengeSlots[index];

      ItemData itemData = null;
      if (state)
        itemData = ServiceLocator.Get<IItemGetter>().ReturnItemData(key.Item1);

      uISlot.item.gameObject.SetActive(state);
      uISlot.amount.text = state ? amount.ToString() : "";
      // slot.grade.sprite = state ? itemDataBase.GetGradeInformation(itemData.grade).blueImage : itemDataBase.GetGradeInformation(ItemGrade.None).blueImage;

      if (state)
      {
        uISlot.item.sprite = itemData.image;
        itemSlotsDict[key] = uISlot;
      }
    }

    public bool ReturnIfActiveSlot((ItemIdentification, ItemLevel) key) => itemSlotsDict.ContainsKey(key);

    public void FilterSlots(List<(ItemIdentification, ItemLevel)> items)
    {
      if (!gameObject.activeInHierarchy) { return; }
      var page = currentPagination?.ReturnCurrentPage() ?? 1;
      Pagination<(ItemIdentification, ItemLevel)> pagination = new(items, this, itemsPerPage, page);
      currentPagination = pagination;
      currentPagination.UpdateDisplay();
    }

    public void UpdatePageButtons()
    {
      Pagination<(ItemIdentification, ItemLevel)>.UpdatePageButtons(currentPagination, alterationButtons);
    }

    public void CustomPaginationBehaviour()
    {
      itemSlotsDict.Clear();
    }

    public void AlterPagePressed(bool isNext)
    {
      if (isNext) { currentPagination.NextPage(); }
      else { currentPagination.PreviousPage(); }
      UpdatePageText(currentPagination.ReturnCurrentPage(), currentPagination.ReturnTotalPages());
    }

    private void UpdatePageText(int current, int max) => page.text = $"{current}/{max}";

    public void ClearObjects()
    {
      for (int i = 0; i < scavengeSlots.Count; i++)
      {
        UpdateSlot(default, i);
      }

      itemSlotsDict.Clear();
      currentPagination = null;

      UpdatePageText(1, 1);
      UpdatePageButtons();
    }


  }

  [System.Serializable]
  public class ScavengeItemSlot
  {
    public Image item;
    public TMP_Text amount;
    public Image grade;

  }

  [System.Serializable]
  public class ButtonUpdateClass
  {
    public GameObject black;
    public Button button;


  }
}
