
using MageAFK.Items;
using MageAFK.Management;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using MageAFK.Tools;
using MageAFK.Core;


namespace MageAFK.UI
{
  public class ItemPanelUI : WavePopUp
  {

    [SerializeField] private TMP_Text itemName, descText, dropText, dropTitle, itemAmount, specifcType, customTitle, customDesc, levelText;
    [SerializeField] private Image itemImage, typeImage, levelImage;
    [SerializeField] private Sprite[] levelSprites;
    [SerializeField] private InventoryUI inventoryUI;

    private (ItemIdentification, ItemLevel) currentKey = new(ItemIdentification.None, ItemLevel.None);


    protected override void OnEnable()
    {
      base.OnEnable();
      ServiceLocator.Get<InventoryHandler>().InventoryAltered += OnSlotAlteredHandler;
      OnSlotAlteredHandler();
    }

    protected override void OnDisable()
    {
      base.OnEnable();
      ServiceLocator.Get<InventoryHandler>().InventoryAltered -= OnSlotAlteredHandler;
    }

    #region UI
    private void OnSlotAlteredHandler() => itemAmount.text = $"x{ServiceLocator.Get<InventoryHandler>().ReturnItemAmount(currentKey)}";

    public void SetUpAndOpen(ItemIdentification iD, ItemLevel level)
    {
      FillUI(iD, level);
      Open();
    }

    private void FillUI(ItemIdentification iD, ItemLevel level)
    {

      if (currentKey.Item1 == iD && currentKey.Item2 == level)
        return;


      var itemData = ServiceLocator.Get<IItemGetter>().ReturnItemData(iD);
      if (currentKey.Item1 == iD && currentKey.Item2 != level)
      {
        //Updating a change in level
        SetUpSpecficUI(itemData, level);
        SetUpLevelUI(itemData, level);
      }
      else
      {
        SetUpLevelUI(itemData, level);
        SetUpSpecficUI(itemData, level);
        SetUpUI(itemData);
      }

      CheckIfFound((iD, level));
      currentKey = (iD, level);
    }

    private void CheckIfFound((ItemIdentification, ItemLevel) key)
    {
      var foundItems = ServiceLocator.Get<InventoryHandler>().foundItems;
      if (!foundItems.Contains(key))
      {
        foundItems.Add(key);
        inventoryUI.RemoveIndicator(key);
      }
    }

    private void SetUpLevelUI(ItemData data, ItemLevel level)
    {
      var intLevel = (int)level;
      bool levelState = intLevel >= 0;
      levelText.text = levelState ? $"Level {intLevel}" : "";
      levelImage.gameObject.SetActive(levelState);
      if (levelState) levelImage.sprite = levelSprites[intLevel];

    }
    private void SetUpUI(ItemData data)
    {
      Color color = ServiceLocator.Get<IItemGradeUIProvider>().GetColor(data.grade);
      specifcType.color = Utility.AlterColor(color);
      specifcType.text = data.ReturnTypeDesc();
      typeImage.color = color;
      itemName.text = data.itemName;
      itemImage.sprite = data.image;
      descText.text = data.description;


      bool state = data.dropInfo != null;
      dropTitle.text = !state ? "Obtained from" : "Drops";
      dropText.text = !state ? "Item is craftable." : SetUpDropInfo(data, state);
    }

    private string SetUpDropInfo(ItemData data, bool state)
    {
      string format = $"<color=#38496F>Location:</color> {{0}}\n<color=#38496F>Type:</color> {{1}}\n<color=#38496F>Race:</color> {{2}}\n<color=#38496F>Specific:</color> {{3}}";

      var dropInfo = data.dropInfo;

      var location = dropInfo.location == Location.None ? "N/A" : dropInfo.location.ToString();
      var enemyType = dropInfo.location != Location.None ? "Enemy" : dropInfo.isResource ? "Resource" : "Animal";
      var raceType = dropInfo.race == EnemyRace.None ? "N/A" : dropInfo.race.ToString();
      var specifc = dropInfo.specificMobDropTables == null ? "None" : dropInfo.custom == null ? "???" : dropInfo.custom;

      return string.Format(format, location, enemyType, raceType, specifc);
    }



    private void SetUpSpecficUI(ItemData data, ItemLevel level)
    {
      customDesc.enableWordWrapping = true;
      switch (data)
      {
        case ArmourData:
          SetUpArmourUI(data as ArmourData, level);
          break;
        case ConsumableData:
          SetUpConsumableUI(data as ConsumableData);
          break;
        case UsableData:
          SetUpUsableUI(/*TBD*/);
          break;
        case FoodData:
          SetUpFoodUI(data as FoodData);
          break;
        default:
          customTitle.text = "";
          customDesc.text = "";
          break;
      }
    }

    private void SetUpArmourUI(ArmourData armourData, ItemLevel level)
    {
      customTitle.text = "On Equip";
      customDesc.text = armourData.ReturnArmourInfo(level, "38496F");
      customDesc.enableWordWrapping = armourData.WrapText;
    }

    private void SetUpConsumableUI(ConsumableData data)
    {
      customTitle.text = "Upon drinking";
      var customConsumableText = data.ReturnCustomDesc();
      customDesc.text = customConsumableText == null ? StringManipulation.CreateTraitString(data.traits, "38496F") : customConsumableText;
    }


    private void SetUpUsableUI()
    {
      customTitle.text = "On Use";
    }

    private void SetUpFoodUI(FoodData foodData)
    {
      customTitle.text = "On Consuming";
      customDesc.text = $"Heal for {foodData.healValue * 100}% of your max health.";
    }

    #endregion
  }
}

