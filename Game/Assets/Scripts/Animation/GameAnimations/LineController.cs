using UnityEngine;

namespace MageAFK
{
    public class LineController : MonoBehaviour
    {
        private LineRenderer lineRenderer;

        [SerializeField]
        private Texture[] textures;

        [SerializeField]
        private float fps = 30f;
        private float fpsCounter;
        private int animationStep = 0;
        private void Awake() => lineRenderer = GetComponent<LineRenderer>();

        private void Update()
        {
            fpsCounter += Time.deltaTime;
            if (fpsCounter >= 1f / fps)
            {
                animationStep = (animationStep + 1) % textures.Length;
                lineRenderer.material.SetTexture("_MainTex", textures[animationStep]);
                fpsCounter = 0;
            }
        }
    }
}
