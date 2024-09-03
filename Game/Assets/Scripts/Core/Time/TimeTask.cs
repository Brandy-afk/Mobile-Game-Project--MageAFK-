using System;
using UnityEngine;

namespace MageAFK.TimeDate
{
  public class TimeTask
  {
    public float duration;
    public float startingDuration;
    public Action finishedCallBack;
    public Action updateCallBack;
    public int key;
    public Coroutine routine;

    public bool ignoreTimeScale;

    public TimeTask(float duration, Action finishedCallBack, Action updateCallBack, bool ignoreTimeScale, int key = -1)
    {
      this.duration = duration;
      this.finishedCallBack = finishedCallBack;
      this.updateCallBack = updateCallBack;
      startingDuration = duration;
      this.key = key;
      this.ignoreTimeScale = ignoreTimeScale;
    }
  }


}
