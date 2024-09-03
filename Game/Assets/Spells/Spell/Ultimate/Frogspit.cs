using MageAFK.Stats;
using UnityEngine;

namespace MageAFK.Spells
{

    [CreateAssetMenu(fileName = "Frogspit", menuName = "Spells/Frogspit")]
  public class Frogspit : Ultimate, IPlacableUlt
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
