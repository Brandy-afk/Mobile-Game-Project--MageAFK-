using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using UnityEditor.Animations;
using System.IO;
using MageAFK.AI;
using System.Collections.Generic;
using MageAFK.Core;


namespace MageAFK.Creation
{
  public class EntityFrameWork : OdinEditorWindow
  {
    [MenuItem("Tools/Odin Enemy Creator")]
    private static void OpenWindow()
    {
      GetWindow<EntityFrameWork>().Show();
    }

    #region Animation
    [Sirenix.OdinInspector.FilePath(Extensions = "png", RequireExistingPath = true), LabelText("Sprite Sheet File"), BoxGroup("Animation")]
    public string sheetFolderPath = "Copy & Paste Here";

    [BoxGroup("Animation"), Header("Int value stands for starting index")]
    public Dictionary<string, SpriteAnimation> rows = new Dictionary<string, SpriteAnimation>
  {
    {"Idle", new SpriteAnimation(0, 6, true, true)},
    {"Run", new SpriteAnimation(6, 6, false, true)},
    {"Attack", new SpriteAnimation(18, 6, false, false)},
    {"Die", new SpriteAnimation(12, 6, false, false)}
  };





    [Sirenix.OdinInspector.FilePath(Extensions = "png", RequireExistingPath = true), LabelText("Projectile Sprite Sheet"), BoxGroup("Animation"), ShowIf("isRanged"), ShowIf("isAnimProjectile")]
    public string projectileSheet = "Copy & Paste Here";
    [BoxGroup("Animation"), ShowIf("isRanged"), ShowIf("isAnimProjectile")]
    public Dictionary<string, SpriteAnimation> projRows = new Dictionary<string, SpriteAnimation>
  {
    {"Idle", new SpriteAnimation(0, 6, true, true)},
    {"Hit", new SpriteAnimation(6, 6, false, false)}
  };

    #region Instructions
    [Title("Enemy Framework Instructions", TitleAlignment = TitleAlignments.Centered)]
    [InfoBox("Please follow the instructions below to setup your enemy framework properly.", InfoMessageType.Info)]
    [FoldoutGroup("Step by Step Instructions")]
    [LabelText("Step 1")]
    [TextArea(3, 5), ReadOnly]
    public string Step1 = "Ensure sprite sheet path is valid, and ensure that rows are properly defined.";

    [FoldoutGroup("Step by Step Instructions")]
    [LabelText("Step 2")]
    [TextArea(3, 5), ReadOnly]
    public string Step2 = "Create enemy enum and define mob attributes below";

    [FoldoutGroup("Step by Step Instructions")]
    [LabelText("Step 3")]
    [TextArea(3, 5), ReadOnly]
    public string Step3 = "Upon mob creation, set up collider, ui placements, and custom script functionality (if applicable)";

    [FoldoutGroup("Step by Step Instructions")]
    [LabelText("Step 4")]
    [TextArea(3, 5), ReadOnly]
    public string Step4 = "Adjust the AI data scriptable, define animation events.";
    #endregion


    [ShowIf("isMagic")]
    public MonoScript customScript;

    [ShowIf("isMagic"), Header("Create magical projectile"), BoxGroup("Magic")]
    public bool createProjectile = false;

    [ShowIf("isRanged"), Header("Does projectile have an animation"), BoxGroup("Ranged")]
    public bool createAnimation = true;

    [ShowIf("isRanged"), Header("If no animation, give a sprite"), BoxGroup("Ranged")]
    public Sprite projectileSprite = null;

    [ShowIf("isRanged"), Header("Name the projectile 'Something'/projectile.prefab"), BoxGroup("Ranged")]
    public string projectileName;

    [ShowIf("isRanged"), Header("If projectile prefab already exists"), BoxGroup("Ranged")]
    public GameObject preProj;

    #endregion

    #region pathing 
    private string enemyPrefabFolderPath = "Assets/Enemies/Prefabs";
    private string enemyDataFolderPath = "Assets/Enemies/Scriptables";
    private string enemyAnimFolderPath = "Assets/Enemies/Animations";
    private string enemyProjectileFolderPath = "Assets/Enemies/Projectiles";

    private string enemyProjAnimFolderPath = "Assets/Enemies/Projectiles/Animations";
    // private string entityPrefabFolderPath = "Assets/Entities/Prefabs";
    // private string entityDataFolderPath = "Assets/Entities/Scriptables";
    // private string entityAnimFolderPath = "Assets/Entities/Animations";
    #endregion


    [LabelText("Entity"), InlineButton("CreateEnemy", "Create")]
    public Mob mob;

    #region Helpers

    private bool isRanged() => mob?.type == EnemyType.Ranged || mob?.type == EnemyType.Magic;
    private bool isMagic() => mob?.type == EnemyType.Magic;
    private bool isAnimProjectile() => createAnimation;

    #endregion

    private void CreateEnemy()
    {
      // Ensure name is not empty
      if (mob.iD == EntityIdentification.None)
      {
        Debug.LogError("Enemy name cannot be empty");
        return;
      }

      EntityHandler entityHandler = FindAnyObjectByType<EntityHandler>();

      if (entityHandler.GetMob(mob.iD) != null)
      {
        Debug.Log("Already added to the list");
        return;
      }

      // Create the folders if they do not exist
      EditorUtility.CreateFolderIfNeeded(enemyDataFolderPath);
      EditorUtility.CreateFolderIfNeeded(enemyAnimFolderPath);
      EditorUtility.CreateFolderIfNeeded(enemyPrefabFolderPath);

      // Load the enemy template prefab from the Resources folder
      GameObject enemyTemplatePrefab = Resources.Load<GameObject>("EnemyTemplate");
      if (enemyTemplatePrefab == null)
      {
        Debug.LogError("EnemyTemplate prefab not found in Resources!");
        return;
      }

      // Instantiate the enemy template prefab in the scene
      GameObject enemyInstance = Instantiate(enemyTemplatePrefab);
      // Object.FindObjectOfType<TestingGame>().currentEnemy = enemyInstance.transform;
      enemyInstance.name = mob.iD.ToString(); // Rename the instantiated prefab to the desired enemy name
                                              // Selection.activeGameObject = enemyInstance; // Select the newly instantiated enemy in the editor
      GameObject prefab = EditorUtility.CreateOrReplacePrefab(enemyInstance, Path.Combine(enemyPrefabFolderPath, $"{mob.iD}.prefab"));

      GameObject projPrefab = null;
      if ((mob.type == EnemyType.Ranged && preProj == null) || (mob?.type == EnemyType.Magic && createProjectile))
      {

        GameObject projectileTemplate = Resources.Load<GameObject>("ProjectileTemplate");
        if (projectileTemplate == null)
        {
          Debug.LogError("EnemyTemplate prefab not found in Resources!");
          return;
        }

        GameObject projectileInstance = Instantiate(projectileTemplate);

        string n = projectileSprite != null ? projectileName : mob.iD.ToString();
        projectileInstance.name = $"{n}Projectile";

        projPrefab = EditorUtility.CreateOrReplacePrefab(projectileInstance, Path.Combine(enemyProjectileFolderPath, $"{projectileInstance.name}.prefab"));
      }

      if (projPrefab != null)
      {
        mob.projectile = projPrefab;
      }
      else if (preProj != null)
      {
        mob.projectile = preProj;
      }

      if (enemyInstance != null)
      {
        switch (mob.type)
        {
          case EnemyType.Melee:
            enemyInstance.AddComponent<Melee>();
            break;

          case EnemyType.Ranged:
            enemyInstance.AddComponent<Ranged>();
            break;

          case EnemyType.Magic:
            EditorUtility.AttachScript(enemyInstance, customScript);
            break;

          default:
            break;
        }
      }


      // Create the ScriptableObject
      EnemyData enemyData = EditorUtility.CreateScriptable<EnemyData>(mob.iD.ToString(), enemyDataFolderPath);


      //Create the animations
      (string folderPath, AnimatorController controller) = AnimationCreator.CreateControllerAtPath(enemyAnimFolderPath, mob.iD.ToString());
      List<Sprite> sprites = AnimationCreator.UnPackSprites(sheetFolderPath);

      AnimationCreator.CreateAnimationsFromSprites(folderPath, controller, sprites, rows, prefab);

      if ((mob.type == EnemyType.Ranged && createAnimation) || (mob?.type == EnemyType.Magic && createProjectile))
      {
        (string projectileFolder, AnimatorController projectileController) = AnimationCreator.CreateControllerAtPath(enemyAnimFolderPath, mob.iD.ToString());

        List<Sprite> projSprites = AnimationCreator.UnPackSprites(projectileSheet);

        AnimationCreator.CreateAnimationsFromSprites(enemyProjAnimFolderPath, projectileController, projSprites, projRows, projPrefab);
      }
      else if (mob.type == EnemyType.Ranged && !createAnimation && preProj == null)
      {
        projPrefab.GetComponent<SpriteRenderer>().sprite = projectileSprite;
      }
      // Save and focus on the new assets

      NPEntity entity = enemyInstance.GetComponent<NPEntity>();
      entity.data = enemyData;

      entity.body = entity.transform.GetChild(1);
      entity.textSpawn = entity.transform.GetChild(0);

      mob.controller = controller;
      mob.prefab = prefab;
      mob.data = enemyData;
      mob.data.iD = mob.iD;

      Mob newMob = new Mob(mob);
      entityHandler.AddEntity(newMob);

      mob = new Mob();
      projectileSheet = "Copy & Paste Here";
      sheetFolderPath = "Copy & Paste Here";
      customScript = null;
      projectileSprite = null;
      createProjectile = false;
      projectileName = "";
      preProj = null;
      AssetDatabase.SaveAssets();
      // EditorUtility.FocusProjectWindow();
      // Selection.activeObject = enemyData;
    }






  }
}