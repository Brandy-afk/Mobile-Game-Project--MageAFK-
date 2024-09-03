using System;
using System.Collections;
using MageAFK.Cam;
using UnityEngine;


namespace MageAFK.Animation
{
  public class CameraAnimations : MonoBehaviour
  {

    public IEnumerator TransitionCameraView(Camera cam, CamLocationInfo startCamInfo, CamLocationInfo endCamInfo, float duration, Action callback = null)
    {
      float elapsed = 0f;

      Vector3 startPosition = startCamInfo.camLoc;
      Vector3 endPosition = endCamInfo.camLoc;
      float startSize = startCamInfo.zoom;
      float endSize = endCamInfo.zoom;

      while (elapsed < duration)
      {
        float t = elapsed / duration;
        cam.orthographicSize = Mathf.Lerp(startSize, endSize, t);
        cam.transform.position = Vector3.Lerp(startPosition, endPosition, t);
        yield return null;
        elapsed += Time.deltaTime;
      }

      cam.orthographicSize = endSize;
      cam.transform.position = endPosition;

      if (callback != null) { callback(); }
    }

    public void SetCameraView(Camera cam, CamLocationInfo camInfo)
    {
      cam.transform.position = camInfo.camLoc;
      cam.orthographicSize = camInfo.zoom;
    }



  }


}