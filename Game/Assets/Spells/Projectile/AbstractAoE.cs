using MageAFK.AI;
using MageAFK.Stats;
using MageAFK.Tools;
using UnityEngine;

namespace MageAFK.Spells
{
    public abstract class AbstractAoE : SpellProjectile
    {
        [SerializeField, Tooltip("Offset of gizmo.")] protected Vector2 gizmoOffset;
        [SerializeField] protected LayerCollision mask = LayerCollision.Body;
        [SerializeField, Tooltip("Affected by area of effect damage stat.")] protected bool areaOfEffectDamage;
        [SerializeField] protected bool forceCrit;
        [SerializeField] protected bool forcePierce;
        [SerializeField] protected bool forceStatus;


        public virtual void DoDamage(Collider2D[] colliders)
        {
            var damage = areaOfEffectDamage ? spell.ReturnStatValue(Stat.Damage, false) * (spell.ReturnStatValue(Stat.AreaOfEffectDamage) / 100)
                                                       : spell.ReturnStatValue(Stat.Damage, false);


            foreach (var col in colliders)
            {
                if (!Utility.VerifyTags(targetTags, col)) continue;
                NPEntity entity = col.GetComponentInParent<NPEntity>();
                if (entity == null)
                {
                    Debug.Log($"Issue concerning this object : {gameObject.name}");
                    continue;
                }
                HandleDamage(entity, forceCrit, forceStatus, forcePierce, damage);
            }
        }
    }
}
