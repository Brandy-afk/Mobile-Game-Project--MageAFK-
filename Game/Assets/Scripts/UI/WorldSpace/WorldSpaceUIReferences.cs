
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using MageAFK.Items;
using MageAFK.Management;

namespace MageAFK.UI
{
    public class WorldSpaceUIReferences : SerializedMonoBehaviour
  {

    [SerializeField] private DamageText dmgRef;
    [SerializeField] private ValueText valueRef;

    private Dictionary<TextInfoType, TextInformation> cachedTextInfos = new Dictionary<TextInfoType, TextInformation>();

    private void Awake()
    {
      ServiceLocator.RegisterService(this);
      // Cache the default TextInformation
      cachedTextInfos[TextInfoType.Default] = new TextInformation(dmgRef.regularFont, dmgRef.regularFontSize, dmgRef.regularGradient, "0");

      // Cache TextInformation for different collision types
      cachedTextInfos[TextInfoType.Crit] = new TextInformation(dmgRef.specialFont, dmgRef.specialFontSize, dmgRef.critGradient, "");
      cachedTextInfos[TextInfoType.Status] = new TextInformation(dmgRef.specialFont, dmgRef.specialFontSize, dmgRef.elementalGradient, "");
      cachedTextInfos[TextInfoType.Pierce] = new TextInformation(dmgRef.specialFont, dmgRef.specialFontSize, dmgRef.pierceGradient, "");

      // Cache TextInformation for different status types
      cachedTextInfos[TextInfoType.Burn] = new TextInformation(dmgRef.regularFont, dmgRef.regularFontSize, dmgRef.burnGradient, "");
      cachedTextInfos[TextInfoType.Bleed] = new TextInformation(dmgRef.regularFont, dmgRef.regularFontSize, dmgRef.bleedGradient, "");
      cachedTextInfos[TextInfoType.Corrupt] = new TextInformation(dmgRef.regularFont, dmgRef.regularFontSize, dmgRef.corruptionGradient, "");
      cachedTextInfos[TextInfoType.Smite] = new TextInformation(dmgRef.regularFont, dmgRef.regularFontSize, dmgRef.smiteGradient, "");

      // Cache TextInformation for different stat types
      cachedTextInfos[TextInfoType.ThornsDamage] = new TextInformation(dmgRef.regularFont, dmgRef.regularFontSize, dmgRef.thornDamage, "");
    }

    public TextInformation GetTextInfo(TextInfoType type, string damage = "0")
    {
      try
      {
        cachedTextInfos[type].text = damage;
        return cachedTextInfos[type];
      }
      catch (KeyNotFoundException)
      {
        Debug.Log("error");
        return null;
      }

    }

    public VertexGradient ReturnValueGradient(string type)
    {
      switch (type)
      {
        case "XP":
          return valueRef.levelGradient;
        case "SilverCoins":
          return valueRef.silverGradient;
        case "DemonicGems":
          return valueRef.gemGradient;

        default:
          return dmgRef.regularGradient;
      }
    }


  }

  public enum TextInfoType
  {
    Default,
    Crit,
    Status,
    Pierce,
    Burn,
    Bleed,
    Corrupt,
    Smite,
    ThornsDamage
  }

  [System.Serializable]
  public class DamageText
  {

    //Proc damage
    public VertexGradient critGradient;
    public VertexGradient pierceGradient;
    public VertexGradient elementalGradient;
    public VertexGradient regularGradient;

    //Status damage
    public VertexGradient burnGradient;
    public VertexGradient bleedGradient;
    public VertexGradient corruptionGradient;
    public VertexGradient smiteGradient;

    //Stat damage
    public VertexGradient thornDamage;


    public TMP_FontAsset regularFont;
    public TMP_FontAsset specialFont;

    public float specialFontSize;
    public float regularFontSize;
  }

  [System.Serializable]
  public class ValueText
  {
    public VertexGradient silverGradient;
    public VertexGradient gemGradient;
    public VertexGradient levelGradient;

  }

  public class TextInformation
  {
    public TMP_FontAsset font;
    public float fontSize;
    public VertexGradient gradient;
    public string text;

    public TextInformation(TMP_FontAsset font, float fontSize, VertexGradient gradient, string text)
    {
      this.font = font;
      this.fontSize = fontSize;
      this.gradient = gradient;
      this.text = text;
    }
  }
}
