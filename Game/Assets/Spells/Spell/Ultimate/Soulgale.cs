
using MageAFK.Stats;
using UnityEngine;

namespace MageAFK.Spells
{

  [CreateAssetMenu(fileName = "Voidgale", menuName = "Spells/Voidgale")]
  public class Soulgale : Ultimate, IPlacableUlt
  {
    public override void Activate()
    {
      uses = (int)ReturnStatValue(Stat.SpawnCap);
    }

    public int OnPlaced(Vector2 pos)
    {
      SpellSpawn(iD, pos);
      return --uses;
    }

  }
}
