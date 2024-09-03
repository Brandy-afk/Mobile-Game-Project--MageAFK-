
using MageAFK.Items;
using UnityEngine;
using UnityEngine.UI;

public class RecipeShopPopUp : PurchasePopUp
{
  [SerializeField] private Image panelImage;


  public bool FillAndOpen(Sprite panelSprite, Recipe recipe, int cost)
  {
    if (recipe == null || panelSprite == null) return false;
    title.text = recipe.ReturnName();
    panelImage.sprite = panelSprite;
    image.sprite = recipe.output.image;
    desc.text = recipe.output.grade.ToString().ToUpper();
    this.cost.text = $"<sprite name=Gold>{cost}";

    SetButtonStates(true);
    OpenPanel();
    return true;
  }

}