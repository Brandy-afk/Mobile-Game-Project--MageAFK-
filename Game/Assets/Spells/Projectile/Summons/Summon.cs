
using System.Collections;
using System.Collections.Generic;
using MageAFK.AI;
using MageAFK.Management;
using MageAFK.Player;
using MageAFK.Stats;
using MageAFK.Tools;
using UnityEngine;

namespace MageAFK.Spells
{
    public abstract class Summon : SpellProjectile
    {
        [SerializeField] protected EntityStateManager stateManager;
        protected float spawnTime;

        public virtual Dictionary<States, bool> states { get; set; } = new Dictionary<States, bool>
            {
                {States.isInRange, false},
                {States.isDead, false},
                {States.siegeOver, false}
            };

        protected Transform target;


        public abstract void Move();
        public abstract IEnumerator Attack();
        public abstract void GetTarget();
        public abstract void Die();
        protected void OnSpawn()
        {
            spawnTime = Time.time;
            ResetStates();
            ServiceLocator.Get<IPlayerDeath>().SubscribeToPlayerDeath(Die, true);
        }
        protected bool GetIsInRange(float variance = 0) => Utility.GetIsInRange(transform.position, target.position, spell.ReturnStatValue(Stat.AttackRange, false), variance);
        protected virtual void ResetStates()
        {
            var tempStates = new Dictionary<States, bool>(states);

            foreach (var b in tempStates.Keys)
            {
                states[b] = false;
            }
        }





    }
}
