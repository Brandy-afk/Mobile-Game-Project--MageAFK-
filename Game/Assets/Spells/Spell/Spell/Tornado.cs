using MageAFK.Tools;
using UnityEngine;

namespace MageAFK.Spells
{

    [CreateAssetMenu(fileName = "Tornado", menuName = "Spells/Tornado")]
  public class Tornado : Spell
  {
    public static int count = 0;
    public override void Activate()
    {
      SpellSpawn(iD, Utility.GetRandomMapPosition()).GetComponent<TornadoProjectile>();
    }
  }
}
