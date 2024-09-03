

using System.Collections.Generic;
using MageAFK.AI;
using MageAFK.Management;
using MageAFK.Pooling;
using MageAFK.Stats;
using MageAFK.Testing;
using MageAFK.Tools;
using UnityEngine;

namespace MageAFK.Spells
{
    public class SoulgaleController : SpellProjectile, IRangeVisualizer
    {

        public SuctionSpellTesting testing;

        [SerializeField] private float timeBetweenDamage;
        [SerializeField] private float suctionPower;
        [SerializeField] private float maxForce;

        private Dictionary<GameObject, float> lastDamageTime = new();
        private bool isActive = false;
        private GameObject rangeVisual;

        public void SetActive()
        {
            rangeVisual = ServiceLocator.Get<SpellPooler>().Get(SpellIdentification.SpellUtility_Range, spell.iD);
            rangeVisual.transform.position = transform.position;
            rangeVisual.SetActive(true);
            rangeVisual.GetComponent<SpellRangeVisualizer>().SetUpVisualizer(spell.ReturnStatValue(Stat.Range), this);

            isActive = true;
        }


        public void OnStay(Collider2D other)
        {
            if (!isActive || !Utility.VerifyTags(targetTags, other)) { return; }
            NPEntity entity = other.GetComponentInParent<NPEntity>();
            Rigidbody2D rb = other.GetComponentInParent<Rigidbody2D>();
            if (rb && entity)
            {
                HandleForce(entity, rb);
                DoDamage(entity);
            }
        }

        public void DoDamage(NPEntity entity)
        {
            if (!lastDamageTime.TryGetValue(entity.gameObject, out float lastTime) || Time.time - lastTime >= testing.timeBetweenDamage)
            {
                // Update last damage time.
                HandleDamage(entity);
                lastDamageTime[entity.gameObject] = Time.time;
            }
        }

        public void HandleForce(NPEntity entity, Rigidbody2D rb)
        {
            Vector2 directionFromCenter = rb.position - (Vector2)transform.position;
            float distance = directionFromCenter.magnitude;

            if (entity.states[States.isRooted] || entity.states[States.isDead]) { return; }
            else
            {
                float forceMagnitude = Mathf.Clamp(1.0f / distance, 0, testing.maxForce);
                Vector2 force = directionFromCenter.normalized * forceMagnitude * testing.suctionPower;  // Multiply by suctionPower here
                rb.AddForce(force);
            }
        }

        public override void Disable()
        {
            gameObject.SetActive(false);
            rangeVisual.SetActive(false);
            lastDamageTime.Clear();
            rangeVisual = null;
        }



        //Not in use
        public void OnEnter(Collider2D other)
        {

        }

        public void OnLeave(Collider2D other)
        {

        }
    }
}
