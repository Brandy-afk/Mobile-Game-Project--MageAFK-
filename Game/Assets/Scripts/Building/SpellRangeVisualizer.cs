using UnityEngine;

namespace MageAFK.Spells
{
    [RequireComponent(typeof(LineRenderer)), RequireComponent(typeof(CircleCollider2D))]
    public class SpellRangeVisualizer : MonoBehaviour
    {

        private LineRenderer lineRenderer;
        private CircleCollider2D circleCollider2D;
        // [SerializeField] private float circleScaleVariance = .25f;
        private IRangeVisualizer script;
        // public GameObject circleFill;


        // public float range;

        // [Button("Test")]
        // public void Testing()
        // {
        //     SetUpVisualizer(range);
        // }

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
            circleCollider2D = GetComponent<CircleCollider2D>();
        }

        private void OnDisable() => script = null;


        public void SetUpVisualizer(float range, IRangeVisualizer script = null, int segments = 20)
        {
            this.script = script;

            circleCollider2D.enabled = script != null;

            lineRenderer.positionCount = segments + 1;
            lineRenderer.useWorldSpace = false;

            if (circleCollider2D.radius != range)
            {
                circleCollider2D.radius = range;

                float deltaTheta = 2f * Mathf.PI / segments;
                float theta = 0f;

                for (int i = 0; i < segments + 1; i++)
                {
                    float x = range * Mathf.Cos(theta);
                    float y = range * Mathf.Sin(theta);
                    Vector3 pos = new(x, y, 0);
                    lineRenderer.SetPosition(i, pos);
                    // GetComponent<LineRenderer>().SetPosition(i, pos);
                    theta += deltaTheta;
                }
            }
        }

        // private void UpdateCircleFill(float radius)
        // {
        //     var baseRadius = 0.5f;
        //     float scaleFactor = (radius / baseRadius) - circleScaleVariance; // Calculate the scale factor based on the original sprite size
        //     circleFill.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f); // Apply the scale
        // }

        private void OnTriggerEnter2D(Collider2D other)
        {
            script?.OnEnter(other);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            script?.OnLeave(other);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            script?.OnStay(other);
        }
    }

    public interface IRangeVisualizer
    {
        public void OnEnter(Collider2D other);
        public void OnLeave(Collider2D other);
        public void OnStay(Collider2D other);
    }
}
