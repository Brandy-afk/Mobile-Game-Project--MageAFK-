using MageAFK.AI;
using UnityEngine;

namespace MageAFK.Cam
{
    public class CameraBounds : MonoBehaviour
  {
    private BoxCollider2D boxCollider;


    [SerializeField] private Camera mainCam;



    public void SetupCollider()
    {
      // Get or add the box collider component
      boxCollider = GetComponent<BoxCollider2D>();
      if (boxCollider == null)
      {
        boxCollider = gameObject.AddComponent<BoxCollider2D>();
      }

      // Calculate the world space dimensions of the camera's view

      float height = 2f * mainCam.orthographicSize;
      float width = height * mainCam.aspect;

      // Transform the world space size to local space
      Vector3 localScale = transform.localScale;
      float localWidth = width / localScale.x;
      float localHeight = height / localScale.y;

      // Set the size of the box collider
      boxCollider.size = new Vector2(localWidth, localHeight);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
      if (other.CompareTag("Enemy") && other.TryGetComponent(out NPEntity nPEntity))
      {
        nPEntity.OnEnteringMap();
      }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
      if (other.CompareTag("Enemy") && other.TryGetComponent(out NPEntity nPEntity))
      {
        nPEntity.OnLeavingMap();
      }
      else
      {
        other.gameObject.SetActive(false);
      }
    }
  }

}