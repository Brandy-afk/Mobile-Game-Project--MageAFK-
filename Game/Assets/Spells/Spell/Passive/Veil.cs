using MageAFK.Player;
using UnityEngine;

namespace MageAFK.Spells
{

  [CreateAssetMenu(fileName = "Veil", menuName = "Spells/Veil")]
  public class Veil : Spell
  {
    public override void Activate()
    {
      var playerPos = PlayerController.Positions.Pivot;
      SpellSpawn(iD, new Vector2(playerPos.x, playerPos.y - .02f));
    }

  }
}
