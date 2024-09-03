using UnityEngine;

namespace MageAFK.UI
{
    public class LocationButton : TabButton
  {

    [SerializeField] private bool isUnlocked;


    

    public void Unlock()
    {
      isUnlocked = true;
    }

    public bool ReturnUnlockState()
    {
      return isUnlocked;
    }

  }
}