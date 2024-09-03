using MageAFK.AI;
using MageAFK.Items;
using MageAFK.Management;
using MageAFK.Tools;
using UnityEngine;

public class FoolsCap_Explosion : AbstractDamager
{
  [SerializeField] protected float radius = 1.0f;
  [SerializeField] protected Vector2 offset;

  private void Awake()
  {
    var cap = ServiceLocator.Get<GearHandler>().ReturnItem(ItemType.Headgear) as FoolsCap;
    damage = cap.damage;
  }

  private void OnDrawGizmos()
  {
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere((Vector2)transform.position + offset, radius);
  }

  public void OnExplosion()
  {
    Collider2D[] colliders = Physics2D.OverlapCircleAll((Vector2)transform.position + offset, radius, GameResources.GetLayerMask(ProjectLayerMask.NonPlayerBodies).mask);
    for (int i = 0; i < colliders.Length; i++)
      DoDamage(colliders[i].GetComponent<NPEntity>());
  }

  public void Disable() => gameObject.SetActive(false);


}