using System;
using MageAFK.Core;
using MageAFK.Management;
using MageAFK.Stats;
using MageAFK.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MageAFK.UI
{
  public class PowerUI : MonoBehaviour, IPagination<ShopElixir>, IOnTabSelected<int>
  {

    [SerializeField] private PowerSlotUI[] slots;
    [SerializeField] private PowerHandler powerHandler;
    [SerializeField] private ButtonUpdateClass[] pageButtons;
    [SerializeField] private PowerPopUpUI popUp;
    [SerializeField] private GameObject indicator;

    private Pagination<ShopElixir> pagination;
    private readonly ShopElixir[] visibleShop = new ShopElixir[3];

    private const int ITEMS_PER_PAGE = 3;

    private void Awake()
    {
      for (int i = 0; i < slots.Length; i++)
      {
        slots[i].SetGroup(this);
      }
    }
    private void OnEnable()
    {
      ServiceLocator.Get<CurrencyHandler>().SubscribeToCurrencyEvent(UpdateSlots, CurrencyType.GoldBars, true);
    }

    private void OnDisable()
    {
      ServiceLocator.Get<CurrencyHandler>().SubscribeToCurrencyEvent(UpdateSlots, CurrencyType.GoldBars, false);

      if (indicator.activeSelf)
        indicator.SetActive(false);
    }

    public void FillShopSlots(ShopElixir[] shop, bool newShop)
    {
      pagination = new Pagination<ShopElixir>(shop, this, ITEMS_PER_PAGE);
      pagination.UpdateDisplay();
      if (newShop) indicator.SetActive(true);
    }

    //Pagination Function
    public void UpdateSlot(ShopElixir elixir, int index)
    {
      visibleShop[index] = elixir;
      slots[index].FillUI(elixir == null ? null : powerHandler.ReturnElixir(elixir.iD), elixir);
      UpdateSlot(ServiceLocator.Get<CurrencyHandler>().GetCurrencyAmount(CurrencyType.GoldBars), index);
    }
    private void UpdateSlots(int gold)
    {
      for (int i = 0; i < visibleShop.Length; i++)
      {
        UpdateSlot(gold, i);
      }
    }

    private void UpdateSlot(int gold, int i)
    {
      var elixir = visibleShop[i];
      if (elixir == null) return;
      slots[i].ToggleBlackMask(gold < elixir.cost && !elixir.isPurchased);
    }

    #region Pagination

    public void AlterPagePressed(bool isNext)
    {
      if (isNext) pagination.NextPage();
      else pagination.PreviousPage();
    }

    public void UpdatePageButtons() => Pagination<ShopElixir>.UpdatePageButtons(pagination, pageButtons);
    public void CustomPaginationBehaviour() { }
    #endregion

    #region Interaction / Popup

    private int chosenIndex = -1;
    public void OnTabSelected(int index)
    {
      if (index >= 0 && index < 3 && visibleShop[index] != null)
      {
        chosenIndex = index;
        var elixir = powerHandler.ReturnElixir(visibleShop[index].iD);

        popUp.FillAndOpen(elixir, visibleShop[index]);
        popUp.OnDecision += OnPurchaseDecision;

      }
    }

    private void OnPurchaseDecision(bool purchase)
    {
      if (purchase)
      {
        var shopElixir = visibleShop[chosenIndex];
        var currencyHandler = ServiceLocator.Get<CurrencyHandler>();
        if (currencyHandler.SubtractCurrency(CurrencyType.GoldBars, shopElixir.cost))
        {
          shopElixir.isPurchased = true;
          slots[chosenIndex].ToggleUpgradedItem(true);
          UpdateSlot(currencyHandler.GetCurrencyAmount(CurrencyType.GoldBars), chosenIndex);
          ServiceLocator.Get<MilestoneHandler>().UpdateMileStone(MilestoneID.PurchaseElixers, 1);
          SaveManager.Save(powerHandler.SaveData(), DataType.ElixirData);
        }
      }
      chosenIndex = -1;
    }

    #endregion

  }
}
