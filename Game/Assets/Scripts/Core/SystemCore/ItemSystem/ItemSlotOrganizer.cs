
using System;
using System.Collections.Generic;
using System.Linq;
using MageAFK.Management;
using MageAFK.UI;

namespace MageAFK.Items
{
    public static class ItemSlotOrganizer
  {
    public static List<(ItemIdentification, ItemLevel)> OrganizeItemSlots(List<ItemSlot> slots, (ItemTypeFilter typeFilter, ItemGradeFilter gradeFilter) filters) =>
     OrderKeys(slots.Select(slot => (slot.item.iD, slot.item.ReturnLevel())).ToList(), filters);

    public static List<(ItemIdentification, ItemLevel)> OrderKeys(List<(ItemIdentification, ItemLevel)> slots, (ItemTypeFilter typeFilter, ItemGradeFilter gradeFilter) filters)
    {
      var itemGetter = ServiceLocator.Get<IItemGetter>();
      return slots
        .Where(slot =>
            ReturnIfFilterMatch(slot.Item1, filters))
        .OrderBy(slot => itemGetter.ReturnItemData(slot.Item1).grade)
        .ThenBy(slot => itemGetter.ReturnItemData(slot.Item1).mainType)
        .ThenBy(slot => slot.Item2)
        .Select(slot => (slot.Item1, slot.Item2))
        .Reverse()
        .ToList();
    }

    public static bool ReturnIfFilterMatch(ItemIdentification iD, (ItemTypeFilter, ItemGradeFilter) filter)
    {

      var itemData = ServiceLocator.Get<IItemGetter>().ReturnItemData(iD);


      bool matchesType = filter.Item1 == ItemTypeFilter.AllTypes ||
                         (Enum.TryParse(filter.Item1.ToString(), out ItemType type) &&
                          itemData.ReturnIfType(type));

      bool matchesGrade = filter.Item2 == ItemGradeFilter.AllGrades ||
                          Enum.TryParse(filter.Item2.ToString(), out ItemGrade grade) &&
                          itemData.grade == grade;

      return matchesType && matchesGrade;
    }
  }
}
