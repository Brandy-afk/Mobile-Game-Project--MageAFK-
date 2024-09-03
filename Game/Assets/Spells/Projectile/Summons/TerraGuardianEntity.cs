using System.Collections;
using MageAFK.AI;
using MageAFK.Core;
using MageAFK.Management;
using MageAFK.Player;
using MageAFK.Stats;
using MageAFK.Tools;
using UnityEngine;

namespace MageAFK.Spells
{
    public class TerraGuardianEntity : Summon
    {

        [SerializeField] private Vector2 mineOffset;
        private float duration;

        public void OnEnabled()
        {
            duration = spell.ReturnStatValue(Stat.SpellDuration);
            OnSpawn();
            StartCoroutine(IntervalUpdate());
            StartCoroutine(SpecialCheck());
        }

        private void Update()
        {
            if (states[States.isDead]) return;

            if (Time.time > (spawnTime + duration))
            {
                Die();
            }

            MoveEntityIntoRange();
        }

        public override void Die()
        {
            states[States.isDead] = true;
            stateManager.ChangeCurrentState(EntityAnimation.Die);
            ServiceLocator.Get<IPlayerDeath>().SubscribeToPlayerDeath(Die, false);
        }

        private const float TIME_BETWEEN_CHECKS = .5f;
        private IEnumerator IntervalUpdate()
        {
            while (!states[States.isDead])
            {
                GetTarget();
                yield return new WaitForSeconds(TIME_BETWEEN_CHECKS);
            }
        }

        private const float TIME_BETWEEN_SPECIAL = 3f;
        private IEnumerator SpecialCheck()
        {
            while (!states[States.isDead])
            {
                if (Utility.RollChance(spell.ReturnStatValue(Stat.SpecialChance)))
                    stateManager.ChangeCurrentState(EntityAnimation.Attack2);

                yield return new WaitForSeconds(TIME_BETWEEN_SPECIAL);
            }
        }

        protected void MoveEntityIntoRange()
        {
            if (target == null) { stateManager.ChangeCurrentState(EntityAnimation.Idle); return; }
            else if (stateManager.ReturnCurrentAnimation() == EntityAnimation.Attack2) return;

            bool isCurrentlyInRange = GetIsInRange();
            if (!states[States.isInRange] && isCurrentlyInRange)
            {
                StartCoroutine(Attack());
            }
            else if (!isCurrentlyInRange && stateManager.ReturnCurrentAnimation() != EntityAnimation.Attack2 && stateManager.ReturnCurrentAnimation() != EntityAnimation.Attack)
            {
                // Not in range. Move towards the player.
                Move();
            }
            // Update the state for the next frame.
            Utility.FlipXSprite(transform.position, target.position, transform);
            states[States.isInRange] = isCurrentlyInRange;
        }

        public override void Move()
        {
            stateManager.ChangeCurrentState(EntityAnimation.Run);
            transform.position = Vector2.MoveTowards(transform.position, target.position, spell.ReturnStatValue(Stat.MovementSpeed, false));
        }

        public override void GetTarget() => target = ServiceLocator.Get<EntityTracker>().ReturnBestTarget(FocusEntity.ClosestTarget, transform).Transform;

        public override IEnumerator Attack()
        {
            yield return null;
            while (target != null && GetIsInRange() && !states[States.isDead])
            {
                stateManager.ChangeCurrentState(EntityAnimation.Attack);
                yield return new WaitForSeconds(spell.ReturnStatValue(Stat.AttackCooldown, modStat: Stat.Cooldown));
            }
        }

        public void Hit()
        {
            if (target == null) return;

            if (!GetIsInRange(3)) stateManager.ChangeCurrentState(EntityAnimation.Run);
            else if (target.TryGetComponent<NPEntity>(out NPEntity enemy))
            {
                Spell.SpawnEffect(enemy.transform.position, SpellEffectAnimation.Earth_1);
                base.HandleDamage(enemy);
            }
        }

        public void Special()
        {
            (spell as TerraGuardian).SpawnMine((Vector2)transform.position + mineOffset);
        }
    }
}
