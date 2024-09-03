using UnityEngine;

namespace MageAFK.Cam
{
    [RequireComponent(typeof(Camera))]
    public class SafeZoneCamera : MonoBehaviour
    {
        public Transform safeZone;  // Set this to the transform of your safe zone in the inspector

        void Start()
        {
            // Adjust the camera's position to be in the center of the safe zone
            transform.position = new Vector3(safeZone.position.x, safeZone.position.y, transform.position.z);
        }

        private void Update() 
        {
            // Calculate the aspect ratio of the screen
            float screenRatio = (float)Screen.width / (float)Screen.height;
            float targetRatio = safeZone.localScale.x / safeZone.localScale.y;

            // If the screen is wider than the target, adjust height to fit the width
            if (screenRatio >= targetRatio)
            {
                Camera.main.orthographicSize = safeZone.localScale.x / 4;
            }
            // If the screen is narrower than the target, adjust height to fit the height
            else
            {
                Camera.main.orthographicSize = safeZone.localScale.y / 2;
            }
        }
    }
}