using MageAFK.Spells;
using UnityEngine;

namespace MageAFK.Combat
{

  public class StatusFactory : MonoBehaviour
  {
    public static StatusEffect CreateStatus(StatusType status, OrginType type, float magnitude, float duration, int iD)
    {
      switch (status)
      {
        case StatusType.Slow:
          return new SlowEffect(duration, magnitude, iD, type);
        case StatusType.Burn:
          return new BurnEffect(duration, magnitude, iD, type);
        case StatusType.Stun:
          return new StunEffect(duration, iD, type);
        case StatusType.Corrupt:
          return new CorruptionEffect(duration, magnitude, iD, type);
        case StatusType.Confuse:
          return new ConfusionEffect(duration, iD, type);
        case StatusType.Bleed:
          return new BleedEffect(2, magnitude, iD, type);
        case StatusType.Weaken:
          return new WeakenEffect(duration, magnitude, iD, type);
        case StatusType.Root:
          return new RootEffect(duration, iD, type);
        case StatusType.Poison:
          return new PoisonEffect(duration, magnitude, iD, type);
        case StatusType.Fear:
          return new FearEffect(duration, iD, type);
        case StatusType.None:
          // Code to handle no effect
          return null;
        default:
          return null;
      }


    }


    public static StatusEffect CreateStatus(StatusBlueprint blueprint, int iD)
    {
      if (blueprint == null || blueprint.status == StatusType.None)
        return null;

      switch (blueprint.status)
      {
        case StatusType.Slow:
          return new SlowEffect(blueprint.duration, blueprint.magnitude, iD, OrginType.Other);
        case StatusType.Burn:
          return new BurnEffect(blueprint.duration, blueprint.magnitude, iD, OrginType.Other);
        case StatusType.Stun:
          return new StunEffect(blueprint.duration, iD, OrginType.Other);
        case StatusType.Corrupt:
          return new CorruptionEffect(blueprint.duration, blueprint.magnitude, iD, OrginType.Other);
        case StatusType.Confuse:
          return new ConfusionEffect(blueprint.duration, iD, OrginType.Other);
        case StatusType.Bleed:
          return new BleedEffect(blueprint.duration, blueprint.magnitude, iD, OrginType.Other);
        case StatusType.Weaken:
          return new WeakenEffect(blueprint.duration, blueprint.magnitude, iD, OrginType.Other);
        case StatusType.Root:
          return new RootEffect(blueprint.duration, iD, OrginType.Other);
        case StatusType.Poison:
          return new PoisonEffect(blueprint.duration, blueprint.magnitude, iD, OrginType.Other);
        case StatusType.None:
          // Code to handle no effect
          return null;
        default:
          return null;
      }
    }

  }

  [System.Serializable]
  //Used for hold data to create statuseffect based on this skills stats defined in the "EffectStats"
  public class StatusBlueprint
  {
    [Header("Should always be whole number, no decimal")]
    public float magnitude;
    public float duration;
    public bool isPercentage;
    public StatusType status;

    public StatusBlueprint()
    {

    }
  }

}