
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace MageAFK.Creation
{
  public static class EditorUtility
  {
    public static void CreateFolderIfNeeded(string folderPath)
    {
      if (!Directory.Exists(folderPath))
      {
        Directory.CreateDirectory(folderPath);
        AssetDatabase.Refresh();
      }
    }


    public static GameObject CreateOrReplacePrefab(GameObject gameObject, string localPath)
    {
      localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

      bool prefabExists = AssetDatabase.LoadAssetAtPath<GameObject>(localPath) != null;

      if (prefabExists)
      {
        // If the prefab already exists, replace it.

        UnityEditor.EditorUtility.DisplayDialog("Prefab Created/Updated", "The prefab was successfully created or updated at " + localPath, "OK");
        return PrefabUtility.SaveAsPrefabAsset(gameObject, localPath);
      }
      else
      {
        // Create a new prefab at the path.
        UnityEditor.EditorUtility.DisplayDialog("Prefab Created/Updated", "The prefab was successfully created at " + localPath, "OK");
        return PrefabUtility.SaveAsPrefabAssetAndConnect(gameObject, localPath, InteractionMode.UserAction);
      }

      // Optionally, you may want to destroy the GameObject in the scene after creating the prefab
      // DestroyImmediate(gameObject);
    }

    public static void AttachScript(GameObject obj, MonoScript script)
    {
      if (script != null && obj != null)
      {
        var scriptType = script.GetClass();
        if (scriptType != null && scriptType.IsSubclassOf(typeof(Component)))
        {
          obj.AddComponent(scriptType);
        }
      }
    }

    public static T CreateScriptable<T>(string objectName, string pathing) where T : ScriptableObject
    {
      T scriptable = ScriptableObject.CreateInstance<T>();

      string dataPath = AssetDatabase.GenerateUniqueAssetPath($"{pathing}/{objectName}.asset");
      // Save the scriptable object asset
      AssetDatabase.CreateAsset(scriptable, dataPath);
      AssetDatabase.SaveAssets();

      return scriptable;
    }




  }

}
