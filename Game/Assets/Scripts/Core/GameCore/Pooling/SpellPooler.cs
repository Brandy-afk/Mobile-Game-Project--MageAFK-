
using System.Collections.Generic;
using UnityEngine;
using MageAFK.Spells;
using MageAFK.Management;
using MageAFK.Core;

namespace MageAFK.Pooling
{
  /// <summary>
  /// The SpellPooler class is responsible for pooling and managing spell objects in the game.
  /// It creates a pool of spell GameObjects for efficient instantiation and reuse.
  /// It also supports pooling different types of spells.
  /// </summary>
  public class SpellPooler : AbstractPooler
  {
    [SerializeField] private Transform parent;
    [SerializeField] private GameObject spellEffectPrefab;

    /// <summary>
    /// (Personal ID, Parent ID) ex: Fireball, None
    /// </summary>
    private readonly Dictionary<(SpellIdentification, SpellIdentification), (GameObject, List<GameObject>)> currentPool = new();
    protected override void RegisterSelf() => ServiceLocator.RegisterService(this);
    protected override void OnWaveStateChanged(WaveState state)
    {
      if (state == WaveState.Wave)
        Pool();
      else if (state == WaveState.Intermission)
        Clear();
    }

    /// <summary>
    /// Create a pool of spell objects based on the provided list of spells.
    /// </summary>
    /// <param name="spells">List of spells to pool.</param>
    /// 
    protected override void Pool()
    {
      //Pool effects
      PoolHelper(SpellIdentification.SpellUtility_Effect, spellEffectPrefab, null);
      // Loop through each spell
      foreach (Spell spell in ServiceLocator.Get<SpellCastHandler>().ReturnPoolableSpells())
      {
        // If the spell is not already in the dictionary, add it
        PoolHelper(spell.iD, spell.prefab, spell);

        if (spell.spellsToPool != null)
        {
          foreach (var extraSpell in spell.spellsToPool)
          {
            PoolHelper(extraSpell.iD, extraSpell.prefab, spell, spell.iD);
          }
        }
      }
    }

    private void PoolHelper(SpellIdentification iD, GameObject prefab, Spell spell, SpellIdentification parentID = SpellIdentification.None)
    {
      if (!currentPool.ContainsKey((iD, parentID)) && prefab)
      {
        currentPool[(iD, parentID)] = (prefab, new List<GameObject>());

        GameObject item = Instantiate(prefab, parent.transform);
        item.SetActive(false);
        currentPool[(iD, parentID)].Item2.Add(item);
      }
    }


    /// <summary>
    /// Retrieve a spell object from the pool based on the spell ID.
    /// </summary>
    /// <param name="iD">The ID of the spell to retrieve.</param>
    /// <returns>A GameObject representing the spell if one is available; otherwise, a new spell object is created.</returns>
    public GameObject Get(SpellIdentification iD, SpellIdentification parentID = SpellIdentification.None)
    {
      // If this spell is not in the pool, return null
      if (!currentPool.ContainsKey((iD, parentID))) return null;

      // Find the first inactive object in the pool
      foreach (var obj in currentPool[(iD, parentID)].Item2)
      {
        if (!obj.activeInHierarchy)
        {
          return obj;
        }
      }

      // If no inactive object is available, you can choose to either return null or create a new object and add it to the pool
      GameObject newItem = Instantiate(currentPool[(iD, parentID)].Item1, parent);
      newItem.SetActive(false);
      currentPool[(iD, parentID)].Item2.Add(newItem);
      return newItem;
    }




    /// <summary>
    /// Cleans up the current spell pool by destroying all GameObjects and clearing the collections.
    /// </summary>
    protected override void Clear()
    {
      foreach (var spellPool in currentPool)
      {
        foreach (var spell in spellPool.Value.Item2)
        {
          Destroy(spell);
        }
      }
      currentPool.Clear();
    }
  }

  [System.Serializable]
  public class PoolSpellClass
  {
    public SpellIdentification iD;
    public GameObject prefab;

    public PoolSpellClass(SpellIdentification iD, GameObject prefab)
    {
      this.iD = iD;
      this.prefab = prefab;
    }
  }

}