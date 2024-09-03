using System.Collections.Generic;
using System.Linq;
using MageAFK.Management;
using MageAFK.Player;
using MageAFK.Stats;
using MageAFK.UI;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

namespace MageAFK.Core
{
  public class PowerHandler : SerializedMonoBehaviour, IData<PowerData>
  {

    [SerializeField] private Dictionary<ElixirIdentification, Elixir> elixirs;

    // [SerializeField] private Elixir[] toAdd;
    [SerializeField, Tooltip("Base amount, can be upgraded\nRepresents the amount of elixirs in the shop.")] private int shopCount;
    private ShopElixir[] currentShop = null;

    // private void OnValidate()
    // {
    //   if (toAdd != null && toAdd.Length > 0)
    //   {
    //     foreach (var item in toAdd)
    //     {
    //       if (elixirs.ContainsKey(item.identification)) continue;
    //       elixirs.Add(item.identification, item);
    //     }
    //   }

    //   toAdd = null;
    // }


    [Header("References")]
    [SerializeField] private PowerUI powerUI;

    #region Lifecycle
    private void Awake()
    {
      ServiceLocator.RegisterService(this);
      ServiceLocator.RegisterService<IData<PowerData>>(this);
      WaveHandler.SubToSiegeEvent((Status state) =>
      {
        if (state == Status.Start && !GameManager.IsLoad)
          DrinkAllElixirs();
      }, true);
    }

    private void Start()
    {
      SaveManager.OnSaveDataLoaded += OnDataLoaded;
      WaveHandler.SubToSiegeEvent((Status state) =>
      {
        if (state == Status.End)
          CreateNewShop();
      }, true);
    }

    public void InitializeData(PowerData data) => currentShop = data.elixirs;
    public PowerData SaveData() => new(currentShop);

    #endregion

    public void OnDataLoaded()
    {
      if (currentShop == null)
        CreateNewShop();
      else
        UpdateShopUI(false);
    }

    public void CreateNewShop()
    {
      currentShop = new ShopElixir[shopCount];
      var iDs = elixirs.Keys.ToArray();
      for (int i = 0; i < shopCount; i++)
      {
        var randomID = iDs[Random.Range(0, iDs.Length)];
        var elixir = elixirs[randomID];
        currentShop[i] = elixir.CreateElixir();
      }

      UpdateShopUI(true);
    }

    private void UpdateShopUI(bool newShop)
    {
      powerUI.FillShopSlots(currentShop, newShop);

      if (newShop)
      {
        ServiceLocator.Get<IndicatorHandler>().SetUIChain(UIPanel.Town_Power);
        SaveManager.Save(SaveData(), DataType.ElixirData);
      }

    }

    public void DrinkAllElixirs()
    {
      foreach (var elixir in currentShop)
      {
        if (!elixir.isPurchased) { continue; }
        elixirs[elixir.iD].DrinkElixir(elixir.value);
        elixir.isPurchased = false;
      }
    }

    public Elixir ReturnElixir(ElixirIdentification iD)
    {
      try
      {
        return elixirs[iD];
      }
      catch (KeyNotFoundException)
      {
        Debug.Log($"Bad key - {iD}");
        return null;
      }
    }
  }

  [System.Serializable]
  public class PowerData
  {
    public ShopElixir[] elixirs;
    public PowerData(ShopElixir[] elixirs) => this.elixirs = elixirs;
  }

  public class ShopElixir
  {
    public ElixirIdentification iD;
    public float value;
    public int cost;
    public bool isPurchased = false;

    public ShopElixir(ElixirIdentification iD, float value, int cost)
    {
      this.iD = iD;
      this.value = value;
      this.cost = cost;
    }
  }


}