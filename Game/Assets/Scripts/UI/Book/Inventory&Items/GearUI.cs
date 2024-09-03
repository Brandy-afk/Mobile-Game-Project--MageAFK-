using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MageAFK.Items;
using MageAFK.Management;
using Sirenix.OdinInspector;

namespace MageAFK.UI
{
  public class GearUI : SerializedMonoBehaviour
  {
    [SerializeField] private Dictionary<ItemType, GearTabButton> buttons;
    [SerializeField] private Sprite chosenSprite;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Image statsButton;
    private GearHandler gearHandler;
    public bool StatState { get; private set; } = false;
    private void Awake()
    {
      gearHandler = ServiceLocator.Get<GearHandler>();
      gearHandler.SetGearUI(this);
      IDragZoneCreator<Item> creator = ServiceLocator.Get<IDragZoneCreator<Item>>();
      foreach (var pair in buttons)
        creator.AddDragZone(pair.Value.transform.parent.GetComponent<RectTransform>(),
                                         (bool state) => OnHovering(pair.Key, state),
                                         () => gearHandler.TryEquippingGear(pair.Key),
                                         DragZoneIndentifier.Gear);
    }
    private void OnEnable()
    {
      StatState = false;
      AlterStatsText();
    }
    private void OnHovering(ItemType type, bool state)
    {
      try
      {
        buttons[type].ItemHovering(state, type);
      }
      catch (KeyNotFoundException)
      {
        Debug.LogWarning($"Key not found : {type}");
      }
    }
    public void UpdateUI(ItemType type, Item item)
    {
      if (buttons.TryGetValue(type, out GearTabButton gearTab))
        gearTab.SetUp(item);
    }

    #region Interaction

    public void OnStatsButtonPressed()
    {
      StatState = !StatState;
      AlterStatsText();
    }

    private void AlterStatsText()
    {
      statsButton.sprite = StatState ? chosenSprite : defaultSprite;
      foreach (var button in buttons.Values)
        button.ToggleUI();
    }

    #endregion

  }
}