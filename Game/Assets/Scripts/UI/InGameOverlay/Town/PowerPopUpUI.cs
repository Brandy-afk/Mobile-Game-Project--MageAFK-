
using MageAFK.Core;
using MageAFK.Management;
using TMPro;
using UnityEngine;

public class PowerPopUpUI : PurchasePopUp
{
  [SerializeField] private TMP_Text val;
  [SerializeField] private GameObject purchasedObject;
  

  public void FillAndOpen(Elixir elixir, ShopElixir shopElixir)
  {
    if (shopElixir == null) return;
    title.text = elixir.title;
    title.color = elixir.titleColor;
    image.sprite = elixir.sprite;
    val.text = elixir.FormatValue(shopElixir.value);

    bool isBuyable = !shopElixir.isPurchased &&
                        ServiceLocator.Get<CurrencyHandler>().ReturnAffordable(CurrencyType.GoldBars, shopElixir.cost);

    desc.text = elixir.CreateDesc(shopElixir.value, shopElixir.cost, isBuyable);

    SetButtonStates(isBuyable);

    cost.text = $"<sprite name=Gold>{shopElixir.cost}";
    purchasedObject.SetActive(shopElixir.isPurchased);

    OpenPanel();
  }
}