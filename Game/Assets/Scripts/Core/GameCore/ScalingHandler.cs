using MageAFK.Player;
using UnityEngine;
using MageAFK.Stats;
using MageAFK.Management;

namespace MageAFK.Core
{
  public class ScalingHandler : MonoBehaviour
  {
    [Header("scaler - (enemyScaler (ex .2) * wavecount(2) * value + value")]
    [SerializeField] private float enemyScaler;
    [SerializeField] private float xpScaler;
    [SerializeField] private float goldScaler;
    [SerializeField] private float gemScaler;

    [Header("100 - (itemDropScaler * wavecount)")]
    [SerializeField] private float itemDropScaler;
    [SerializeField] private float itemDespawnScaler;

    private void Awake() => ServiceLocator.RegisterService(this);

    public float ReturnEnemyScalerMultiplier() => (WaveHandler.Wave - 1) * enemyScaler;

    public float ReturnItemScaler() => 100 - ((WaveHandler.Wave - 1) * itemDropScaler);

    // may need modification ( all modifiers added up like .3 + .2 + .1 = .6 so 60%) 
    public float ReturnXPScaler()
    {
      return (xpScaler * (WaveHandler.Wave - 1))
      + ServiceLocator.Get<PlayerStatHandler>().ReturnModification(Stat.ExperienceDrops)
      + ServiceLocator.Get<LocationHandler>().ReturnXPModifier(); //Have location modifiers just tune into playerstathandler
    }

    public float ReturnSilverScaler()
    {
      return (goldScaler * (WaveHandler.Wave - 1)) +
     ServiceLocator.Get<PlayerStatHandler>().ReturnModification(Stat.SilverDrops) +
      ServiceLocator.Get<LocationHandler>().ReturnCurrencyModifier();
    }

    public float ReturnGemScaler()
    {
      return (gemScaler * (WaveHandler.Wave - 1)) +
      ServiceLocator.Get<PlayerStatHandler>().ReturnModification(Stat.GemDrops) +
      ServiceLocator.Get<LocationHandler>().ReturnCurrencyModifier();
    }

    public float ReturnItemDespawnScaler()
    {
      return itemDespawnScaler * (WaveHandler.Wave - 1);
    }
  }

}