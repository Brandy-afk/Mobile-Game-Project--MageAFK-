using System.Collections.Generic;
using MageAFK.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MageAFK.Management
{
    public class SceneObjectLoader : SerializedMonoBehaviour
  {
    [SerializeField] private Dictionary<Location, GameObject> tileMaps;
    [SerializeField] private GameObject currentMap;

    public void LoadSiegeObjects(Location location)
    {
      Destroy(currentMap);
      currentMap = Instantiate(tileMaps[location]);
    }

    public void UnloadSiegeObjects()
    {
      Destroy(currentMap);
      currentMap = Instantiate(tileMaps[Location.Town]);
    }

  }
}