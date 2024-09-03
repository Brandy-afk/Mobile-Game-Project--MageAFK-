using System;
using System.Collections;
using System.Collections.Generic;
using MageAFK.Management;
using UnityEngine;

namespace MageAFK.TimeDate
{
  public class TimeTaskHandler : MonoBehaviour
  {
    [SerializeField] private int index = 0;
    private Dictionary<int, TimeTask> currentTimeTasks = new();

    private void Awake() => ServiceLocator.RegisterService(this);


    #region  TimeTasks
    public void AddTimer(Action callback, Action update, float duration, bool ignoreTimeScale = false)
    {
      TimeTask task = new(duration, callback, update, ignoreTimeScale);
      StartCoroutine(TrackTimer(task));

    }
    public void AddTimer(Action callback, Action update, float duration, int key, bool ignoreTimeScale = false)
    {
      currentTimeTasks[key] = new TimeTask(duration, callback, update, ignoreTimeScale, key);
      currentTimeTasks[key].routine = StartCoroutine(TrackTimer(currentTimeTasks[key]));
    }

    public IEnumerator TrackTimer(TimeTask task)
    {
      while (task.duration > 0)
      {
        task.duration -= task.ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
        task.updateCallBack?.Invoke();

        yield return null;
      }

      task.finishedCallBack?.Invoke();
      currentTimeTasks.Remove(task.key);
    }

    public int GetUniqueTimeKey()
    {
      return index++;
    }

    public string GetTimeLeftString(int key, float largerFont, float smallerFont, bool notKey = false, float minuteValue = 0, string imageStr = "")
    {
      if (currentTimeTasks.ContainsKey(key) || notKey)
      {
        float timeLeft = notKey ? minuteValue : currentTimeTasks[key].duration;
        // Define the different time formats
        string timeFormat = $"<size={largerFont}>{{2}}{{0}}</size><size={smallerFont}>{{1}}</size>";

        int minutes = (int)timeLeft / 60;
        int seconds = (int)timeLeft % 60;

        string timeString = "";

        // If there are minutes, display minutes and seconds
        if (minutes > 0)
        {
          timeString = string.Format(timeFormat, minutes, "M", imageStr);
          if (seconds > 0) // Only add seconds if there are any
          {
            timeString += " " + string.Format(timeFormat, seconds, "S", "");
          }
        }
        // If there are only seconds, display seconds
        else if (seconds > 0)
        {
          timeString = string.Format(timeFormat, seconds, "S", imageStr);
        }
        else
        {
          timeString = "Done";
        }

        return timeString;
      }
      else
      {
        Debug.LogWarning($"No time task found with key: {key}");
        return " ";
      }
    }


    public int ReturnGemCostToComplete(int key)
    {
      if (currentTimeTasks.ContainsKey(key))
      {
        float timeLeft = currentTimeTasks[key].duration;
        float totalMinutes = timeLeft / 60; // TotalMinutes property gives the total number of minutes in the timespan
                                            // Multiplying totalMinutes with 0.5 to get the cost. 
                                            // We are rounding the result to the nearest integer value since gem cost should be an integer.
        int cost = Math.Max(5, (int)Math.Round(totalMinutes * 10));

        return cost;
      }
      else
      {
        Debug.LogWarning($"No time task found with key: {key}");
        return 0;
      }
    }

    public void EndTask(int key, bool isCancel)
    {

      if (currentTimeTasks.ContainsKey(key))
      {
        if (isCancel is false) { currentTimeTasks[key].finishedCallBack?.Invoke(); }
        StopCoroutine(currentTimeTasks[key].routine);
        currentTimeTasks.Remove(key);
      }
      else
      {
        Debug.Log($"No time task found with key: {key}");
      }
    }

    public float ReturnTimeLeft(int key)
    {
      if (currentTimeTasks.TryGetValue(key, out TimeTask task))
      {
        return task.duration;
      }
      else
      {
        Debug.Log($"Bad Key for time return");
        return 0;
      }
    }

    public TimeTask ReturnTimeTask(int key)
    {
      if (currentTimeTasks.ContainsKey(key))
      {
        return currentTimeTasks[key];
      }

      Debug.Log($"Bad Key : {key}");
      return null;
    }

    #endregion

  }
}