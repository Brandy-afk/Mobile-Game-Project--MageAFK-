using System;
using MageAFK.AI;
using MageAFK.Management;
using MageAFK.UI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MageAFK.Testing
{
    public class MobTestingScript : MonoBehaviour, IEntityPosition
    {
        [SerializeField] private float maxHealth = 500;
        [SerializeField] private float health = 500;

        public Transform textSpawn;
        public Transform healthBarLoc;

        [ReadOnly]
        public EntityUIController uiController;
        public AIShaderController shaderController;

        public Transform Transform => transform;

        public Vector2 Pivot => transform.position;

        public Vector2 Body => transform.position;

        public Vector2 Feet => transform.position;

        private void Start()
        {
            uiController = ServiceLocator.Get<WorldSpaceUI>().AddEntityUIPair(this);
            uiController.UpdateHealthBar(health, maxHealth);
        }


        public void TakeDamage(float damage)
        {
            health -= damage;
            health = Math.Max(health, 0);

            if (health == 0)
            {
                health = maxHealth;
            }

            uiController.UpdateHealthBar(health, maxHealth);

            StartCoroutine(shaderController.FlashEntityHit(new ColorPair(Color.white, Color.white)));
            var textInfo = ServiceLocator.Get<WorldSpaceUIReferences>().GetTextInfo(TextInfoType.Default, damage.ToString());
            ServiceLocator.Get<WorldSpaceUI>().CreateDamageText(textSpawn, textInfo);
        }



    }
}
