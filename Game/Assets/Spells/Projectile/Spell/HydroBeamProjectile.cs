
using UnityEngine;
using MageAFK.AI;
using MageAFK.Stats;
using System.Collections.Generic;
using MageAFK.Tools;
using MageAFK.Combat;


namespace MageAFK.Spells
{

  public class HydroBeamProjectile : SpellProjectile
  {

    [SerializeField, Tooltip("Enemies who will take damage from the spell, others will be knocked back (targetTags)")] Tags[] damageTargets;
    private readonly Dictionary<GameObject, float> lastDamageTime = new();
    private const float damageInterval = 0.15f;
    private Animator animator;
    private float spawnTime;
    private float duration;
    private bool active = false;

    private void Awake()
    {
      animator = GetComponent<Animator>();
    }

    public void SetActive()
    {
      duration = spell.ReturnStatValue(Stat.SpellDuration);
      spawnTime = Time.time;
      active = true;
      CapsuleCollider2D cap = GetComponent<CapsuleCollider2D>();

      Collider2D[] colliders = Physics2D.OverlapCapsuleAll(cap.transform.position, cap.size, CapsuleDirection2D.Horizontal,
                                cap.transform.eulerAngles.z, ReturnMask(LayerCollision.Body));
      foreach (var col in colliders)
        OnTriggerStay2D(col);

    }

    private void Update()
    {
      if (active && Time.time > (spawnTime + duration))
      {
        active = false;
        if (animator != null)
        {
          animator?.Play("End");
        }
      }
      else
      {
        (spell as HydroBeam).OnUpdate(gameObject);
      }

    }

    private void OnTriggerStay2D(Collider2D other)
    {
      if (!active || !Utility.VerifyTags(targetTags, other)) return;

      NPEntity entity = other.GetComponentInParent<NPEntity>();
      // Check if it's time to apply damage again
      HandleColHelper(entity);
    }

    private void HandleColHelper(NPEntity entity)
    {
      if (entity == null) return;
      if (!lastDamageTime.TryGetValue(entity.gameObject, out float lastTime) || Time.time - lastTime >= damageInterval)
      {
        Rigidbody2D entityBody = entity.GetComponentInParent<Rigidbody2D>();

        Vector2 direction = (entityBody.transform.position - transform.position).normalized;
        entityBody.AddForce(direction * spell.ReturnStatValue(Stat.KnockBack), ForceMode2D.Impulse);

        lastDamageTime[entity.gameObject] = Time.time;

        Spell.SpawnEffect(entity.transform.position, SpellEffectAnimation.Water_1);

        if (Utility.VerifyTags(damageTargets, entity))
          HandleDamage(entity);
      }
    }

    public override void Disable()
    {
      gameObject.SetActive(false);
      lastDamageTime.Clear();
    }



  }

}
