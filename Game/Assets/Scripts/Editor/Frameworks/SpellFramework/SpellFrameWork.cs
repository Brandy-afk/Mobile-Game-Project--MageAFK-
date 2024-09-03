
using System;
using System.Collections.Generic;
using System.IO;
using MageAFK.Spells;
using MageAFK.UI;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace MageAFK.Creation
{
  public class SpellFrameWork : OdinEditorWindow
  {
    [MenuItem("Tools/Spell Creator")]
    private static void OpenWindow()
    {
      GetWindow<SpellFrameWork>().Show();
    }

    #region Pathings

    private Dictionary<SpellType, string> anim = new Dictionary<SpellType, string>()
    {
      {SpellType.Spell, "Assets/Spells/Animators/Spell"},
      {SpellType.Passive, "Assets/Spells/Animators/Passive"},
      {SpellType.Ultimate, "Assets/Spells/Animators/Ultimate"}
    };

    private Dictionary<SpellType, string> script = new Dictionary<SpellType, string>()
    {
      {SpellType.Spell, "Assets/Spells/Spell/Spell"},
      {SpellType.Passive, "Assets/Spells/Spell/Passive"},
      {SpellType.Ultimate, "Assets/Spells/Spell/Ultimate"}
    };

    private Dictionary<SpellType, string> projectile = new Dictionary<SpellType, string>()
    {
      {SpellType.Spell, "Assets/Spells/Projectile/Spell"},
      {SpellType.Passive, "Assets/Spells/Projectile/Passive"},
      {SpellType.Ultimate, "Assets/Spells/Projectile/Ultimate"}
    };

    private Dictionary<SpellType, string> scriptable = new Dictionary<SpellType, string>()
    {
      {SpellType.Spell, "Assets/Spells/Scriptables/Spell"},
      {SpellType.Passive, "Assets/Spells/Scriptables/Passive"},
      {SpellType.Ultimate, "Assets/Spells/Scriptables/Ultimate"}
    };
    private Dictionary<SpellType, string> sprite = new Dictionary<SpellType, string>()
    {

      {SpellType.Spell, "Assets/Spells/Sprites/Spell"},
      {SpellType.Passive, "Assets/Spells/Sprites/Passive"},
      {SpellType.Ultimate, "Assets/Spells/Sprites/Ultimate"}
    };

    private Dictionary<SpellType, string> prefabs = new Dictionary<SpellType, string>()
    {
      {SpellType.Spell, "Assets/Spells/Prefabs/Spell"},
      {SpellType.Passive, "Assets/Spells/Prefabs/Passive"},
      {SpellType.Ultimate, "Assets/Spells/Prefabs/Ultimate"}
    };




    #endregion

    #region  Helpers

    public bool PrefabNull()
    {
      return prefab == null;
    }

    #endregion

    [Sirenix.OdinInspector.FilePath(Extensions = "png", RequireExistingPath = true), LabelText("Sprite Sheet File"), BoxGroup("Animation")]
    public string sheetFolderPath = "Copy & Paste Here";

    [BoxGroup("Animation")]
    public Dictionary<string, SpriteAnimation> animations = new Dictionary<string, SpriteAnimation>
    {
      {"Active", new SpriteAnimation()}
    };


    [BoxGroup("Spell Information")]
    public SpellIdentification spellName = SpellIdentification.None;

    [BoxGroup("Spell Information")]
    public SpellType type;

    [BoxGroup("Spell Information")]
    public Sprite icon;

    [BoxGroup("Spell Information")]
    public bool createUIController = false;

    [BoxGroup("BuildInfo"), ReadOnly]
    public GameObject prefab = null;





    [BoxGroup("BuildInfo"), ReadOnly]
    public RuntimeAnimatorController controller;

    [Button("Create Spell"), ShowIf("PrefabNull")]
    public void CreateSpell()
    {



      //ERROR CHECK
      if (spellName == SpellIdentification.None || sheetFolderPath == "Copy & Paste Here")
      {
        Debug.Log($"Key already in list or error : {spellName}");
        return;
      }

      #region ScriptCreation
      string projectilePathing = projectile[type];
      string spellPathing = script[type];

      CreateScriptTemplates.CreateScript(spellName.ToString(), spellPathing, TemplateTypes.Spell);
      CreateScriptTemplates.CreateScript($"{spellName}Projectile", projectilePathing, TemplateTypes.SpellProjectile);
      AssetDatabase.Refresh(); // Refresh the AssetDatabase to show the new script in Unity Editor

      #endregion

      #region Prefabs
      GameObject projectileTemplate = Resources.Load<GameObject>("SpellTemplate");

      if (projectileTemplate == null)
      {
        Debug.LogError("SpellTemplate prefab not found in Resources!");
        return;
      }

      GameObject projectileInstance = Instantiate(projectileTemplate);
      projectileInstance.name = $"{spellName}";

      string prefabPath = prefabs[type];

      prefab = EditorUtility.CreateOrReplacePrefab(projectileInstance, Path.Combine(prefabPath, $"{projectileInstance.name}.prefab"));
      #endregion

      #region Sprites
      // Base path where the asset will be moved
      string basePath = sprite[type];

      string assetFileName = System.IO.Path.GetFileName(sheetFolderPath);

      // Ensure there's a trailing slash in the base path
      if (!basePath.EndsWith("/"))
      {
        basePath += "/";
      }

      // Construct the new path with the desired new name
      string newAssetPath = basePath + assetFileName;


      // Check if the sprite sheet is already at the target location
      if (sheetFolderPath.Equals(newAssetPath, StringComparison.OrdinalIgnoreCase))
      {
        Debug.LogWarning("The sprite sheet is already at the intended location.");
      }
      else
      {
        // Check if a file already exists at the target location
        if (AssetDatabase.LoadAssetAtPath<Texture2D>(newAssetPath) != null)
        {
          Debug.LogError($"An asset already exists at the target path: {newAssetPath}");
        }
        else
        {
          // Attempt to move the asset
          string errorMsg = AssetDatabase.MoveAsset(sheetFolderPath, newAssetPath);
          if (!string.IsNullOrEmpty(errorMsg))
          {
            Debug.LogError($"Failed to move asset: {errorMsg}");
          }
          else
          {
            Debug.Log("Asset moved successfully.");
          }
        }
      }

      #endregion

      #region Animation
      string animationPath = anim[type];

      (string folderPath, AnimatorController regularController) = AnimationCreator.CreateControllerAtPath(animationPath, spellName.ToString());

      List<Sprite> sprites = AnimationCreator.UnPackSprites(newAssetPath);

      AnimationCreator.CreateAnimationsFromSprites(folderPath, regularController, sprites, animations, prefab);

      if (createUIController)
      {
        (string newPath, AnimatorController uiController) = AnimationCreator.CreateControllerAtPath(folderPath, $"UI{spellName.ToString()}");

        AnimationCreator.CreateAnimationsFromSprites(newPath, uiController, sprites, animations, prefab, true);
        this.controller = uiController as RuntimeAnimatorController;
      }
      else
      {
        this.controller = regularController as RuntimeAnimatorController;
      }

      prefab.GetComponent<Animator>().runtimeAnimatorController = regularController as RuntimeAnimatorController;
      #endregion

      UnityEditor.EditorUtility.DisplayDialog("Spell : Step 1 complete", "Prefab, animations, scripts, sprite process complete, next -> Step 2", "OK");
      AssetDatabase.SaveAssets();
    }




    [Title("After compilation of new scripts, change scriptable type in code"), HideIf("PrefabNull")]
    [BoxGroup("Step 2"), Button("Finish Creation"), HideIf("PrefabNull")]
    public void CreateScriptable()
    {
      string pathing = scriptable[type];
      //Change spell type CreateScriptable<#HERE#>
      Spell spellScriptable = EditorUtility.CreateScriptable<Terrorcut>(spellName.ToString(), pathing);

      spellScriptable.iD = spellName;
      spellScriptable.type = type;
      spellScriptable.prefab = prefab;
      spellScriptable.controller = controller;
      spellScriptable.image = icon;

      // Mark the spellScriptable as dirty to ensure changes are saved
      UnityEditor.EditorUtility.SetDirty(spellScriptable);

      // Change spell PROJECTILE <#HERE#>
      prefab.AddComponent<TerrorcutProjectile>();

      // Additional changes to prefab should also be marked as dirty
      UnityEditor.EditorUtility.SetDirty(prefab);

      AssetDatabase.Refresh(); // Refresh the AssetDatabase to show the new script in Unity Editor
      AssetDatabase.SaveAssets();
      UnityEditor.EditorUtility.FocusProjectWindow();
      Selection.activeObject = spellScriptable;

      SetObjectsNull();
    }

    [BoxGroup("Error"), Button("RESET")]
    public void SetObjectsNull()
    {
      spellName = SpellIdentification.None;
      prefab = null;
      sheetFolderPath = "Copy & Paste Here";
      this.controller = null;
      animations = new Dictionary<string, SpriteAnimation>
      {
      {"Active", new SpriteAnimation()}
      };

      icon = null;
      createUIController = false;
    }

  }
}
