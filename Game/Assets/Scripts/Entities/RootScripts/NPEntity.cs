
using System.Collections.Generic;
using MageAFK.Animation;
using MageAFK.Combat;
using MageAFK.Core;
using MageAFK.Management;
using MageAFK.Spells;
using MageAFK.Stats;
using MageAFK.UI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MageAFK.AI
{
    public abstract class NPEntity : Entity, INonPlayerPosition
    {



        [TabGroup("Locations")]
        public Transform textSpawn;

        [TabGroup("Locations")]
        [SerializeField] public Vector2 healthBarLoc;

        [TabGroup("Locations")]
        [SerializeField] public Vector2 effectsLoc;

        public Vector2 EffectsPosition => effectsLoc;
        public Vector2 HealthPosition => healthBarLoc;


        [TabGroup("Data")]
        public AIData data;

        [TabGroup("Data")]
        public abstract Dictionary<Stat, float> runtimeStats { get; set; }


        public AIStatusHandler StatusHandler { get; private set; }
        public EntityUIController UiController { get; private set; }
        public Rigidbody2D Rb { get; private set; }


        protected AIShaderController shaderController;
        protected readonly Dictionary<NPEntityCollider, Collider2D> colliders = new();
        protected EntityStateManager stateManager;
        protected SpriteRenderer spriteRenderer;


        //Animation
        protected const float fadeOutDuration = 1f;

        #region Utility
        protected virtual void OnAwake()
        {
            colliders[NPEntityCollider.Entity] = GetComponent<Collider2D>();
            colliders[NPEntityCollider.Body] = body.GetComponent<Collider2D>();
            colliders[NPEntityCollider.Feet] = feet.GetComponent<Collider2D>();

            colliders[NPEntityCollider.Entity].enabled = true;
            ToggleCollisionColliders(false);

            StatusHandler = GetComponent<AIStatusHandler>();
            shaderController = GetComponent<AIShaderController>();

            spriteRenderer = GetComponent<SpriteRenderer>();
            Rb = GetComponent<Rigidbody2D>();
            stateManager = GetComponent<EntityStateManager>();
        }
        protected virtual void LoadData()
        {
            if (runtimeStats == null) { runtimeStats = new Dictionary<Stat, float>(); }
            Dictionary<Stat, float> alteredStats = data.GetStats(AIDataType.Altered);

            foreach (var stat in alteredStats.Keys)
            {
                runtimeStats[stat] = alteredStats[stat];
            }
        }

        public virtual void OnSetActive()
        {
            //Add health bar
            UiController = ServiceLocator.Get<WorldSpaceUI>().AddEntityUIPair(this);

            //Update UI loc
            UiController.SetUILocations(healthBarLoc, effectsLoc);

            if (StatusHandler != null)
            {
                StatusHandler.SetEntityUIController(UiController);

                StatusHandler.UpdateVisualDisplays();
            }

            LoadData();
            ResetStates();

            //Register enemy for data modification
            ServiceLocator.Get<EntityDataManager>().RegisterEntity(this);

            //Update health bar
            UiController.UpdateHealthBar(runtimeStats[Stat.Health], data.GetStats(AIDataType.Altered)[Stat.Health]);

            //Set Color Back to full opacity
            Color color = spriteRenderer.color;
            color.a = 1f;
            spriteRenderer.color = color;
        }

        protected void ToggleCollisionColliders(bool state)
        {
            colliders[NPEntityCollider.Body].enabled = state;
            colliders[NPEntityCollider.Feet].enabled = state;
        }

        public Collider2D GetCollider(NPEntityCollider collider)
        {
            try
            {
                return colliders[collider];
            }
            catch (KeyNotFoundException)
            {
                Debug.LogError("Missing collider");
                return null;
            }
        }


        protected virtual void ResetStates()
        {
            var tempStates = new Dictionary<States, bool>(states);

            foreach (var b in tempStates.Keys)
            {
                states[b] = false;
            }
        }
        #endregion

        #region Stats
        public override void HandleStatusEffect(Stat stat, float mod = 0, bool positive = false)
        {
            runtimeStats[stat] = mod == 0 ? data.GetStats(AIDataType.Altered)[stat]
            : positive ? data.GetStats(AIDataType.Altered)[stat] + (data.GetStats(AIDataType.Altered)[stat] * (mod / 100))
            : data.GetStats(AIDataType.Altered)[stat] - (data.GetStats(AIDataType.Altered)[stat] * (mod / 100));
        }

        public virtual float ReturnStat(Stat stat)
        {
            try
            {
                return runtimeStats[stat];
            }
            catch (KeyNotFoundException)
            {
                Debug.Log($"Bad key : {stat}");
                return 0;
            }
        }

        public virtual void RecalculateStat(Stat stat)
        {
            runtimeStats[stat] = data.GetStats(AIDataType.Altered)[stat];

            switch (stat)
            {
                case Stat.MovementSpeed:
                    if (!StatusHandler.activeEffects.ContainsKey(StatusType.Slow))
                        return;

                    SlowEffect.ApplyBest(this, StatusHandler.activeEffects[StatusType.Slow]);
                    break;
                case Stat.Damage:
                    if (!StatusHandler.activeEffects.ContainsKey(StatusType.Weaken))
                        return;

                    WeakenEffect.ApplyBest(this, StatusHandler.activeEffects[StatusType.Weaken]);
                    break;
            }
        }

        #endregion

        #region Damage / Death
        //Status effect damage
        public override void StatusDamage(StatusType type, float damage, int iD, OrginType source, TextInfoType textType)
        {
            var values = DoDamage(damage, textType);
            if (values.Item1)
            {
                var spell = source == OrginType.Spell ? ServiceLocator.Get<SpellHandler>().GetSpellData((SpellIdentification)iD) : null;
                if (spell != null) MetricController.RecordSpellMetrics(spell, values.Item2, states[States.isDead]);
            }
        }

        //Other entities damage
        public override bool DoDamage(float damage, NPEntity entity, TextInfoType textType = TextInfoType.Default)
        {
            var values = DoDamage(damage, textType);
            if (values.Item1)
            {

            }
            return states[States.isDead];
        }

        //Spell Damage
        public override bool DoDamage(float damage, Spell spell, TextInfoType textType = TextInfoType.Default)
        {
            var values = DoDamage(damage, textType);
            if (values.Item1)
            {
                StartCoroutine(shaderController.FlashEntityHit(spell.shaderColors));
                MetricController.RecordSpellMetrics(spell, values.Item2, states[States.isDead]);
            }

            return states[States.isDead];
        }

        public virtual (bool, float) DoDamage(float damage, TextInfoType textType = TextInfoType.Default)
        {
            if (states[States.isDead]) return (false, 0);

            float actualDamage = Mathf.Min(damage, runtimeStats[Stat.Health]);
            runtimeStats[Stat.Health] -= actualDamage;
            UiController.UpdateHealthBar(runtimeStats[Stat.Health], data.GetStats(AIDataType.Altered)[Stat.Health]);

            CreateText(actualDamage, textType);
            CheckIfDead(actualDamage);
            //Tracking Statistics
            MetricController.DamageEnemyMetrics(damage);

            return (true, actualDamage);
        }

        protected virtual void CreateText(float actualDamage, TextInfoType textType)
        {
            var info = ServiceLocator.Get<WorldSpaceUIReferences>().GetTextInfo(textType, actualDamage.ToString("N1"));
            ServiceLocator.Get<WorldSpaceUI>().CreateDamageText(textSpawn, info);
        }

        protected virtual bool CheckIfDead(float actualDamage)
        {
            if (runtimeStats[Stat.Health] <= 0)
            {
                states[States.isDead] = true;

                MetricController.RecordKillMetrics(data.iD);

                if (actualDamage >= data.GetStats(AIDataType.Altered)[Stat.Health])
                    ServiceLocator.Get<MilestoneHandler>().UpdateMileStone(MilestoneID.OneShotEnemies, 1);

                if (StatusHandler.CheckIfEffect(StatusType.Slow))
                    ServiceLocator.Get<MilestoneHandler>().UpdateMileStone(MilestoneID.KillSlowedEnemies, 1);


                Die();
                return true;
            }
            return false;
        }
        public override void Die(bool forcedDeath = false)
        {
            //Turn off colliders
            colliders[NPEntityCollider.Body].enabled = false;
            colliders[NPEntityCollider.Feet].enabled = false;

            //Remove enemy from being tracked
            //UI death method
            UiController?.OnDeath();

            //Unregister enemy from the data manager so runtime stats are no longer manipulated
            ServiceLocator.Get<EntityDataManager>().UnRegisterEntity(this);

            //Stop enemy movement
            Rb.velocity = Vector2.zero;

            //Remove tracking
            OnLeavingMap();

            //Change animation state if death is not forced (as when a siege is ending)
            if (forcedDeath) { FinishEnemyDeath(); return; }
            stateManager.ChangeCurrentState(EntityAnimation.Die);
        }

        public virtual void OnDeathAnimationFinished()
        {
            HandleDrops();
            StartCoroutine(ServiceLocator.Get<GameAnimations>().FadeOutAndDie(GetComponent<SpriteRenderer>(), fadeOutDuration, FinishEnemyDeath));
        }

        protected void HandleDrops()
        {
            Drops drops = ServiceLocator.Get<DropHandler>().CreateDrops(data.iD);

            ServiceLocator.Get<WorldSpaceUI>().CreateValueTextObjects(drops.xpAmount, drops.currencyType, drops.currencyAmount, transform.position);

            ServiceLocator.Get<LevelHandler>().GiveExperience(drops.xpAmount);
            ServiceLocator.Get<CurrencyHandler>().AddCurrency(drops.currencyType, drops.currencyAmount);

            if (drops.item != null)
            {
                ServiceLocator.Get<WorldSpaceUI>().CreateItemDrop(drops.item, drops.itemAmount, Pivot);
            }

            MetricController.EnemyDropMetrics(drops);
        }

        protected virtual void FinishEnemyDeath() => gameObject.SetActive(false);

        #endregion

        #region Location / Tracking

        public virtual void OnEnteringMap()
        {
            ServiceLocator.Get<EntityTracker>().AddTrackableEntity(this);
            SetState(States.inMap, true);
            ToggleCollisionColliders(true);

        }

        public virtual void OnLeavingMap()
        {
            ServiceLocator.Get<EntityTracker>().RemoveEntity(this);
            SetState(States.inMap, false);
            ToggleCollisionColliders(false);
        }

        #endregion

    }

    public interface IConfused
    {
        void OnConfused();
        void IsConfused();
        void RemoveConfused();
    }

    public interface IFeared
    {
        void OnFeared();
        void IsFeared();
    }

    public enum NPEntityCollider
    {
        Entity,
        Body,
        Feet
    }

    public interface INonPlayerPosition : IEntityPosition
    {
        public Collider2D GetCollider(NPEntityCollider collider);
    }

}
