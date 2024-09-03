using System.Collections.Generic;
using MageAFK.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MageAFK.Items
{
    [CreateAssetMenu(fileName = "DropInfo", menuName = "Items/Item Drop Settings", order = 0)]
  public class DropInformation : SerializedScriptableObject
  {
    [BoxGroup("Drop Settings"), Tooltip("Location.All is a generic, Location.None means only custom pools, and other will add to custom location pool")]
    public Location location = Location.All;

    [BoxGroup("Drop Settings"), Tooltip("For defining a specific race. If is none, it will not add to any race list.")]
    public EnemyRace race = EnemyRace.None;

    [ShowIf("ReturnIfSpecific"), BoxGroup("Drop Settings"), Tooltip("Is resource or animal")]
    public bool isResource;

    [BoxGroup("Drop Settings"), Tooltip("Specific Entity Drops -> Animals and resources should be set up through here.")]
    public Dictionary<EntityIdentification, DropRange> specificMobDropTables;

    [BoxGroup("Drop Settings"), Tooltip("A custom representation used in defining how it drops -> used when there are a lot of specific enemies.")]
    public string custom = null;

    [BoxGroup("Values"), HideIf("ReturnIfSpecific")]
    public DropRange range = new DropRange();

    [BoxGroup("Values"), InfoBox("Decides the quality pool - Small chance for high")]
    public DropQualityPool quality;



    private bool ReturnIfNotAnyLocations() => location == Location.None;

    private bool ReturnIfSpecific() => race == EnemyRace.None && location == Location.None;


  }
}
