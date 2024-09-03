
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MageAFK.AI;
using MageAFK.Spells;
using Unity.VisualScripting;
using UnityEngine;


namespace MageAFK.Combat
{
    public abstract class StatusHandler : MonoBehaviour
  {
    protected Entity entity; // Reference to the object that contains stats

    public Dictionary<StatusType, Dictionary<(OrginType, int), StatusEffect>> activeEffects { get; private set; }
      = new Dictionary<StatusType, Dictionary<(OrginType, int), StatusEffect>>();

    protected void OnUpdate()
    {
      if (activeEffects == null || activeEffects.Count < 1) return;

      List<(StatusType, OrginType, int)> effectsToRemove = new();
      foreach (KeyValuePair<StatusType, Dictionary<(OrginType, int), StatusEffect>> entry in activeEffects)
      {
        if (entry.Value.Count <= 0) continue;
        // Create a list to store the effects that have expired
        foreach (KeyValuePair<(OrginType, int), StatusEffect> effectEntry in entry.Value)
        {
          StatusEffect effect = effectEntry.Value;
          if (!effect.decrement) continue; // Means it will be removed by a spell specifically

          effect.duration -= Time.deltaTime; // Decrement the duration of the effect

          if (effect.duration <= 0)
          {
            effectsToRemove.Add((entry.Key, effectEntry.Key.Item1, effectEntry.Key.Item2)); // Mark effect for removal
          }
        }

        // Remove all effects that have been marked for removal
      }

      if (effectsToRemove.Count > 0)
      {
        foreach (var pair in effectsToRemove)
        {
          TryRemoveEffect(pair.Item1, pair.Item2, pair.Item3);
        }
        UpdateVisualDisplays();
      }
    }


    private IEnumerator ApplyStatusEffect(StatusType type, OrginType orgin, int iD, float waitTime)
    {
      while (activeEffects[type].ContainsKey((orgin, iD)))
      {
        activeEffects[type][(orgin, iD)].Apply(entity);
        yield return new WaitForSeconds(waitTime);
      }
    }


    /// <summary>
    /// Create an effect on the entity.
    /// </summary>
    /// <param name="source">Player, enemy, skill/other</param>
    /// <param name="status"></param>
    /// <param name="magnitude">Pass 0 if not applicable</param>
    /// <param name="duration">Pass 0 if not applicable</param>
    /// <param name="iD"></param>
    public void CreateEffect(OrginType source, StatusType status, float magnitude, float duration = 0, int iD = 0)
    {
      if (ReturnImmunities().Contains(status)) return;
      if (activeEffects.ContainsKey(status) && activeEffects[status].ContainsKey((source, iD)))
      {
        activeEffects[status][(source, iD)].AddEffect(activeEffects, entity, false);
      }
      else
      {
        if (!activeEffects.ContainsKey(status)) activeEffects[status] = new Dictionary<(OrginType, int), StatusEffect>();
        StatusEffect effect = StatusFactory.CreateStatus(status, source, magnitude, duration, iD);
        bool isApplied = effect.AddEffect(activeEffects, entity, true);
        ApplyNewEffect(isApplied, effect, status);
      }

    }

    protected void ApplyNewEffect(bool isApplied, StatusEffect effect, StatusType type)
    {
      if (!isApplied)
      {
        if (effect.waitTime != -1) //If never altered it wont be updated, For instance if it is changing a bool
        {
          StartCoroutine(ApplyStatusEffect(type, effect.source, effect.iD, effect.waitTime));
        }
        else
        {
          effect.Apply(entity);
          if (type == StatusType.Smite)
            effect = null;
        }
      }

      UpdateVisualDisplays();
    }

    #region Helpers
    public Dictionary<(OrginType, int), StatusEffect> ReturnEffects(StatusType type)
      => activeEffects != null && activeEffects.ContainsKey(type) ? activeEffects[type] : null;

    public bool CheckIfEffect(StatusType type)
      => activeEffects != null && activeEffects.ContainsKey(type) && activeEffects[type].Count > 0;

    public void TryRemoveEffect(StatusType type, OrginType source, int iD)
    {
      if (activeEffects != null && activeEffects.ContainsKey(type) && activeEffects[type].TryGetValue((source, iD), out StatusEffect effect))
      {
        activeEffects[type].Remove((source, iD));
        effect.Remove(entity, activeEffects);
        UpdateVisualDisplays();
      }
    }
    #endregion

    private void OnDisable()
    {
      if (activeEffects == null) return;
      List<(StatusType type, OrginType orgin, int key)> list = new();

      foreach (var status in activeEffects.Keys)
      {
        foreach (var key in activeEffects[status].Keys)
        {
          list.Add((status, key.Item1, key.Item2));
        }
      }

      for (int i = 0; i < list.Count; i++)
        TryRemoveEffect(list[i].type, list[i].orgin, list[i].key);

    }

    public abstract void UpdateVisualDisplays();

    public abstract StatusType[] ReturnImmunities();
  }
}
