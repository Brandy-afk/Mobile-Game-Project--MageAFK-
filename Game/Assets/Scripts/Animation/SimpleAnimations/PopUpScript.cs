
using MageAFK.Animation;
using UnityEngine;

namespace MageAFK
{
    public class PopUpScript : MonoBehaviour
    {
        private float duration = .25f;
        private Vector3 startingScale = new(0f, 0f, 1);
        private Vector3 normalScale = new(1.0f, 1.0f, 1.0f);
        private void OnEnable() => UIAnimations.Instance.PopUpObject(gameObject, startingScale, normalScale, duration);
    }
}
