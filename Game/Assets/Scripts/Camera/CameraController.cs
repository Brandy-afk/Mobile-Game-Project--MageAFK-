using System;
using MageAFK.Animation;
using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace MageAFK.Cam
{
  public class CameraController : SerializedMonoBehaviour
  {


    [SerializeField] private Dictionary<CameraLoc, CamLocationInfo> camDict;


    [Header("References")]
    [SerializeField] private CameraBounds cameraBounds;
    [SerializeField] private CameraAnimations cameraAnimations;

    [Header("Objects")]

    [SerializeField] private float camSpeed;
    [SerializeField] private Camera mainCamera;

    [SerializeField] private float DesiredWidth = 9.0f;  // the width of the game area you want displayed
    [SerializeField] private float DesiredHeight = 16.0f;  // the height of the game area you want displayed

    [Header("Testing")]
    [SerializeField] private CameraLoc moveToLocation;
    [SerializeField] private Transform playerLoc;
    [SerializeField] private TestingGame testingGame;



    [Button("Set Cam")]
    public void SetCam() => SwapToCam(moveToLocation);

    [Button("Testing")]
    public void Test() => SetUpCamera();


    private CameraLoc currentCam;

    private void Awake()
    {
      if (testingGame.isTesting) return;
      SetUpCamera();
      SwapToCam(CameraLoc.Start);
      currentCam = CameraLoc.Start;
    }

    private void SetUpCamera()
    {
      float targetaspect = DesiredWidth / DesiredHeight;
      float windowaspect = Screen.width / (float)Screen.height;
      float scaleheight = windowaspect / targetaspect;
      if (scaleheight < 1.0f)
      {
        camDict[CameraLoc.Game].zoom = (DesiredHeight / 2 / scaleheight) - .5f;
      }
      else
      {
        camDict[CameraLoc.Game].zoom = (DesiredHeight / 2) - .5f;
      }
    }

    public void TransitionToCam(CameraLoc targetCam, Action callback = null)
    {
      StartCoroutine(cameraAnimations.TransitionCameraView(mainCamera, camDict[currentCam], camDict[targetCam], camSpeed, () => { cameraBounds.SetupCollider(); if (callback != null) { callback(); }; }));
      currentCam = targetCam;
    }

    public void SwapToCam(CameraLoc targetCam)
    {
      cameraAnimations.SetCameraView(mainCamera, camDict[targetCam]);
      playerLoc.position = camDict[targetCam].playerLoc;
      currentCam = targetCam;
      cameraBounds.SetupCollider();
    }

  }

  public enum CameraLoc
  {
    Start,
    Game,
    TownChoices,
    Blacksmith,
    Farm1,
    Alchemist

  }


  [System.Serializable]
  public class CamLocationInfo
  {
    public float zoom;
    public Vector3 camLoc;
    public Vector3 playerLoc;

  }

}