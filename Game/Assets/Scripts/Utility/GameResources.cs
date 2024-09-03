using System.Collections.Generic;
using MageAFK.AI;
using MageAFK.Spells;
using UnityEngine;

namespace MageAFK.Tools
{
    public class GameResources : MonoBehaviour
    {
        [SerializeField] private SpellReferences spellRef;
        public static SpellReferences Spell;
        [SerializeField] private EntityReferences entityRef;
        public static EntityReferences Entity;
        private static readonly Dictionary<ProjectLayerMask, (int index, int mask)> cachedLayers = new();
        

        private void Awake()
        {
            Spell = spellRef;
            Entity = entityRef;

            spellRef = null;
            entityRef = null;
        }

        /// <summary>
        /// Get mask from cached dict.
        /// </summary>
        /// <param name="layer"></param>
        /// <returns>Returns index for setting layer, and mask for utilizing physics.</returns>
        public static (int index, int mask) GetLayerMask(ProjectLayerMask layer)
        {
            try
            {
                return cachedLayers[layer];
            }
            catch (KeyNotFoundException)
            {
                var mask = LayerMask.GetMask(layer.ToString());
                var index = LayerMask.NameToLayer(layer.ToString());
                if (mask != -1)
                {
                    cachedLayers[layer] = (index, mask);
                    return GetLayerMask(layer);
                }
                else
                {
                    Debug.Log("Did not find layer");
                    return default;
                }


            }
        }
    }

    public enum ProjectLayerMask
    {
        NonPlayerBodies,
        NonPlayerFeet,
        ConfusedBodyProjectile,
        ConfusedFootProjectile,
        NonPlayerBodyProjectile,
        NonPlayerFootProjectile
    }
}
