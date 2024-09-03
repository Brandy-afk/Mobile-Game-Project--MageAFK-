
using UnityEngine;
using MageAFK.AI;
using MageAFK.Combat;
using MageAFK.Stats;


namespace MageAFK.Spells
{

  public class SeaStrikesProjectile : SingleTargetController
  {

    private int strikeCount;
    [SerializeField] private Vector2 whipSize;
    [SerializeField] private Vector2 whipOffset;
    [SerializeField] private Vector2 punchSize;
    [SerializeField] private Vector2 punchOffset;

    public IEntityPosition entityPosition;

    protected override void OnDrawGizmos()
    {
      base.OnDrawGizmos();
      Gizmos.color = Color.red;
      Gizmos.DrawWireCube((Vector2)transform.position + offset, size);

      Gizmos.color = Color.green;
      Gizmos.DrawWireCube((Vector2)transform.position + whipOffset, whipSize);

      Gizmos.color = Color.blue;
      Gizmos.DrawWireCube((Vector2)transform.position + punchOffset, punchSize);
    }

    private void OnEnable()
    {
      strikeCount = 0;
    }

    public void FirstStrike()
    {
      if (CheckForCollider(offset, size))
      {
        HandleDamage(target.GetComponentInParent<NPEntity>());
      }
    }


    public void SecondStrike()
    {
      if (CheckForCollider(whipOffset, whipSize))
      {
        HandleDamage(target.GetComponentInParent<NPEntity>());
      }
    }

    public void ThirdStrike()
    {
      if (CheckForCollider(punchOffset, punchSize))
      {
        HandleDamage(target.GetComponentInParent<NPEntity>());
      }

      Disable();
    }

    protected override CollisionInformation HandleDamage(NPEntity entity, bool forceCrit = false, bool forceStatus = false, bool forcePierce = false, float baseDamage = .01f)
    {
      CollisionInformation information = SpellCollisionHandler.ReturnCollisionInformation(spell, entity, forceCrit, forceStatus, forcePierce, baseDamage);

      if (strikeCount == 1)
        information.damage += information.damage * (spell.ReturnStatValue(Stat.DamageIncrease, false) / 100);

      entity.DoDamage(information.damage, spell, information.textType);

      if (strikeCount == 2 && information.isStatusProc)
        CreateEffect(entity);


      return information;
    }


    public void OnAnimationFinished()
    {
      strikeCount++;

      if (strikeCount == 3 || !target.gameObject.activeSelf || !target.GetComponent<NPEntity>().states[States.inMap])
      {
        Disable();
      }
      else
      {
        transform.position = entityPosition.Body;
      }
    }

  }

}
