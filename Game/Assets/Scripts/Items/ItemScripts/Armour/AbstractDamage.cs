using MageAFK.AI;
using MageAFK.Management;
using MageAFK.Player;
using MageAFK.Stats;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MageAFK.Items
{
    public abstract class AbstractDamager : MonoBehaviour
  {
    [SerializeField, InfoBox("These modifiers will be applied to damage")] protected Stat[] mods = new Stat[] { Stat.Damage };
    [ShowInInspector] protected float damage;

    public float DoDamage(NPEntity entity)
    {
      if (entity == null) return 0;

      float totalMod = 0;
      var statHandler = ServiceLocator.Get<PlayerStatHandler>();
      foreach (var mod in mods)
        totalMod += statHandler.ReturnModification(mod);

      var sumDamage = (damage * (1 + totalMod)) - entity.runtimeStats[Stat.Armour];

      var values = entity.DoDamage(sumDamage);
      return values.Item1 ? values.Item2 : 0;
    }
  }
}