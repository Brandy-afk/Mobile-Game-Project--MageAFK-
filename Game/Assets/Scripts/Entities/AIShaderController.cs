using System.Collections;
using UnityEngine;
using MageAFK.Combat;
using MageAFK.Spells;
using MageAFK.Tools;

namespace MageAFK.AI
{
    public class AIShaderController : MonoBehaviour
  {
    [SerializeField] private SpriteRenderer spriteRenderer;

    private StatusHandler statusHandler;



    private Material currentMaterial;
    private string mainColor = "_Color";
    private string tintColor = "_Tint";

    private bool isAnimating;

    private static readonly StatusType[] statusTypes = new StatusType[]
    {
        StatusType.Burn,
        StatusType.Slow,
        StatusType.Bleed,
        StatusType.Corrupt,
        StatusType.Poison,
        StatusType.Confuse
    };


    private void OnEnable()
    {
      currentMaterial = GameResources.Spell.ReturnStatusShader(StatusType.None);
      SetMaterial(currentMaterial);
    }

    public void SetStatusHandler(StatusHandler h)
    {
      statusHandler = h;
    }

    public IEnumerator FlashEntityHit(ColorPair pair)
    {
      isAnimating = true;
      SetMaterial(GameResources.Spell.ReturnHitMaterial());
      SetHitShaderColor(pair);
      yield return new WaitForSeconds(.1f);
      SetMaterial(currentMaterial);
      isAnimating = false;
    }

    public void UpdateShaderDisplay()
    {
      StatusType status = StatusType.None;
      foreach (var statusType in statusTypes)
      {
        if (statusHandler != null && statusHandler.CheckIfEffect(statusType))
        {
          status = statusType;
          break;
        }
      }

      currentMaterial = GameResources.Spell.ReturnStatusShader(status);

      if (!isAnimating)
      {
        SetMaterial(currentMaterial);
      }
    }

    private void SetHitShaderColor(ColorPair pair)
    {
      spriteRenderer.material.SetColor(mainColor, pair.mainColor);
      spriteRenderer.material.SetColor(tintColor, pair.tintColor);
    }

    private void SetMaterial(Material material)
    {
      spriteRenderer.material = material;
    }





  }

}