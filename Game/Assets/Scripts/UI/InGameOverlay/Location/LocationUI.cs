using System.Collections.Generic;
using MageAFK.Animation;
using MageAFK.Core;
using TMPro;
using UnityEngine;
using Sirenix.OdinInspector;
using MageAFK.Management;
using UnityEngine.UI;
using System;

namespace MageAFK.UI
{
  public class LocationUI : SerializedMonoBehaviour
  {

    [SerializeField] private Dictionary<Location, LocationNode> nodes;

    [SerializeField] private GameObject[] travelButtons;
    [SerializeField] private Transform pointer;

    [HideInInspector] public bool choosingLocation = false;


    [System.Serializable]
    public class LocationNode
    {
      public Transform pointerPos;
    }

    [System.Serializable]
    public class LockedNode : LocationNode
    {
      public GameObject[] unlockObjects;
      public GameObject[] lockObjects;
      public Image sprite;
    }

    private void OnEnable()
    {
      if (WaveHandler.WaveState == WaveState.None && pointer.gameObject.activeInHierarchy)
      {
        pointer.gameObject.SetActive(false);
      }
    }



    private void OnDisable() => choosingLocation = false;


    private void OnLocationChanged(Location newLoc)
    {

    }

    public void UnlockLocation(Location location)
    {
      try
      {
        var node = nodes[location] as LockedNode;
        node.sprite.color = Color.white;
        SetObjects(node.lockObjects, false);
        SetObjects(node.unlockObjects, true);
      }
      catch (KeyNotFoundException)
      {
        Debug.Log($"{location} not found!");
      }
      catch (NullReferenceException)
      {
        Debug.Log($"{location} is not a locked location");
      }


    }

    private void SetObjects(IList<GameObject> objects, bool state)
    {
      for (int i = 0; i < objects.Count; i++)
        objects[i].SetActive(state);
    }


    public void OnInfoPressed(int enumIndex)
    {

    }

    public void OnTravelPressed(int enumIndex)
    {

    }

  }


  [System.Serializable]
  public class LocationUIData
  {
    public GameObject[] lockItems;
    public GameObject[] unlockItems;
    public GameObject buttonMask, indicator;
    public TMP_Text silver, xp, scaling, wave;
  }




}