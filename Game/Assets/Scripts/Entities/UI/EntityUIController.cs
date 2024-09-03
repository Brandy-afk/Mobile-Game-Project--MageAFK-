
using System.Collections.Generic;
using MageAFK.Combat;
using MageAFK.Management;
using MageAFK.Spells;
using MageAFK.Tools;
using UnityEngine;
using UnityEngine.UI;

namespace MageAFK.UI
{
    public class EntityUIController : MonoBehaviour
  {


    [SerializeField] private List<Image> images;
    [SerializeField] public Slider healthBar;
    [SerializeField] public Transform effects;
    [SerializeField] public CanvasGroup group;

    [SerializeField] private Image fill;
    [SerializeField] private Color enemyColor;



    //For tracking active effects to display
    private StatusHandler statusHandler;

    private Dictionary<StatusType, Image> activeSprites = new() { };

    private void OnEnable()
    {
      foreach (Image image in images)
      {
        image.gameObject.SetActive(false);
      }
      activeSprites.Clear();
    }

    public void SetStatusHandler(StatusHandler h)
    {
      statusHandler = h;
    }

    public void UpdateHealthBar(float amount, float maxHealth)
    {
      healthBar.maxValue = maxHealth;
      healthBar.value = amount;
    }

    public void SetUILocations(Vector2 healthBarLoc, Vector2 effectsLoc)
    {
      healthBar.transform.localPosition = healthBarLoc;
      effects.localPosition = effectsLoc;
    }




    public void UpdateStatusDisplay()
    {
      if (statusHandler.activeEffects == null) { return; }

      List<StatusType> toRemove = new();
      foreach (var status in statusHandler.activeEffects)
      {
        if (status.Value.Count > 0)
        {
          if (!activeSprites.ContainsKey(status.Key))
          {
            ToggleStatusDisplay(status.Key, true);
          }
        }
        else
        {
          toRemove.Add(status.Key);
        }
      }

      foreach (var pair in toRemove)
      {
        ToggleStatusDisplay(pair, false);
      }
    }


    private void ToggleStatusDisplay(StatusType key, bool activate)
    {
      if (activate)
      {
        foreach (Image image in images)
        {
          if (!image.gameObject.activeInHierarchy)
          {
            Sprite sprite = GameResources.Spell.ReturnStatusSprite(key);
            if (sprite == null) { Debug.Log("Null Sprite"); return; }
            image.gameObject.SetActive(true);
            image.sprite = sprite;
            activeSprites.Add(key, image);
            break; // Exit the loop once an image has been activated
          }
        }
      }
      else // Deactivate the effect display
      {
        if (!activeSprites.ContainsKey(key)) return;
        Image imageToRemove = activeSprites[key];
        if (imageToRemove != null)
          imageToRemove.gameObject.SetActive(false);
        activeSprites.Remove(key);
      }
    }

    public void OnDeath()
    {
      ServiceLocator.Get<WorldSpaceUI>().RemoveEnemyUIPair(gameObject, group);
    }

  }

}