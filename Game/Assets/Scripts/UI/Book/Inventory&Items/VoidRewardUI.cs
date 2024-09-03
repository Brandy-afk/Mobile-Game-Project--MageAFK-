
using System;
using System.Collections.Generic;
using MageAFK.Animation;
using MageAFK.Core;
using MageAFK.Items;
using MageAFK.Management;
using MageAFK.Player;
using MageAFK.Tools;
using MageAFK.UI;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MageAFK
{
    public class VoidRewardUI : SerializedMonoBehaviour
    {
        [SerializeField, TabGroup("Variables")] private GameObject allRewards, oneObjectPanel;
        [SerializeField, TabGroup("Variables")] private Image chestButton, rewardImage, itemImage;
        [SerializeField, TabGroup("Variables")] private TMP_Text rewardsLeft, title, rewardName, rewardAmount;
        [SerializeField, TabGroup("Variables")] private Dictionary<RewardType, (TMP_Text, GameObject)> rewardOV;

        [SerializeField, TabGroup("Sprites")] public Sprite closedChest, openedChest;
        [SerializeField, TabGroup("Sprites")] public Dictionary<RewardType, Sprite> sprites;
        [SerializeField, TabGroup("References")] private ItemPanelUI itemPanelUI;
        [SerializeField, TabGroup("References")] private RecipeUIController recipeUI;

        public event Action OnClose;

        private List<Reward> rewards;
        private int currentIndex;

        private void OnEnable() => ServiceLocator.Get<IPlayerDeath>().SubscribeToPlayerDeath(ClosePanel, true);
        private void OnDisable() => ServiceLocator.Get<IPlayerDeath>().SubscribeToPlayerDeath(ClosePanel, false);

        #region Interaction
        public void OpenPanel(List<Reward> rewards, VoidUI.SystemStage stage, bool isFail)
        {
            this.rewards = rewards;
            SetUp(stage, isFail);
            UIAnimations.Instance.OpenPanel(gameObject);
        }
        public void ClosePanel() => UIAnimations.Instance.ClosePanel(gameObject, () =>
                                             {
                                                 OnClose?.Invoke();
                                                 OnClose = null;
                                                 rewards = null;
                                             });

        public void NextItemPressed()
        {
            //Initial press
            if (currentIndex == -1)
                ToggleRewardObjects(true);

            //If current index is the last reward
            if (currentIndex == rewards.Count - 1)
                AllRewardsToUI();
            //Continue one to the next reward
            else
            {
                currentIndex++;
                UpdateChestButton(rewards.Count - (currentIndex + 1));
                ShowNextReward(rewards[currentIndex]);
            }
        }

        public void OnRewardImagePressed()
        {
            var reward = rewards[currentIndex];
            if (reward.rewardType == RewardType.Items)
            {
                var itemReward = reward as ItemReward;
                itemPanelUI.SetUpAndOpen(itemReward.item.iD, itemReward.level);
            }
            else if (reward.rewardType == RewardType.Recipe)
            {
                var recipeReward = reward as RecipeReward;
                recipeUI.OpenRecipePanel(recipeReward.recipe);
            }
        }

        #endregion

        #region UI

        private void TogglePanels(bool isAll)
        {
            allRewards.SetActive(isAll);
            oneObjectPanel.SetActive(!isAll);
        }

        #region Single Reward
        private void SetUp(VoidUI.SystemStage stage, bool isFail)
        {
            title.text = stage == VoidUI.SystemStage.Upgrade ? isFail ? "Failure" : "Success"
                      : "Rewards";

            currentIndex = -1;
            ToggleRewardObjects(false);
            itemImage.gameObject.SetActive(false);
            TogglePanels(false);
            UpdateChestButton(rewards.Count);
        }

        private void ShowNextReward(Reward reward)
        {
            if (reward.rewardType == RewardType.Items)
                InputItemReward(reward as ItemReward);
            else if (reward.rewardType == RewardType.Recipe)
                InputRecipeReward(reward as RecipeReward);
            else
                InputValueReward(reward.rewardType, reward.amount);

            reward.GiveReward();
        }

        #region Input
        private void InputItemReward(ItemReward reward)
        {
            itemImage.gameObject.SetActive(true);
            itemImage.sprite = reward.item.image;
            rewardImage.sprite = ServiceLocator.Get<IItemGradeUIProvider>().GetSlotSprite(reward.item.grade,
                                                                                          InventorySpriteType.Void,
                                                                                          reward.level);
            rewardName.text = reward.item.itemName;
            rewardAmount.text = $"x{reward.amount}";
        }

        private void InputRecipeReward(RecipeReward recipeReward)
        {
            itemImage.gameObject.SetActive(false);
            rewardImage.sprite = sprites[RewardType.Recipe];
            rewardAmount.text = "Void Recipe";
            rewardName.text = recipeReward.recipe.output.itemName;
        }

        private void InputValueReward(RewardType type, int amount)
        {
            itemImage.gameObject.SetActive(false);
            rewardImage.sprite = sprites[type];
            rewardName.text = type.ToString();
            rewardAmount.text = StringManipulation.FormatShortHandNumber(amount);
        }
        #endregion

        #region Toggles
        private void ToggleRewardObjects(bool state)
        {
            rewardName.gameObject.SetActive(state);
            rewardAmount.gameObject.SetActive(state);
            rewardImage.gameObject.SetActive(state);
        }

        private void UpdateChestButton(int count)
        {
            rewardsLeft.text = count.ToString();
            chestButton.sprite = count > 0 ? closedChest : openedChest;
        }
        #endregion

        #endregion

        #region All Rewards

        private void AllRewardsToUI()
        {
            Dictionary<RewardType, int> amounts = new Dictionary<RewardType, int>();
            TallyAmounts(amounts);
            InputAmount(amounts);
            TogglePanels(true);
        }

        private void TallyAmounts(Dictionary<RewardType, int> amounts)
        {
            for (int i = 0; i < rewards.Count; i++)
            {
                amounts.TryAdd(rewards[i].rewardType, 0);
                amounts[rewards[i].rewardType] += rewards[i].ReturnAmount();
            }
        }

        private void InputAmount(Dictionary<RewardType, int> amounts)
        {
            foreach (var type in rewardOV.Keys)
            {
                if (amounts.TryGetValue(type, out int amount))
                {
                    rewardOV[type].Item2.gameObject.SetActive(true);
                    var isValue = type != RewardType.Items && type != RewardType.Recipe;
                    rewardOV[type].Item1.text = isValue ? StringManipulation.FormatShortHandNumber(amount)
                    : $"x{amount}";
                }
                else
                    rewardOV[type].Item2.gameObject.SetActive(false);
            }
        }

        #endregion

        #endregion

    }
}
