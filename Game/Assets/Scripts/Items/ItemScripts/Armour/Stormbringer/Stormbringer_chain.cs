
using System.Collections;
using System.Collections.Generic;
using MageAFK.AI;
using MageAFK.Management;
using MageAFK.Pooling;
using UnityEngine;

namespace MageAFK.Items
{
    public class Stormbringer_chain : AbstractDamager
    {
        private CircleCollider2D coll;
        private Animator ani;
        private ParticleSystem parti;
        private int singleSpawns;
        private int amountToChain;
        private HashSet<Collider2D> hits = new HashSet<Collider2D>();
        private const float timeToDisable = 1f;

        #region Start
        private void Awake()
        {
            parti = GetComponent<ParticleSystem>();
            coll = GetComponent<CircleCollider2D>();
            ani = GetComponent<Animator>();
        }

        public void Initialize(float damage, int amountToChain, HashSet<Collider2D> hits)
        {
            this.damage = damage;
            this.amountToChain = amountToChain;
            this.hits = hits;
        }

        #endregion

        #region Enabled / Disable
        private void OnEnable()
        {
            if (amountToChain == 0) gameObject.SetActive(false);
            singleSpawns = 1;
            coll.enabled = true;
        }

        private void OnDisable() => hits = null;

        #endregion

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!hits.Contains(other))
            {
                if (singleSpawns != 0)
                {
                    hits.Add(other);
                    Transform endObject = other.transform;

                    amountToChain -= 1;
                    singleSpawns = 0;

                    var newParticle = ServiceLocator.Get<ItemPooler>().Get(ItemIdentification.Stormbringer);
                    newParticle.transform.position = endObject.position;
                    newParticle.GetComponent<Stormbringer_chain>().Initialize(damage, amountToChain, hits);
                    DoDamage(other.GetComponentInParent<NPEntity>());
                    newParticle.SetActive(true);

                    Disable();

                    EmitParticles(endObject);
                }
            }
        }

        private void EmitParticles(Transform endObject)
        {
            parti.Play();

            var emitParams = new ParticleSystem.EmitParams
            {
                position = endObject.position
            };

            parti.Emit(emitParams, 1);

            emitParams.position = (transform.position + transform.position) / 2;

            parti.Emit(emitParams, 1);

            emitParams.position = transform.position;

            parti.Emit(emitParams, 1);
        }

        public void Disable()
        {
            ani.StopPlayback();
            coll.enabled = false;
            StartCoroutine(DisableCoroutine());
        }

        private IEnumerator DisableCoroutine()
        {
            // Wait for the specified delay
            yield return new WaitForSeconds(timeToDisable);

            // Disable the GameObject
            gameObject.SetActive(false);
        }
    }
}
