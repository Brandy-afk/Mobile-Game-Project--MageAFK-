using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MageAFK.Animation
{
  public class UIExperienceBarAM : MonoBehaviour
  {
    [System.Serializable]
    public class AnimationState
    {
      public string name;
      public List<Sprite> frames;
      public float frameRate = 0.1f;
    }

    public List<AnimationState> animationStates;
    public Image uiImage;

    private Coroutine currentAnimation;
    private int currentState;
    private bool isSwitching;

    void Start()
    {
      if (!uiImage)
      {
        Debug.LogError("Image component not assigned in the inspector. Please assign an Image component.");
        return;
      }

      if (animationStates.Count == 0)
      {
        Debug.LogWarning("No animation states defined. Add at least one animation state.");
      }
      else
      {
        PlayAnimation(0);
      }
    }



    public void SwitchAnimationState(int newIndex)
    {
      if (newIndex != 2 && isSwitching) return;
      if (newIndex < 0 || newIndex >= animationStates.Count)
      {
        Debug.LogWarning("Invalid animation index. Please provide a valid index.");
        return;
      }
      if (newIndex == 2)
      {
        StopAllCoroutines();
      }


      StartCoroutine(SwitchAnimationStateCoroutine(newIndex));
    }

    private IEnumerator SwitchAnimationStateCoroutine(int newIndex)
    {
      isSwitching = true;
      currentState = newIndex;
      PlayAnimation(currentState);
      yield return new WaitForSeconds(animationStates[currentState].frameRate * animationStates[currentState].frames.Count);
      isSwitching = false;
    }

    private void PlayAnimation(int stateIndex)
    {
      if (currentAnimation != null)
      {
        StopCoroutine(currentAnimation);
      }

      currentAnimation = StartCoroutine(PlayAnimationCoroutine(animationStates[stateIndex]));
    }

    private IEnumerator PlayAnimationCoroutine(AnimationState animationState)
    {
      int frameIndex = 0;
      int loopCounter = 0;
      int loopLimit = currentState == 0 ? -1 : animationState.frames.Count;



      while (loopCounter != loopLimit)
      {
        uiImage.sprite = animationState.frames[frameIndex];
        frameIndex = (frameIndex + 1) % animationState.frames.Count;

        if (frameIndex == 0)
        {
          loopCounter++;
        }

        yield return new WaitForSeconds(animationState.frameRate);
      }

      if (currentState == 1 || currentState == 2)
      {
        SwitchAnimationState(0);
      }
    }
  }


}



