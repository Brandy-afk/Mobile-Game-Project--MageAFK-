using MageAFK.Tutorial;
using UnityEngine;

[CreateAssetMenu(fileName = "Default", menuName = "Tip/Default", order = 0)]
public class DefaultTip : Tip
{
  [SerializeField, TextArea(2, 2)] private string title;
  [SerializeField, TextArea(4, 8)] private string desc;
  public override string ReturnDesc() => desc;
  public override string ReturnTitle() => title;
}