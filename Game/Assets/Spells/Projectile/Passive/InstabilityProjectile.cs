
using UnityEngine;
using MageAFK.AI;
using MageAFK.Combat;
using MageAFK.Tools;

namespace MageAFK.Spells
{

  public class InstabilityProjectile : SpellProjectile
  {
    [SerializeField, Tooltip("Decides what kind of tags will cause the trap to proc.")] protected Tags[] trapProcTargets;
    private bool active = false;

    private void OnEnable() => active = false;

    public void SetActive() => active = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
      if (active && Utility.VerifyTags(trapProcTargets, other))
      {
        InitialDisable();

        if (Utility.VerifyTags(targetTags, other))
        {
          NPEntity entity = other.GetComponentInParent<NPEntity>();
          if (entity == null) return;
          _ = base.HandleDamage(entity);
        }
      }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
      if (active)
        OnTriggerEnter2D(other);
    }

    public void InitialDisable()
    {
      (spell as Instability).Dequeue(this);
      active = false;
      GetComponent<Animator>().Play("Hit");
    }

    public override void Disable() => gameObject.SetActive(false);


  }

}
