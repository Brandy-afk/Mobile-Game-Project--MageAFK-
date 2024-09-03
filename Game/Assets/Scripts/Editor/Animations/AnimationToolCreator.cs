using System.Collections;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace MageAFK.Creation
{
  public class AnimationToolCreator : OdinEditorWindow
  {
    [MenuItem("Tools/Manual Creator")]
    private static void OpenWindow()
    {
      GetWindow<AnimationToolCreator>().Show();
    }


    [Sirenix.OdinInspector.FilePath(Extensions = "png", RequireExistingPath = true), LabelText("Sprite Sheet File, source"), BoxGroup("Pathing")]
    public string sheetPath = "Copy & Paste Here / Leave alone if you do not want";

    [BoxGroup("Animation")]
    public Dictionary<string, SpriteAnimation> animations;



    [LabelText("Animation Pathing, destination"), BoxGroup("Pathing")]
    public string animationPath = "Copy & Paste Here / Leave alone if you do not want";

    [LabelText("Prefab path, destination"), BoxGroup("Pathing")]
    public string prefabPath = "Copy & Paste Here";


    [BoxGroup("Info")]
    public GameObject prefab = null;


    [BoxGroup("Info")]
    public EditorToolType type;
    [BoxGroup("Info")]
    public string objectName = "";

    [BoxGroup("Info")]
    public bool createUIController = false;

    [BoxGroup("Info")]
    public bool onlyCreateUIController = false;

    [BoxGroup("Animation"), Header("UI Animations, best if only 1"), ShowIf("ReturnIfUiController")]
    public Dictionary<string, SpriteAnimation> uiAnimations;

    [BoxGroup("Animation"), Header("Sprites"), ShowIf("ReturnIfNotNullOrEmpty")]
    public List<Sprite> sprites;

    private Dictionary<EditorToolType, string> resource = new Dictionary<EditorToolType, string>()
    {
        {EditorToolType.Enemy, "EnemyTemplate"},
        {EditorToolType.EnemyProjectile, "ProjectileTemplate"},
        {EditorToolType.SpellProjectile, "SpellTemplate"}
    };


    public bool ReturnIfUiController()
    {
      return createUIController;
    }

    public bool ReturnIfNotNullOrEmpty()
    {
      return sprites != null && sprites.Count > 0;
    }

    [Button("Unpackage Sprites")]
    public void UnPackSprites()
    {
      sprites = AnimationCreator.UnPackSprites(sheetPath);
    }

    [Button("Create Objects")]
    public void CreateObjects()
    {
      if (objectName == "")
      {
        return;
      }

      if (prefabPath != "Copy & Paste Here" && prefab == null)
      {
        GameObject template = Resources.Load<GameObject>(resource[type]);

        if (template == null)
        {
          Debug.LogError("SpellTemplate prefab not found in Resources!");
          return;
        }

        GameObject templateInstance = Instantiate(template);
        templateInstance.name = objectName;

        prefab = EditorUtility.CreateOrReplacePrefab(templateInstance, Path.Combine(prefabPath, $"{templateInstance.name}.prefab"));
      }

      string path = null;
      if (!onlyCreateUIController)
      {
        (string folderPath, AnimatorController regularController) = AnimationCreator.CreateControllerAtPath(animationPath, objectName);
        path = folderPath;
        AnimationCreator.CreateAnimationsFromSprites(folderPath, regularController, sprites, animations, prefab);

      }


      if (createUIController)
      {

        (string newPath, AnimatorController uiController) = AnimationCreator.CreateControllerAtPath(path ?? animationPath, $"UI{objectName}");

        AnimationCreator.CreateAnimationsFromSprites(newPath, uiController, sprites, uiAnimations, prefab, true);
      }

      SetObjectNull();
    }

    public void SetObjectNull()
    {


    }
  }

  public enum EditorToolType
  {
    Enemy,
    EnemyProjectile,
    SpellProjectile
  }
}
