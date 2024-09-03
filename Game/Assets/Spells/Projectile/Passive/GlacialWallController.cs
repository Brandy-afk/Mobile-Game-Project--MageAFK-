
using System.Collections.Generic;
using MageAFK.AI;
using MageAFK.Management;
using MageAFK.Pooling;
using MageAFK.Stats;
using MageAFK.Tools;
using UnityEngine;

namespace MageAFK.Spells
{
    public class GlacialWallController : SpellProjectile, IRangeVisualizer
    {
        [HideInInspector] public GameObject rangeLine;
        private HashSet<Enemy> inRange = new();
        private bool active = false;
        private float spawnTime;
        private float duration;

        public void OnStartAnimationFinished()
        {
            rangeLine = ServiceLocator.Get<SpellPooler>().Get(SpellIdentification.SpellUtility_Range, spell.iD);
            rangeLine.GetComponent<SpellRangeVisualizer>().SetUpVisualizer(spell.ReturnStatValue(Stat.Range), this);
            rangeLine.transform.SetParent(transform);
            rangeLine.transform.localPosition = Vector2.zero;
            rangeLine.gameObject.SetActive(true);
            duration = spell.ReturnStatValue(Stat.SpellDuration);
            spawnTime = Time.time;
            active = true;
            //Get inital enemies in area
            foreach (Collider2D collider in PhysicsCheck())
                OnEnter(collider);

        }

        private Collider2D[] PhysicsCheck()
            => Physics2D.OverlapCircleAll(transform.position, spell.ReturnStatValue(Stat.Range), ReturnMask(LayerCollision.Feet));

        private void Update()
        {
            if (active && Time.time > (spawnTime + duration))
            {
                InitialDisable();
            }
        }

        public void OnEnter(Collider2D other)
        {
            if (active && Utility.VerifyTags(targetTags, other))
            {
                var enemy = GetComponentInParent<Enemy>();
                if (inRange.Contains(enemy)) return;

                inRange.Add(enemy);
                enemy.SubscribeToEnemyDeath(OnEnemyDeath, true);
                GlacialWall.OnEnter(enemy, spell);
            }
        }

        public void OnLeave(Collider2D other)
        {
            if (active && Utility.VerifyTags(targetTags, other))
            {
                var enemy = GetComponentInParent<Enemy>();
                // Line below could make this malfunction.
                if (!inRange.Contains(enemy)) return;

                enemy.SubscribeToEnemyDeath(OnEnemyDeath, false);
                inRange.Remove(enemy);
                GlacialWall.OnLeave(enemy, spell);
            }
        }


        public void OnStay(Collider2D other)
        {

        }

        public void OnEnemyDeath(Enemy enemy)
        {
            enemy.SubscribeToEnemyDeath(OnEnemyDeath, false);
            inRange.Remove(enemy);
        }

        public void InitialDisable()
        {
            active = false;
            rangeLine.SetActive(false);
            rangeLine = null;

            foreach (var enemy in inRange)
            {
                GlacialWall.OnLeave(enemy, spell);
                enemy.SubscribeToEnemyDeath(OnEnemyDeath, false);
            }

            inRange.Clear();
            GetComponent<Animator>().Play("End");
        }

    }
}
