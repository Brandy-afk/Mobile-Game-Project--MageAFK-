using MageAFK.Core;
using MageAFK.Management;
using MageAFK.Tools;
using UnityEngine;

namespace MageAFK.Player
{
  public class PlayerRotation : MonoBehaviour
  {
    [SerializeField] private Transform playerSprite;

    Transform bestTarget;
    void Update()
    {
      bestTarget = ServiceLocator.Get<EntityTracker>().ReturnBestTarget(FocusEntity.ClosestTarget, playerSprite)?.Transform;

      if (bestTarget == null)
      {
        return;
      }

      Utility.FlipXSprite(transform.position, bestTarget.position, transform);

    }

  }
}
