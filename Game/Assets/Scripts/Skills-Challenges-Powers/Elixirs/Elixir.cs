using System;
using System.Collections;
using System.Collections.Generic;
using MageAFK.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MageAFK.Core
{
    public abstract class Elixir : ScriptableObject
    {
        [BoxGroup("Elixir")] public ElixirIdentification identification;
        [BoxGroup("Elixir"), PreviewField] public Sprite sprite;
        [BoxGroup("Elixir")] public string title;
        [BoxGroup("Elixir")] public Color titleColor;
        [SerializeField, BoxGroup("Elixir")] protected int baseCost;

        // private void OnValidate()
        // {
        //     string objectName = name;
        //     var enums = Enum.GetValues(typeof(ElixirIdentification));
        //     foreach (ElixirIdentification iD in enums)
        //     {
        //         if (iD.ToString() == objectName)
        //             identification = iD;
        //     }
        // }


        public int BaseCost { get { return baseCost; } }


        /// <summary>
        /// Append the upgrade to the player.
        /// </summary>

        public abstract ShopElixir CreateElixir();
        public abstract void DrinkElixir(float value);
        public abstract string FormatValue(float value);

        protected abstract int GetCost(float value);
        public abstract string CreateDesc(float value, float cost, bool buyable);
    }

    public enum ElixirIdentification
    {
        None,
        Cooldown,
        CritChance,
        CritDamage,
        Damage,
        Health,
        HealthRegen,
        Silver,
        ThornsDamage,
        XP
    }
}
