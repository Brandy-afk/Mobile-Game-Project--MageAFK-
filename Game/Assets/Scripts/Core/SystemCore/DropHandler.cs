using System;
using System.Collections.Generic;
using System.Linq;
using MageAFK.AI;
using MageAFK.Items;
using MageAFK.Management;
using MageAFK.Tools;
using Sirenix.OdinInspector;
using UnityEngine;


namespace MageAFK.Core
{
  public class DropHandler : SerializedMonoBehaviour
  {

    [Header("Value fields")]
    [SerializeField] private Dictionary<MobGrade, MinMax> baseGold;
    [SerializeField] private Dictionary<MobGrade, int> baseXP;
    [SerializeField] private Dictionary<MobGrade, MinMax> baseGem;

    [Header("Chance for gems to drop - ex: 30 -> 30%")]
    [SerializeField] private float gemChance;

    [Header("Chance for high quality item pool - ex: 30 -> 30%"), InfoBox("i 0 = low, i 1 = med, i 2 = high (Must add to 100)")]
    [SerializeField] private float[] qualityChance = new float[3];

    [SerializeField, Tooltip("Specific for Enemies - Decides the chance of rolling a pool of items. Ex. 20 -> 20%")]
    private Dictionary<MobGrade, List<(ItemGrade, float)>> gradePoolChance;



    private void OnValidate()
    {
      foreach (MobGrade grade in Enum.GetValues(typeof(MobGrade)))
      {
        try
        {
          gradePoolChance[grade] = gradePoolChance[grade].OrderBy(info => info.Item2).ToList();
        }
        catch (KeyNotFoundException)
        {
          continue;
        }
      }
    }


    [Header("Drop tables - DO NOT EDIT")]

    [SerializeField, ShowInInspector, ReadOnly] private Dictionary<ItemGrade, List<DropRange>[]> generic;
    [SerializeField, ShowInInspector, ReadOnly] private Dictionary<Location, Dictionary<ItemGrade, List<DropRange>[]>> location;
    [SerializeField, ShowInInspector, ReadOnly] private Dictionary<EnemyRace, Dictionary<ItemGrade, List<DropRange>[]>> race;
    [SerializeField, ShowInInspector, ReadOnly] private Dictionary<EntityIdentification, Dictionary<ItemGrade, List<DropRange>[]>> specific;
    private void Awake() => ServiceLocator.RegisterService(this);

    #region Set up
    public void ResetDropTables()
    {
      generic = new Dictionary<ItemGrade, List<DropRange>[]>();
      location = new Dictionary<Location, Dictionary<ItemGrade, List<DropRange>[]>>();
      race = new Dictionary<EnemyRace, Dictionary<ItemGrade, List<DropRange>[]>>();
      specific = new Dictionary<EntityIdentification, Dictionary<ItemGrade, List<DropRange>[]>>();
    }

    public void AddItemToDropTable(ItemData item)
    {
      var dropInfo = item.dropInfo;
      dropInfo.range.iD = item.iD;
      // Local function to reduce repetition
      void EnsureAndAdd<T1, T2>(Dictionary<T1, Dictionary<T2, List<DropRange>[]>> dict, T1 key1, T2 key2, DropRange value)
      {
        if (!dict.ContainsKey(key1))
          dict[key1] = new Dictionary<T2, List<DropRange>[]>();

        if (!dict[key1].ContainsKey(key2))
          dict[key1][key2] = new List<DropRange>[3] { new(), new(), new() };

        AddBasedOnQuality(dict[key1][key2][0], dict[key1][key2][1], dict[key1][key2][2], value);
      }

      void AddBasedOnQuality(List<DropRange> low, List<DropRange> med, List<DropRange> high, DropRange value)
      {
        switch (dropInfo.quality)
        {
          case DropQualityPool.Low:
            AddToCollection(low, value);
            break;
          case DropQualityPool.Medium:
            AddToCollection(med, value);
            break;
          case DropQualityPool.High:
            AddToCollection(high, value);
            break;
          case DropQualityPool.All:
            AddToCollection(low, value);
            AddToCollection(med, value);
            AddToCollection(high, value);
            break;
        }
      }

      void AddToCollection(List<DropRange> ranges, DropRange value) => ranges.Add(value);

      if (dropInfo.location != Location.All && dropInfo.location != Location.None)
        EnsureAndAdd(location, dropInfo.location, item.grade, dropInfo.range);

      if (dropInfo.location == Location.All)
      {
        if (!generic.ContainsKey(item.grade)) generic[item.grade] = new List<DropRange>[3] { new(), new(), new() };
        AddBasedOnQuality(generic[item.grade][0], generic[item.grade][1], generic[item.grade][2], dropInfo.range);
      }

      if (dropInfo.race != EnemyRace.None)
        EnsureAndAdd(race, dropInfo.race, item.grade, dropInfo.range);

      if (dropInfo.specificMobDropTables != null && dropInfo.specificMobDropTables.Count > 0)
      {
        foreach (var pair in dropInfo.specificMobDropTables)
        {
          dropInfo.range.iD = item.iD;
          EnsureAndAdd(specific, pair.Key, item.grade, dropInfo.range);
        }
      }
    }


    #endregion
    public Drops CreateDrops(EntityIdentification mobID)
    {
      Mob enemy = ServiceLocator.Get<EntityHandler>().GetMob(mobID);

      DropsBuilder builder = new();

      GetXPAmount(enemy, builder);
      GetCurrency(enemy, builder);
      RollForItems(enemy, builder);

      return builder.Build();
    }
    private void GetXPAmount(Mob mob, DropsBuilder builder)
    {
      int xpAmount = Mathf.FloorToInt(baseXP[mob.grade] + (baseXP[mob.grade] * ServiceLocator.Get<ScalingHandler>().ReturnXPScaler()));
      builder.WithXpAmount(xpAmount);
    }
    private void GetCurrency(Mob mob, DropsBuilder builder)
    {
      float roll = UnityEngine.Random.Range(0f, 100f);
      bool isSilver = roll > gemChance;
      var values = isSilver ? baseGold[mob.grade] : baseGem[mob.grade];
      var scaler = isSilver ? ServiceLocator.Get<ScalingHandler>().ReturnSilverScaler() : ServiceLocator.Get<ScalingHandler>().ReturnGemScaler();

      int amount = UnityEngine.Random.Range(values.min, values.max);
      amount = Mathf.FloorToInt(amount + (amount * scaler));

      builder.WithCurrencyType(isSilver ? CurrencyType.SilverCoins : CurrencyType.DemonicGems);
      builder.WithCurrencyAmount(amount);
    }

    //Testing
    int id = 0;
    private void RollForItems(Mob mob, DropsBuilder builder)
    {
      id++;
      if (mob is not null && RollAndReturn(mob.grade, out ItemGrade grade))
      {
        var qualityIndex = Utility.RollCumulativeChances(qualityChance, iD: id);
        Debug.Log($"Drops - ID{id} - {qualityIndex} (0 - low , 1 - med, 2 - high)");
        Debug.Log($"Drops - QualityIndex - {qualityIndex} (0 - low , 1 - med, 2 - high)");
        List<DropRange> drops = CreateDropList(mob, grade, qualityIndex);
        if (drops.Count > 0)
        {
          DropRange selectedDrop = drops[UnityEngine.Random.Range(0, drops.Count)];
          int amount = UnityEngine.Random.Range(selectedDrop.min, selectedDrop.max);
          builder.WithItem(ServiceLocator.Get<IItemGetter>().ReturnItemData(selectedDrop.iD));
          builder.WithItemAmount(amount);
        }
      }
    }
    private bool RollAndReturn(MobGrade mobGrade, out ItemGrade grade)
    {
      // Calculate a random number between 0 and 100

      var maxRange = ServiceLocator.Get<ScalingHandler>().ReturnItemScaler();
      var index = Utility.RollCumulativeChances
        (gradePoolChance[mobGrade].Select(chance => chance.Item2).ToArray(), maxRange, id);

      grade = index == -1 ? ItemGrade.None : gradePoolChance[mobGrade][index].Item1;

      Debug.Log($"Drops - ID{id} -grade- {grade}");
      Debug.Log($"Drops -DROPGRADE - {grade}");
      return grade != ItemGrade.None;
    }
    private List<DropRange> CreateDropList(Mob mob, ItemGrade gradePool, int qualityIndex)
    {
      List<DropRange> drops = new List<DropRange>(generic[gradePool][qualityIndex]);

      void TryAddPool<T1>(Dictionary<T1, Dictionary<ItemGrade, List<DropRange>[]>> dict, T1 key1)
      {
        if (dict.ContainsKey(key1) && dict[key1].ContainsKey(gradePool))
          drops.AddRange(dict[key1][gradePool][qualityIndex]);
      }

      TryAddPool(location, LocationHandler.currentLocation);
      TryAddPool(race, mob.race);
      TryAddPool(specific, mob.iD);

      return drops;
    }

  }

  public class Drops
  {
    public int xpAmount;
    public CurrencyType currencyType;
    public int currencyAmount;
    public ItemData item;
    public int itemAmount;

    public Drops(int xpAmount, CurrencyType type, int currencyAmount, ItemData item = null, int itemAmount = 0)
    {
      this.xpAmount = xpAmount;
      this.currencyType = type;
      this.currencyAmount = currencyAmount;
      this.item = item;
      this.itemAmount = itemAmount;

    }

  }

  public class DropsBuilder
  {
    private int _xpAmount;
    private CurrencyType _currencyType;
    private int _currencyAmount;
    private ItemData _item;
    private int _itemAmount;

    public DropsBuilder WithXpAmount(int xpAmount)
    {
      _xpAmount = xpAmount;
      return this;
    }

    public DropsBuilder WithCurrencyType(CurrencyType currencyType)
    {
      _currencyType = currencyType;
      return this;
    }

    public DropsBuilder WithCurrencyAmount(int currencyAmount)
    {
      _currencyAmount = currencyAmount;
      return this;
    }

    public DropsBuilder WithItem(ItemData item)
    {
      _item = item;
      return this;
    }

    public DropsBuilder WithItemAmount(int itemAmount)
    {
      _itemAmount = itemAmount;
      return this;
    }

    public Drops Build()
    {
      return new Drops(_xpAmount, _currencyType, _currencyAmount, _item, _itemAmount);
    }
  }



  [Serializable]
  public struct MinMax
  {
    public int min;
    public int max;
    public MinMax(int min, int max)
    {
      this.min = min;
      this.max = max;
    }
  }

  [Serializable]
  public struct FloatMinMax
  {
    public float min;
    public float max;
    public FloatMinMax(float min, float max)
    {
      this.min = min;
      this.max = max;
    }
  }

  [Serializable]
  public struct DropRange
  {
    [ReadOnly] public ItemIdentification iD;
    public int min;
    public int max;

    public DropRange(ItemIdentification iD, int min = 1, int max = 1)
    {
      this.min = min;
      this.max = max;
      this.iD = iD;
    }
  }

  public enum DropQualityPool
  {
    Low = 0,
    Medium = 1,
    High = 2,
    All = 3
  }
}
