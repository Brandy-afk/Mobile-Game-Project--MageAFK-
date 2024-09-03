using System.Collections;
using UnityEngine;
using MageAFK.Spells;
using MageAFK.Management;
using MageAFK.Tools;

namespace MageAFK.AI
{
    public class PlayerShaderController : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        private Material currentMaterial;
        private const string mainColor = "_Color";
        private const string tintColor = "_Tint";

        private bool isAnimating;

        private void Awake() => ServiceLocator.RegisterService<PlayerShaderController>(this);

        private void Start()
        {
            currentMaterial = GameResources.Spell.ReturnStatusShader(StatusType.None);
            SetMaterial(currentMaterial);
        }

        public IEnumerator FlashEntityHit()
        {
            isAnimating = true;
            SetMaterial(GameResources.Spell.ReturnHitMaterial());

            SetHitShaderColor(new ColorPair(Color.white, Color.white));
            yield return new WaitForSeconds(.1f);
            SetMaterial(currentMaterial);
            isAnimating = false;
        }


        private void SetHitShaderColor(ColorPair pair)
        {
            spriteRenderer.material.SetColor(mainColor, pair.mainColor);
            spriteRenderer.material.SetColor(tintColor, pair.tintColor);
        }

        public void SetShader(Material material)
        {
            currentMaterial = material != null ? material : GameResources.Spell.ReturnStatusShader(StatusType.None);
            if (!isAnimating) SetMaterial(currentMaterial);
        }

        private void SetMaterial(Material material)
        {
            spriteRenderer.material = material;
        }





    }

}