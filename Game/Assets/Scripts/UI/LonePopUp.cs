using UnityEngine;
using UnityEngine.UI;

namespace MageAFK.UI
{
  public class LonePopUp : WavePopUp
  {
    protected Button parentButton;

    protected override void OnEnable()
    {
      base.OnEnable();
      if (parentButton == null) parentButton = parent.GetComponent<Button>();
      parentButton.onClick.AddListener(Close);
    }

    protected override void OnDisable()
    {
      base.OnDisable();
      parentButton.onClick.RemoveAllListeners();
    }


  }
}