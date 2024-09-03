using MageAFK.Items;
using UnityEngine;

namespace MageAFK.UI
{
    public class ItemGradeButton : TabButton
{
    [SerializeField] private ItemGrade grade;

    public ItemGrade ReturnGrade()
    {
        return grade;
    }
}

}