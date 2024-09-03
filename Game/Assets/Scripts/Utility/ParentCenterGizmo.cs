using UnityEngine;

public class ParentCenterGizmo : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        if (transform.parent != null)
        {
            Gizmos.color = Color.white;
            Vector3 parentCenter = transform.parent.position;
            Gizmos.DrawSphere(parentCenter, 0.1f);
        }
    }
}