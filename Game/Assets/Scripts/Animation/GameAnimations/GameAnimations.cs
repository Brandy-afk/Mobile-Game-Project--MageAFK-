using System;
using System.Collections;
using MageAFK.Management;
using UnityEngine;

namespace MageAFK.Animation
{
    public class GameAnimations : MonoBehaviour
  {
    private void Awake() => ServiceLocator.RegisterService(this);

    #region Enemy / Object fade out (made for enemies upon death)

    public IEnumerator FadeOutAndDie(SpriteRenderer spriteRenderer, float duration, Action callback)
    {
      Color color = spriteRenderer.color;

      for (float t = 0f; t < duration; t += Time.deltaTime)
      {
        // Interpolate the alpha value from 1 to 0 over the duration
        color.a = Mathf.Lerp(1f, 0f, t / duration);
        spriteRenderer.color = color;

        yield return null; // Wait for the next frame
      }

      // Ensure the sprite is fully transparent at the end of the duration
      color.a = 0f;
      spriteRenderer.color = color;

      // Place your logic to execute after fading out here
      callback();

    }

    #endregion

    #region Disolve in or out

    /// <summary>
    /// Takes a sprite renderer and then utilizing the Dissolve Material(para) will fade it in or out
    /// </summary>
    /// <param name="spriteRenderer"></param>
    /// <param name="mat"></param>
    /// <param name="duration"></param>
    /// <param name="callback"></param>
    /// <param name="IsIn"></param>
    /// <returns></returns>
    public void DissolveAnimation(SpriteRenderer spriteRenderer, Material mat, float duration = 1f, Action callback = null, bool IsIn = true)
    {
      if (spriteRenderer && mat)
      {
        StartCoroutine(Dissolve(spriteRenderer, mat, duration, callback, IsIn));
      }
    }


    private IEnumerator Dissolve(SpriteRenderer spriteRenderer, Material mat, float duration, Action callback, bool IsIn)
    {
      Material orginal = spriteRenderer.material;
      spriteRenderer.material = mat;

      float start = IsIn ? 0f : 1f, end = IsIn ? 1f : 0f;

      for (float t = 0f; t < duration; t += Time.deltaTime)
      {
        // Interpolate the alpha value from 1 to 0 over the duration
        spriteRenderer.material.SetFloat("_Fade", Mathf.Lerp(start, end, t / duration));
        yield return null; // Wait for the next frame
      }

      callback?.Invoke();
    }


    #endregion



  }

}