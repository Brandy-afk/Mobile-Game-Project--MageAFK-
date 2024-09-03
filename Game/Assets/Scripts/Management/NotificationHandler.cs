using UnityEngine;

namespace MageAFK.Tools
{
    public class NotificationHandler : MonoBehaviour
  {
    private INotification _notificationService;

    private string androidChannel;

    private void Awake()
    {
      if (Application.platform == RuntimePlatform.Android)
      {
        _notificationService = new AndroidNotification();
      }
      else if (Application.platform == RuntimePlatform.IPhonePlayer)
      {
        _notificationService = new iOSNotification();
      }
      else
      {
        Debug.Log("Notifications not supported on this platform");
        return;
      }
    }




    public void SendNotification(string title, string message, int delaySeconds)
    {
      _notificationService?.SendNotification(title, message, delaySeconds);
    }
  }

  public interface INotification
  {
    void SendNotification(string title, string message, int delaySeconds);
  }

  public class AndroidNotification : INotification
  {
    public void SendNotification(string title, string message, int delaySeconds)
    {
      // Insert Android specific notification code here
    }
  }

  public class iOSNotification : INotification
  {
    public void SendNotification(string title, string message, int delaySeconds)
    {
      // Insert iOS specific notification code here
    }
  }


}