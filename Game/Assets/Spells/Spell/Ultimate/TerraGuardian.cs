
using MageAFK.Animation;
using MageAFK.Management;
using MageAFK.Stats;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MageAFK.Spells
{

  [CreateAssetMenu(fileName = "TerraGuardian", menuName = "Spells/TerraGuardian")]
  public class TerraGuardian : Ultimate, IPlacableUlt
  {
    [TabGroup("Ultimate"), SerializeField] private Material mat;

    public override void Activate()
    {
      uses = (int)ReturnStatValue(Stat.SpawnCap);
    }

    public int OnPlaced(Vector2 pos)
    {
      var instance = SpellSpawn(iD, pos);
      ServiceLocator.Get<GameAnimations>().DissolveAnimation(instance.GetComponent<SpriteRenderer>(), mat);
      instance.GetComponent<TerraGuardianEntity>().OnEnabled();
      AppendRecord(SpellRecordID.GuardiansSummoned, 1);
      return --uses;
    }

    public void SpawnMine(Vector2 pos)
    {
      SpellSpawn(SpellIdentification.TerraGuardian_Mine, pos, true, SpellIdentification.TerraGuardian);
      AppendRecord(SpellRecordID.MinesSet, 1);
    }

    public override void OnCast() { }
  }
}
