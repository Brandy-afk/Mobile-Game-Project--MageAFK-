using MageAFK.AI;
using MageAFK.Tools;
using UnityEngine;

namespace MageAFK.Spells
{

  [CreateAssetMenu(fileName = "Growth", menuName = "Spells/Growth")]
  public class Growth : Spell
  {

    Camera mainCam;

    public override void Activate()
    {
      if (mainCam == null)
      {
        mainCam = Camera.main;
      }

      SpellSpawn(iD, Utility.GetRandomMapPosition()).GetComponent<GrowthProjectile>().intial = true;

    }

    public void SpawnGrowth(IEntityPosition target)
    {
      SpellSpawn(iD, target.Feet).GetComponent<GrowthProjectile>().intial = false;
    }


  }
}
