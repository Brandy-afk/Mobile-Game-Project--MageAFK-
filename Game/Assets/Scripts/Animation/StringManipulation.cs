using System;
using System.Text;
using System.Text.RegularExpressions;
using MageAFK.Items;

namespace MageAFK.Tools
{
  public class StringManipulation
  {


    public static string FormatShortHandNumber(float num)
    {
      if (num >= 1000000)
        return (num / 1000000.0).ToString("0.0") + "m";
      else if (num >= 1000)
        return (num / 1000.0).ToString("0.0") + "k";
      else
        return num.ToString();
    }

    public static string FormatShortHandNumber(int num)
    {
      return FormatShortHandNumber((float)num);
    }


    public static string FormatTimeForm(float num)
    {
      // Convert input to TimeSpan
      TimeSpan time = TimeSpan.FromMinutes(num);

      // Check if time is more than a day
      if (time.TotalDays >= 1)
      {
        return $"{time.TotalDays:F1} Day(s)";
      }
      // Check if time is more than an hour
      else if (time.TotalHours >= 1)
      {
        return $"{time.TotalHours:F1} Hour(s)";
      }
      // If it's less than an hour, return in minutes
      else if (time.TotalMinutes >= 1)
      {
        return $"{Math.Ceiling(time.TotalMinutes)} Minute(s)";
      }
      else
      {
        return $"{time.TotalSeconds:F1} Second(s)";
      }
    }

    public static string AddSpacesBeforeCapitals(string input)
    {
      return Regex.Replace(input, "(?<!^)([A-Z])", " $1");
    }

    //Needs to be decimal
    public static string FormatStatNumberValue(float value, bool isPercentage, string decimalSymbol, bool createSign = true, string symbol = "")
    {
      symbol = isPercentage ? "%" : symbol;
      string sign = createSign ? (value < 0) ? "-" : "+" : "";
      string amountStr = Math.Abs(value).ToString(decimalSymbol);
      return $"{sign}{amountStr}{symbol}";
    }

    public static string CreateTraitString(ItemStatTrait[] traits, string boldHex, ItemLevel itemLevel = ItemLevel.None)
    {
      var format = $"<color=#{{2}}>{{0}}</color> {{1}}\n";

      StringBuilder stringBuilder = new();

      foreach (var statTrait in traits)
        stringBuilder.Append(string.Format(format, statTrait.PrintValue(itemLevel), statTrait.PrintTrait(), boldHex));

      return stringBuilder.ToString();
    }




  }
}