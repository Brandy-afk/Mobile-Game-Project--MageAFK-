using MageAFK.Core;
using MageAFK.Management;
using UnityEngine;

namespace MageAFK.Items
{
  [CreateAssetMenu(fileName = "GoldBarRecipe", menuName = "Items/Recipes/CustomRecipes/GoldBar")]
  public class GoldBar : Recipe
  {

    public override void OnRecipeCrafted(Order order)
    {
      ServiceLocator.Get<CurrencyHandler>().AddCurrency(CurrencyType.GoldBars, (order.multiplier * order.recipe.AmountToCraft));
    }

  }
}
