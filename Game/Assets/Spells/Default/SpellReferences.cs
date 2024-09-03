
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace MageAFK.Spells
{

  [CreateAssetMenu(fileName = "Spell References", menuName = "SpellReferences", order = 51)]
  public class SpellReferences : SerializedScriptableObject
  {

    [SerializeField] private Dictionary<SpellBook, Sprite> bookRef;
    [SerializeField] private Dictionary<StatusType, SpellStatusReference> defaultStatusReferences;
    [SerializeField] private Dictionary<StatusType, Material> defaultShaderPairs;
    [SerializeField] private Dictionary<SpellElement, VertexGradient> elementVerGrad;

    [SerializeField] private Dictionary<SpellEffectAnimation, RuntimeAnimatorController> effectAnimations;
    [SerializeField] private Dictionary<SpellIdentification, RuntimeAnimatorController> spellAnimations;
    [SerializeField] private Material hitMaterial;

    public Material ReturnHitMaterial()
    {
      return hitMaterial;
    }

    public Sprite ReturnBookImage(SpellBook spellBook)
    {
      if (bookRef.ContainsKey(spellBook))
      {
        return bookRef[spellBook];
      }

      Debug.Log("SpellBook not found in dict");
      return null;
    }


    public Sprite ReturnStatusSprite(StatusType status)
    {

      if (defaultStatusReferences.ContainsKey(status))
      {

        return defaultStatusReferences[status].image;
      }

      Debug.Log($"Bad key : {status}");
      return null;

    }

    public string ReturnStatusDesc(StatusType status)
    {
      if (defaultStatusReferences.ContainsKey(status))
      {
        return defaultStatusReferences[status].desc;
      }

      Debug.Log($"Bad key : {status}");
      return defaultStatusReferences[StatusType.None].desc;
    }

    public Material ReturnStatusShader(StatusType status)
    {
      if (defaultShaderPairs.ContainsKey(status))
      {
        return defaultShaderPairs[status];
      }

      Debug.Log($"Bad key : {status}");
      return defaultShaderPairs[StatusType.None];
    }

    public RuntimeAnimatorController ReturnEffect(SpellEffectAnimation effect)
    {
      if (effectAnimations.ContainsKey(effect))
      {
        return effectAnimations[effect];
      }
      Debug.Log($"Bad key : {effect}");
      return null;
    }

    public RuntimeAnimatorController ReturnEffect(SpellIdentification effect)
    {
      if (spellAnimations.ContainsKey(effect))
      {
        return spellAnimations[effect];
      }
      Debug.Log($"Bad key : {effect}");
      return null;
    }


    public VertexGradient ReturnElementGradient(SpellElement element)
    {
      try
      {
        return elementVerGrad[element];
      }
      catch (KeyNotFoundException)
      {
#if DEBUG
        Debug.Log("Element Not Found");
#endif
        return new VertexGradient(Color.white);
      }
    }
  }



  [System.Serializable]
  public class SpellStatusReference
  {
    public Sprite image;
    [TextArea(5, 5)]
    public string desc;

  }


}

[System.Serializable]
public struct ColorPair
{
  public Color mainColor;
  public Color tintColor;

  public ColorPair(Color mainColor, Color tintColor)
  {
    this.mainColor = mainColor;
    this.tintColor = tintColor;
  }
}