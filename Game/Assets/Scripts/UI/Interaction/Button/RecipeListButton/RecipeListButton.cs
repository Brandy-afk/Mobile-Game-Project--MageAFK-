using MageAFK.Items;
using MageAFK.Management;
using MageAFK.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MageAFK.UI
{
    public class RecipeListButton : MonoBehaviour, IPointerClickHandler
  {

    [SerializeField] private TMP_Text itemName;
    [SerializeField] private Image itemImage;
    [SerializeField] private Image itemPanelImage;
    [SerializeField] private GameObject indicatorObject;

    private static RecipeUIController controller;
    private Recipe recipe;

    public static void InputScripts(RecipeUIController control) => controller = control;

    public void OnPointerClick(PointerEventData eventData)
    {
      controller.OpenRecipePanel(recipe);
      ToggleIndicator(false);
    }

    public void SetRecipe(Recipe recipe)
    {
      this.recipe = recipe;
      SetUpUI();
    }

    private void SetUpUI()
    {
      itemName.text = recipe.ReturnName();
      itemImage.sprite = recipe.output.image;
      IItemGradeUIProvider provider = ServiceLocator.Get<IItemGradeUIProvider>();
      itemName.color = Utility.AlterColor(provider.GetColor(recipe.output.grade));
      itemPanelImage.sprite = provider.GetSlotSprite(recipe.output.grade,
                                                     InventorySpriteType.Filled,
                                                     ItemLevel.None);
    }

    public void ToggleIndicator(bool state)
    {
      if (indicatorObject.activeSelf == state) { return; }
      indicatorObject.SetActive(state);
    }


  }

}