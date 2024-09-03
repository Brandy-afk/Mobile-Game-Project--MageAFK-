
using MageAFK.AI;
using MageAFK.Player;
using MageAFK.Stats;
using MageAFK.Tools;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MageAFK.Spells
{

  [CreateAssetMenu(fileName = "DemonicShield", menuName = "Spells/DemonicShield")]
  public class DemonicShield : Spell
  {
    [BoxGroup("Demonic Shield")]
    public float radius;
    [BoxGroup("Demonic Shield")]
    public float angularSpeed;


    public override void Activate()
    {
      SpellSpawn(iD, new Vector2(PlayerController.Positions.Pivot.x + radius, PlayerController.Positions.Pivot.y));
    }

    public void Retaliate(NPEntity entity, Transform spawn)
    {
      GameObject instance = SpellSpawn(SpellIdentification.DemonicShield_Fireball, spawn.position, true, iD);
      SpawnEffect(spawn.position, SpellEffectAnimation.FireHitEffect_1);

      var direction = Utility.SetRotation(instance, entity.Pivot, ReturnStatValue(Stat.AimVariance));
      Utility.SetVelocity(direction, instance, ReturnStatValue(Stat.SpellSpeed));
    }
  }
}
