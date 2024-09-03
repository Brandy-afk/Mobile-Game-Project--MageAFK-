using MageAFK.Management;
using UnityEngine;

namespace MageAFK.Items
{
    public class CrownEffect : MonoBehaviour
    {
        private Crown crown;
        private void Awake() => crown = ServiceLocator.Get<GearHandler>().ReturnItem(ItemType.Headgear) as Crown;
        private void ApplyEffect() => crown.ApplyEffect();
        private void Disable() => gameObject.SetActive(false);
    }
}
