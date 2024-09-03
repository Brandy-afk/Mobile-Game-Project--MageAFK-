using MageAFK.Core;
using MageAFK.Management;
using UnityEngine;

namespace MageAFK.Pooling
{
  public abstract class AbstractPooler : MonoBehaviour
  {

    protected virtual void Awake()
    {
      RegisterSelf();
      WaveHandler.SubToWaveState(OnWaveStateChanged, true, Priority.First);
      WaveHandler.SubToSiegeEvent((Status status) =>
      {
        if (status == Status.End_CleanUp)
        {
          Clear();
        }
      }, true);
    }

    protected abstract void RegisterSelf();

    protected virtual void OnWaveStateChanged(WaveState state) { }
    protected virtual void Create() { }
    protected abstract void Pool();
    protected abstract void Clear();

  }

}