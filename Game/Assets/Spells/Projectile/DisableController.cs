using UnityEngine;
namespace MageAFK.Spells
{
    public class DisableController : MonoBehaviour
    {
        public void Disable() => gameObject.SetActive(false);
    }

}
