
using MageAFK.Core;
using MageAFK.Management;
using MageAFK.Player;
using MageAFK.Stats;
using MageAFK.Tools;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MageAFK
{
    [CreateAssetMenu(fileName = "New Stat Elixir", menuName = "Elixir/Stat Elixir")]
    public class StatElixir : Elixir
    {

        [SerializeField, BoxGroup("StatElixir")] private Stat stat;
        [SerializeField, BoxGroup("StatElixir")] private bool isPercentage = true;
        [SerializeField, Tooltip("Whole values e.g. 10 -> 10% (If negative, make negative)"), BoxGroup("StatElixir")] private FloatMinMax values;

        public override string CreateDesc(float value, float cost, bool isBuyable)
        {
            string format = $"{{0}} {{1}} by <color=#{{2}}>{{3}}{{4}}</color> next wave.";
            var desc = string.Format(format,
                        values.min < 0 ? "Decrease" : "Increase",
                        ServiceLocator.Get<StatInformation>().ReturnStatInformation(stat).statName.ToLower(),
                        ColorUtility.ToHtmlStringRGB(titleColor),
                        Mathf.Abs(value).ToString("N0"),
                        isPercentage ? "%" : "");

            return desc;
        }

        public override ShopElixir CreateElixir()
        {
            var randomValue = Mathf.Round(Random.Range(values.min, values.max));
            return new ShopElixir(identification, randomValue, GetCost(randomValue));
        }

        protected override int GetCost(float value)
        {
            var ratio = value / values.min;
            return (int)Mathf.Round(baseCost * ratio);
        }

        public override void DrinkElixir(float value)
        {
            value = isPercentage ? value / 100 : value;
            ServiceLocator.Get<PlayerStatHandler>().ModifyStat(stat, value, false);
        }

        public override string FormatValue(float value) => StringManipulation.FormatStatNumberValue(value, isPercentage, "N0");

    }
}
