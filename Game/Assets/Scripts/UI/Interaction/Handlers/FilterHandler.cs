using System;
using MageAFK.Animation;
using MageAFK.Tools;
using TMPro;
using UnityEngine;


namespace MageAFK.UI
{
  public class FilterHandler : MonoBehaviour
  {

    [SerializeField] private TMP_Text typeText, gradeText;
    private ItemTypeFilter newType;
    private ItemGradeFilter newGrade;
    private readonly ItemTypeFilter[] typeArray = (ItemTypeFilter[])Enum.GetValues(typeof(ItemTypeFilter));
    private readonly ItemGradeFilter[] gradeArray = (ItemGradeFilter[])Enum.GetValues(typeof(ItemGradeFilter));
    private IFilterHandler handler;

    public void OpenFilterPanel(IFilterHandler handler, ItemTypeFilter type, ItemGradeFilter grade)
    {
      this.handler = handler;

      newType = type;
      newGrade = grade;
      UpdateTextDisplay(typeText, newType);
      UpdateTextDisplay(gradeText, newGrade);

      ToggelPanelState(true);
    }

    private void ToggelPanelState(bool state)
    {
      if (state)
      {
        transform.parent.gameObject.SetActive(true);
        UIAnimations.Instance.OpenPanel(gameObject);
      }
      else
      {
        UIAnimations.Instance.ClosePanel(gameObject, () => transform.parent.gameObject.SetActive(false));
      }
    }


    public void IterateGradeFilter(bool isLeft)
    {
      var value = isLeft ? -1 : 1;
      int gradeIndex = ((int)newGrade + value + gradeArray.Length) % gradeArray.Length;
      newGrade = gradeArray[gradeIndex];
      UpdateTextDisplay(gradeText, newGrade);
    }

    public void IterateTypeFilter(bool isLeft)
    {
      var value = isLeft ? -1 : 1;
      int typeIndex = ((int)newType + value + typeArray.Length) % typeArray.Length;
      newType = typeArray[typeIndex];
      UpdateTextDisplay(typeText, newType);
    }

    public void UpdateTextDisplay<T>(TMP_Text text, T type) => text.text = StringManipulation.AddSpacesBeforeCapitals(type.ToString());

    public void OnConfirmation(bool isDiscard)
    {
      if (!isDiscard) handler.ApplyFilterChange(newType, newGrade);
      ToggelPanelState(false);
    }

  }

  public interface IFilterHandler
  {
    void ApplyFilterChange(ItemTypeFilter type, ItemGradeFilter grade);
  }

  public enum ItemTypeFilter
  {
    AllTypes = 0,
    Headgear = 1,
    Torso = 2,
    Jewelry = 3,
    Consumable = 4,
    Part = 5,
    Feed = 6,
    Equippable = 7
  }

  public enum ItemGradeFilter
  {
    AllGrades = 0,
    Common = 1,
    Unique = 2,
    Rare = 3,
    Artifact = 4,
    Corrupt = 5
  }

}