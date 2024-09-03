
using System.Collections.Generic;
using MageAFK.Combat;
using MageAFK.Spells;
using MageAFK.Stats;
using MageAFK.UI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MageAFK.AI
{
    public abstract class Entity : MonoBehaviour, IEntityPosition
    {

        [TabGroup("Locations")]
        public Transform body;

        [TabGroup("Locations")]
        public Transform feet;

        public Transform Transform => transform;
        public Vector2 Pivot => transform.position;
        public Vector2 Body => body.position;
        public Vector2 Feet => feet.position;

        public abstract Dictionary<States, bool> states { get; set; }

        //Reset states like 'isDead'
        public abstract void Die(bool forcedDeath = false);

        public abstract void HandleStatusEffect(Stat stat, float mod = 0, bool positive = false);

        public abstract void StatusDamage(StatusType type, float damage, int iD, OrginType source, TextInfoType textType = TextInfoType.Default);


        /// <summary>
        /// Do damage to the entity.
        /// </summary>
        /// <param name="damage">Damage amount.</param>
        /// <param name="spell">Source of damage.</param>
        /// <param name="textType">Type of text to be printed on screen. (leave null if damage for player)</param>
        /// <returns>True if entity is killed otherwise false</returns>
        public abstract bool DoDamage(float damage, Spell spell, TextInfoType textType = TextInfoType.Default);


        /// <summary>
        /// Do damage to the entity.
        /// </summary>
        /// <param name="damage">Damage amount</param>
        /// <param name="entity">The source of this damage.</param>
        /// <param name="textType">Type of text to be printed on screen. (leave null if damage for player)</param>
        /// <returns>True if entity is killed otherwise false</returns>
        public abstract bool DoDamage(float damage, NPEntity entity, TextInfoType textType = TextInfoType.Default);

        public bool SetState(States state, bool mode)
        {
            if (states.ContainsKey(state))
            {
                states[state] = mode;
                return true;
            }

            return false;
        }
    }

    public interface IEntityPosition
    {
        public Transform Transform { get; }
        public Vector2 Pivot { get; }
        public Vector2 Body { get; }
        public Vector2 Feet { get; }
    }

    public enum States
    {
        isInRange = 0,
        isDead = 1,
        isRooted = 2,
        isStunned = 3,
        isConfused = 4,
        siegeOver = 5,
        isFeared = 6,
        inMap = 7,
    }

    

}