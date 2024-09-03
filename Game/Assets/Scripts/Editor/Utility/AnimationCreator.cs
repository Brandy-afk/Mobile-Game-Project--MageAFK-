using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace MageAFK.Creation
{

    [System.Serializable]
  public class SpriteAnimation
  {
    [Header("IMPORTANT")]
    [Header("If 'createUIController', does not matter if forUI is enabled")]
    [Space(5)]
    public int index;
    public int length;

    public bool createEvent = false;
    [Header("Leave events null if createEvent is false")]
    public AnimationEventInfo[] events;

    public bool forUI = false;

    public bool loopAnim = false;

    public SpriteAnimation(int index, int length, bool forUI, bool loopAnim)
    {
      this.index = index;
      this.length = length;
      this.forUI = forUI;
      this.loopAnim = loopAnim;
    }

    public SpriteAnimation()
    { }
  }

  [System.Serializable]
  public class AnimationEventInfo
  {
    public int index;
    public string function;
  }

  public static class AnimationCreator
  {
    public const float standardFrameDuration = 1f / 6f;

    public static (string, AnimatorController) CreateControllerAtPath(string rootPath, string objectName)
    {
      // Create a subfolder for the object inside the animation folder
      string newFolderPath = Path.Combine(rootPath, objectName);
      EditorUtility.CreateFolderIfNeeded(newFolderPath);

      // Create the Animation Controller in the enemy's animation folder
      string controllerPath = Path.Combine(newFolderPath, $"{objectName}.controller");
      string path = AssetDatabase.GenerateUniqueAssetPath(controllerPath);
      AnimatorController c = AnimatorController.CreateAnimatorControllerAtPath(path);

      return (newFolderPath, c);
    }


    public static List<Sprite> UnPackSprites(string path)
    {

      UnityEngine.Object[] loadedObjects = AssetDatabase.LoadAllAssetsAtPath(path);
      List<Sprite> sprites = new List<Sprite>();


      foreach (var loadedObject in loadedObjects)
      {
        if (loadedObject is Sprite)
        {
          sprites.Add((Sprite)loadedObject);
        }
      }

      if (sprites == null || sprites.Count == 0)
      {
        Debug.LogError("No sprites found at the specified path.");
        return null;
      }

      return sprites;
    }



    public static void CreateAnimationsFromSprites
    (string folderPath, AnimatorController c, List<Sprite> sprites, Dictionary<string, SpriteAnimation> animations, GameObject prefab = null, bool uiController = false)
    {
      foreach (var pair in animations)
      {
        CreateAndAddClip(pair.Key, pair.Value, sprites, c, folderPath, uiController);
      }

      if (prefab != null)
      {
        prefab.GetComponent<SpriteRenderer>().sprite = sprites[0];
        if (!uiController)
        {
          prefab.GetComponent<Animator>().runtimeAnimatorController = c as RuntimeAnimatorController;
        }
      }

    }


    private static void CreateAndAddClip(string name, SpriteAnimation anim, List<Sprite> sprites, AnimatorController controller, string folderPath, bool uiController)
    {
      AnimationClip clip = new AnimationClip();
      string nPath = Path.Combine(folderPath, $"{name}.anim");
      string clipPath = AssetDatabase.GenerateUniqueAssetPath(nPath);


      if (anim.loopAnim || uiController)
      {
        AnimationClipSettings clipSettings = AnimationUtility.GetAnimationClipSettings(clip);
        clipSettings.loopTime = true;
        AnimationUtility.SetAnimationClipSettings(clip, clipSettings);
      }

      AssetDatabase.CreateAsset(clip, clipPath);

      ObjectReferenceKeyframe[] keyframes = new ObjectReferenceKeyframe[anim.length + 1];


      for (int i = 0, index = anim.index; i < keyframes.Length; i++, index++) // Loop through the sprites
      {
        if (i == anim.length)
        {
          index--;
        }

        keyframes[i] = new ObjectReferenceKeyframe
        {
          time = i * standardFrameDuration,
          value = sprites[index]
        };
      }

      keyframes[anim.length].time = keyframes[anim.length - 1].time + standardFrameDuration;
      // Set curve for SpriteRenderer


      #region Binding
      // Binding for SpriteRenderer
      EditorCurveBinding spriteRendererBinding = EditorCurveBinding.PPtrCurve(string.Empty, typeof(SpriteRenderer), "m_Sprite");
      // Binding for UIImage if needed
      EditorCurveBinding uiImageBinding = anim.forUI || uiController ? EditorCurveBinding.PPtrCurve(string.Empty, typeof(UnityEngine.UI.Image), "m_Sprite") : default;


      if (!uiController)
      {
        AnimationUtility.SetObjectReferenceCurve(clip, spriteRendererBinding, keyframes);
      }

      // Set curve for UIImage if necessary
      if (anim.forUI || uiController)
      {
        AnimationUtility.SetObjectReferenceCurve(clip, uiImageBinding, keyframes);
      }

      #endregion

      AddClipToAnimatorController(clip, controller);

      if (anim.createEvent && !uiController)
      {
        if (anim.events != null)
        {
          foreach (var animEvent in anim.events)
          {
            AddAnimationEvent(clip, animEvent.function, standardFrameDuration * animEvent.index);
          }
        }
      }
    }

    private static void AddClipToAnimatorController(AnimationClip clip, AnimatorController controller)
    {
      // Assumes that the animatorController is a field or property available
      // You would need to assign the animatorController before running this
      AnimatorControllerLayer layer = controller.layers[0];
      AnimatorStateMachine stateMachine = layer.stateMachine;
      AnimatorState state = stateMachine.AddState(clip.name);
      state.motion = clip;

      AssetDatabase.SaveAssets();
      AssetDatabase.Refresh();
    }

    private static void AddAnimationEvent(AnimationClip clip, string functionName, float time)
    {
      AnimationEvent animEvent = new AnimationEvent
      {
        functionName = functionName,
        time = time
      };

      // Get existing events, add the new one, and then set them back
      AnimationEvent[] existingEvents = AnimationUtility.GetAnimationEvents(clip);
      var allEvents = new AnimationEvent[existingEvents.Length + 1];
      existingEvents.CopyTo(allEvents, 0);
      allEvents[existingEvents.Length] = animEvent;

      AnimationUtility.SetAnimationEvents(clip, allEvents);
    }




  }
}
