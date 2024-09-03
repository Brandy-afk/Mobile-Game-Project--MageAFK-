using System.Collections.Generic;
using System.Linq;
using MageAFK.Items;
using MageAFK.Management;
using MageAFK.TimeDate;
using MageAFK.UI;
using UnityEngine;

namespace MageAFK
{
    public class OrderUI : MonoBehaviour
    {

        [SerializeField] private GameObject orderContentParent;
        [SerializeField] private GameObject orderPrefab;


        [Header("References")]

        [SerializeField] private ItemPanelUI itemPanelUI;
        [SerializeField] private RecipeUIController recipeController;

        private Dictionary<int, OrderUIController> currentOrders = new();
        private List<OrderUIController> availablePanels = new();


        private void Awake()
        {
            OrderUIController.InputOrderUI(this);
            var craftingHandler = ServiceLocator.Get<CraftingHandler>();
            craftingHandler.InputOrderUI(this);
        }

        #region Filter Objects

        public void OrganizeOrderUI()
        {
            var timeTaskHandler = ServiceLocator.Get<TimeTaskHandler>();
            var keyArray = currentOrders.OrderBy(pair => timeTaskHandler.ReturnTimeLeft(pair.Key)).Select(pair => pair.Key).ToList();

            for (int i = 0; i < keyArray.Count; i++)
                currentOrders[keyArray[i]].transform.SetSiblingIndex(i);
        }


        #endregion

        #region OrderOperations
        public void AddWorkOrder(Order order)
        {
            OrderUIController slot = availablePanels.Count > 0 ? availablePanels[0] : CreateWorkOrderObject();
            currentOrders.Add(order.key, slot);
            slot.SetOrderData(order, false);
            availablePanels.Remove(slot);

            OrganizeOrderUI();
        }

        private OrderUIController CreateWorkOrderObject()
        {
            var uiController = Instantiate(orderPrefab, orderContentParent.transform).GetComponent<OrderUIController>();
            availablePanels.Add(uiController);
            uiController.gameObject.SetActive(false);
            return uiController;
        }


        public void UpdateWorkOrderUI(int timeKey)
        {
            if (!currentOrders.ContainsKey(timeKey)) return;
            currentOrders[timeKey].UpdateTimeUI();
        }

        public void OnOrderFinishedUI(int timeKey) => currentOrders[timeKey].OnFinish();

        public void OnCollectOrCancel(int timeKey)
        {
            currentOrders[timeKey].SetOrderData(null, true);
            availablePanels.Add(currentOrders[timeKey]);
            currentOrders.Remove(timeKey);
        }

        #endregion

        #region Interactions

        public void InspectRecipe(Recipe recipe) => recipeController.OpenRecipePanel(recipe);
        public void InpsectItem
        (ItemIdentification iD, ItemLevel level, int timeKey) =>
        itemPanelUI.SetUpAndOpen(iD, level);

        #endregion
    }
}
