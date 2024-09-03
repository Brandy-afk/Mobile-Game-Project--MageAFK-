
using MageAFK.Stats;
using UnityEngine;

namespace MageAFK.Spells
{

  [CreateAssetMenu(fileName = "NaturesFury", menuName = "Spells/NaturesFury")]
  public class NaturesFury : Ultimate, IPlacableUlt
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
