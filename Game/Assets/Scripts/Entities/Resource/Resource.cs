using System.Collections.Generic;
using MageAFK.Stats;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MageAFK.AI
{

    public class Resource : NPEntity
  {
    [SerializeField, ReadOnly] public override Dictionary<Stat, float> runtimeStats { get; set; }
    public override Dictionary<States, bool> states { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

  }

}