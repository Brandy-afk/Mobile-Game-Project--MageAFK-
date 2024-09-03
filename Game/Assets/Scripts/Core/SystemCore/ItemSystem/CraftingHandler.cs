using System;
using System.Collections.Generic;
using System.Linq;
using MageAFK.Core;
using MageAFK.Management;
using MageAFK.Player;
using MageAFK.Stats;
using MageAFK.TimeDate;
using MageAFK.UI;
using UnityEngine;

namespace MageAFK.Items
{
  public class CraftingHandler : IData<int>
  {
    public event Action OnOrdersChanged;
    public int maxOrderCount { get; private set; } = 2;
    public readonly Dictionary<int, Order> currentOrders = new();

    private OrderUI orderUI;

    public CraftingHandler()
    {
      WaveHandler.SubToSiegeEvent((Status status) =>
     {
       if (status == Status.End_CleanUp)
       {
         EndAllOrders();
       }
     }, true);
    }


    #region Load/Save

    public OrderDataCollection SaveTempData()
    {
      return new OrderDataCollection(currentOrders.Values.ToList());
    }
    public void InitializeTempData(OrderDataCollection data)
    {
      if (data.orders == null) return;

      var timeTaskHandler = ServiceLocator.Get<TimeTaskHandler>();
      var recipeHandler = ServiceLocator.Get<RecipeHandler>();

      foreach (OrderData order in data.orders)
        CreateOrder(recipeHandler.ReturnRecipe(order.recipeID), order.quantity, timeTaskHandler.GetUniqueTimeKey(), order.timeLeft);
    }

    public void InitializeData(int data) => maxOrderCount = data;
    public int SaveData() => maxOrderCount;

    #endregion

    //Event 
    #region Events

    public void SubscribeToOrderEvent(Action callback, bool state)
    {
      if (state)
      {
        OnOrdersChanged += callback;
        InvokeOrdersChanged();
      }
      else
      {
        OnOrdersChanged -= callback;
      }
    }

    private void InvokeOrdersChanged() => OnOrdersChanged?.Invoke();
    public string ReturnOrderString(bool fullString) => fullString ? $"{currentOrders.Count}/{maxOrderCount} Orders Active" : $"{currentOrders.Count}/{maxOrderCount}";

    #endregion

    public void InputOrderUI(OrderUI orderUI)
    {
      this.orderUI = orderUI;
      foreach (var order in currentOrders.Values)
        orderUI.AddWorkOrder(order);
    }



    #region Order Creation
    public void InitiateOrder(Recipe recipe, int multiplier)
    {
      //Pay for order
      PayForOrder(recipe, multiplier);
      // Create a time task for the recipe
      int uniqueTimeKey = ServiceLocator.Get<TimeTaskHandler>().GetUniqueTimeKey();
      CreateOrder(recipe,
                  multiplier,
                  uniqueTimeKey,
                  ServiceLocator.Get<PlayerStatHandler>().ReturnModifiedValue(Stat.CraftingTime,
                                                                              recipe.TimeToBuild * multiplier));
    }

    private void PayForOrder(Recipe recipe, int multiplier)
    {
      var inventoryHandler = ServiceLocator.Get<InventoryHandler>();
      // Deduct ingredients from the inventory
      foreach (Ingredient ingredient in recipe.ingredients)
        inventoryHandler.RemoveItem((ingredient.item.iD, ingredient.level), GetReducedItemAmount(ingredient.quantity, multiplier));

      // Take currency
      ServiceLocator.Get<CurrencyHandler>().SubtractCurrency(CurrencyType.SilverCoins, GetReducedCost(recipe.Cost, multiplier));
    }

    private void CreateOrder(Recipe recipe, int multiplier, int timeKey, float seconds)
    {
      Order newOrder = new(recipe, multiplier, timeKey);
      currentOrders.Add(timeKey, newOrder);

      if (orderUI != null)
        orderUI.AddWorkOrder(newOrder);

      ServiceLocator.Get<TimeTaskHandler>().AddTimer(() => FinishCraftingOrder(timeKey), () => UpdateOrder(timeKey), seconds, timeKey);
      InvokeOrdersChanged();
    }

    public bool CanCraft(Recipe recipe, int multiplier)
    {
      // Check if all ingredients are available in the inventory
      var inventoryHandler = ServiceLocator.Get<InventoryHandler>();

      for (int i = 0; i < recipe.ingredients.Length; i++)
      {
        var ingredient = recipe.ingredients[i];
        if (inventoryHandler.ReturnItemAmount((ingredient.item.iD, ingredient.level)) < GetReducedItemAmount(recipe.ingredients[i].quantity, multiplier))
          return false;
      }

      return ServiceLocator.Get<CurrencyHandler>().ReturnAffordable(CurrencyType.SilverCoins, GetReducedCost(recipe.Cost, multiplier)) && !CheckIfOrdersMaxed();
    }

    #endregion

    #region Order Manipulation

    public void UpdateOrder(int timeKey)
    {
      if (!currentOrders.ContainsKey(timeKey) || orderUI == null || !orderUI.gameObject.activeInHierarchy) return;
      orderUI.UpdateWorkOrderUI(timeKey);
    }

    private void EndAllOrders()
    {
      foreach (var order in currentOrders)
      {
        EndOrderEarly(order.Value, true);
      }
    }

    public void EndOrderEarly(Order order, bool isCancel)
    {
      ServiceLocator.Get<TimeTaskHandler>().EndTask(order.key, isCancel);

      if (isCancel)
        OnCollectOrCancel(order.key);
    }


    public void FinishCraftingOrder(int timeKey)
    {
      if (currentOrders.ContainsKey(timeKey))
      {
        ServiceLocator.Get<IndicatorHandler>().SetUIChain(UIPanel.Book_Inventory_Crafting_Orders);
        if (orderUI != null)
          orderUI.OnOrderFinishedUI(timeKey);
      }
      else
      {
        Debug.Log("Not an active order.");
      }
    }

    public void OnCollectOrCancel(int timeKey)
    {
      // Remove the time key and recipe index from the dictionary
      currentOrders.Remove(timeKey);
      InvokeOrdersChanged();

      if (orderUI != null)
        orderUI.OnCollectOrCancel(timeKey);
    }

    #endregion

    #region Helpers
    public int GetReducedCost(int cost, int multiplier) =>
    (int)ServiceLocator.Get<PlayerStatHandler>().ReturnModifiedValue(Stat.CraftingCost, cost * multiplier);
    public int GetReducedItemAmount(int itemsPerCraft, int multiplier) =>
    Math.Max(1, (int)ServiceLocator.Get<PlayerStatHandler>().ReturnModifiedValue(Stat.CraftingItemCostMod, itemsPerCraft * multiplier));

    public void AlterWorkOrderSize(int increase)
    {
      maxOrderCount += increase;
      InvokeOrdersChanged();

      if (WaveHandler.WaveState == WaveState.None)
        SaveManager.Save(SaveData(), DataType.CraftingData);
    }

    public bool CheckIfOrdersMaxed() => currentOrders.Count >= maxOrderCount;
    #endregion
  }



  [Serializable]
  public class OrderDataCollection
  {
    public List<OrderData> orders = new List<OrderData>();

    public OrderDataCollection(List<Order> currentOrders)
    {
      if (currentOrders == null) return;
      foreach (var order in currentOrders)
      {
        orders.Add(new OrderData(order.recipe.ReturnID(), order.multiplier, ServiceLocator.Get<TimeTaskHandler>().ReturnTimeLeft(order.key)));
      }
    }
  }

  [Serializable]
  public class OrderData
  {
    public ItemIdentification recipeID;
    public int quantity;

    //Seconds
    public float timeLeft;


    public OrderData(ItemIdentification info, int quantity, float secondsLeft)
    {
      recipeID = info;
      this.quantity = quantity;
      timeLeft = secondsLeft;
    }
  }

  public class Order
  {

    public Recipe recipe;

    public int multiplier;

    public int key;

    public Order(Recipe recipe, int amount, int key)
    {
      this.recipe = recipe;
      multiplier = amount;
      this.key = key;

    }

  }

}
