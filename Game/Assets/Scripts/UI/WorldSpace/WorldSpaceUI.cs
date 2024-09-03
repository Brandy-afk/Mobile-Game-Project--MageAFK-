using UnityEngine;
using MageAFK.Pooling;
using MageAFK.Animation;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using MageAFK.Core;
using System.Collections;
using MageAFK.Items;
using MageAFK.Management;
using MageAFK.AI;

namespace MageAFK.UI
{
  public class WorldSpaceUI : MonoBehaviour
  {
    //For understanding when certain objects do not have to be created
    [SerializeField] private GameObject bookUIObject;

    [Header("Animation")]

    [Header("Health Bar A")]
    [SerializeField] private float enemyUIFadeTime;

    [Header("Damage Text A")]
    [SerializeField] private Vector2 dmgInitialScale;
    [SerializeField] private Vector2 dmgTargetScale;

    [SerializeField] private Vector3 dmgFloatAmount;
    [SerializeField] private float dmgLifetime;

    [Header("Value Text A")]
    [SerializeField] private Vector3 valueFloatAmount;
    [SerializeField] private float valueLifetime;
    [SerializeField] private float timeBetweenValues;

    [Header("Item Drop A")]
    [SerializeField] private Vector2 itemInitialScale;
    [SerializeField] private Vector2 itemTargetScale;


    [SerializeField] private float maxTargetX;
    [SerializeField] private float minTargetX;
    [SerializeField] private float dropTime;
    [SerializeField] private float itemDespawnTime;
    [SerializeField] private float blinkInterval = 0.5f;

    [Header("References")]

    [SerializeField] private WorldSpaceUIReferences worldSpaceUIReferences;
    [SerializeField] private ObjectPooler objectPooler;
    private IItemGradeUIProvider itemDataBase;




    private Dictionary<GameObject, IEntityPosition> enemyUIPairs;

    //Active items on the ground
    private List<ItemDrop> drops = new();

    private void Awake() => ServiceLocator.RegisterService<WorldSpaceUI>(this);
    private void Start()
    {
      WaveHandler.SubToWaveState(OnWaveStateChanged, true);
      itemDataBase = ServiceLocator.Get<IItemGradeUIProvider>();
    }

    private void Update()
    {
      if (enemyUIPairs != null && enemyUIPairs.Count > 0) { UpdateEntityPairs(); }
    }

    private void OnWaveStateChanged(WaveState state)
    {
      if (state == WaveState.Intermission)
      {
        PickupAllActiveDrops();
      }

    }

    #region EnemyUI


    //Update health bar locations
    private void UpdateEntityPairs()
    {
      foreach (KeyValuePair<GameObject, IEntityPosition> UIPair in enemyUIPairs)
      {
        UIPair.Key.transform.position = UIPair.Value.Body;
      }
    }

    //Add health bar pair for enemy tracking.
    public EntityUIController AddEntityUIPair(IEntityPosition entity)
    {
      enemyUIPairs ??= new Dictionary<GameObject, IEntityPosition>();

      GameObject enemyUI = objectPooler.GetFromPool(PoolingObjects.EnemyUI);

      enemyUI.transform.position = entity.Body;
      var uI = enemyUI.GetComponent<EntityUIController>();
      uI.group.alpha = 1;

      enemyUI.SetActive(true);
      enemyUIPairs[enemyUI] = entity;

      return uI;
    }



    public void RemoveEnemyUIPair(GameObject enemyUI, CanvasGroup group)
    {
      enemyUIPairs.Remove(enemyUI);
      UIAnimations.Instance.FadeOut(group, enemyUIFadeTime, () => { enemyUI.SetActive(false); });
    }

    #endregion

    #region Damage Text
    //Set up a text display for damage
    public void CreateDamageText(Transform textSpawn, TextInformation textInfo)
    {
      if (bookUIObject.activeInHierarchy) { return; }

      GameObject textInstance = objectPooler.GetFromPool(PoolingObjects.DamageText);

      //Set up damage element
      TMP_Text textTMP = textInstance.GetComponent<TMP_Text>();
      textTMP.text = textInfo.text;
      textTMP.colorGradient = textInfo.gradient;
      textTMP.font = textInfo.font;
      textTMP.fontSize = textTMP.fontSize;

      Color color = textTMP.color;
      color.a = 1f;
      textTMP.color = color;


      textInstance.transform.position = textSpawn.position;

      RectTransform rect = textInstance.GetComponent<RectTransform>();
      Vector2 targetLocation = rect.position + dmgFloatAmount;


      textInstance.SetActive(true);

      textInstance.transform.localScale = dmgInitialScale;

      StartCoroutine(UIAnimations.Instance.FadeText(textTMP, dmgLifetime));
      UIAnimations.Instance.ChangeScale(textInstance.GetComponent<RectTransform>(), dmgTargetScale, dmgLifetime / 4);
      UIAnimations.Instance.Slide(rect, targetLocation, dmgLifetime, () => { textInstance.SetActive(false); });
    }
    #endregion

    #region CurrencyText

    public void CreateValueTextObjects(int xpAmount, CurrencyType type, int currencyAmount, Vector3 position)
    {
      List<RectTransform> valueObjects = PrepareValueObjects();

      ConfigureValueObject(valueObjects[0], currencyAmount.ToString(), type.ToString());
      ConfigureValueObject(valueObjects[1], xpAmount.ToString(), "XP");

      if (gameObject.activeInHierarchy)
      {
        StartCoroutine(SpawnValueText(valueObjects, position));
      }
    }

    private List<RectTransform> PrepareValueObjects()
    {
      List<RectTransform> valueObjects = new();
      for (int i = 0; i < 2; i++)
      {
        GameObject value = objectPooler.GetFromPool(PoolingObjects.ValueText);
        value.SetActive(true);
        value.GetComponent<CanvasGroup>().alpha = 0;
        HideAllChildrenExceptText(value.transform);
        valueObjects.Add(value.GetComponent<RectTransform>());
      }
      return valueObjects;
    }

    private void HideAllChildrenExceptText(Transform parent)
    {
      for (int i = 0; i < parent.childCount; i++)
      {
        parent.GetChild(i).gameObject.SetActive(parent.GetChild(i).name == "Text");
      }
    }

    private void ConfigureValueObject(RectTransform valueObject, string text, string activeChildName)
    {
      TMP_Text txt = valueObject.Find("Text").GetComponent<TMP_Text>();
      txt.text = text;
      txt.colorGradient = worldSpaceUIReferences.ReturnValueGradient(activeChildName);
      valueObject.Find(activeChildName).gameObject.SetActive(true);
      LayoutRebuilder.ForceRebuildLayoutImmediate(valueObject);
    }

    private IEnumerator SpawnValueText(List<RectTransform> objects, Vector3 position)
    {
      Vector2 targetLocation = position + valueFloatAmount;

      for (int i = 0; i < objects.Count; i++)
      {
        SetObjectAnimation(targetLocation, objects[i], position);
        yield return new WaitForSeconds(timeBetweenValues);
      }
    }

    public void SetObjectAnimation(Vector2 targetLocation, RectTransform valueObject, Vector3 position)
    {
      // Capture the current index
      CanvasGroup group = valueObject.GetComponent<CanvasGroup>();
      group.alpha = 1f;
      valueObject.position = position;


      UIAnimations.Instance.FadeOut(group, valueLifetime, null, false);
      UIAnimations.Instance.Slide(valueObject, targetLocation, valueLifetime, () => { valueObject.gameObject.SetActive(false); }, false);
    }


    #endregion

    #region ItemDrop



    public void RemoveActiveDrop(ItemDrop drop)
    {
      if (drops.Contains(drop))
      {
        drops.Remove(drop);
      }
    }

    public void PickupAllActiveDrops()
    {
      if (drops.Count > 0)
      {
        foreach (ItemDrop drop in drops)
        {
          drop.OnWaveFinished();
        }
      }
      drops.Clear();
    }

    public void CreateItemDrop(ItemData item, int amount, Vector3 position)
    {
      GameObject itemDrop = objectPooler.GetFromPool(PoolingObjects.ItemDrop);

      CanvasGroup group = itemDrop.GetComponent<CanvasGroup>();

      ItemDrop dropClass = itemDrop.GetComponent<ItemDrop>();
      dropClass.image.material = itemDataBase.ReturnItemDropMaterial(item.grade);
      drops.Add(dropClass);

      group.alpha = 0f;
      itemDrop.GetComponent<Image>().sprite = item.image;
      var level = item.isUpgradable ? ItemLevel.Level0 : ItemLevel.None;
      dropClass.SetItem(item.iD, level, amount);

      float random = Random.Range(minTargetX, maxTargetX);

      int coinFlip = Random.Range(0, 2);
      if (coinFlip == 0)
      {
        random = random * -1;
      }

      itemDrop.transform.position = position;
      Vector3 targetPosition = position + new Vector3(random, 0f, 0f);
      itemDrop.SetActive(true);



      itemDrop.transform.localScale = itemInitialScale;

      UIAnimations.Instance.FadeIn(group, dropTime, 0, null, false);
      UIAnimations.Instance.ChangeScale(itemDrop.GetComponent<RectTransform>(), itemTargetScale, dropTime, null, false);
      UIAnimations.Instance.CurvedSlide(itemDrop, targetPosition, dropTime, () =>
      {
        if (dropClass.gameObject.activeInHierarchy) { dropClass.despawnRoutine = dropClass.StartDespawnTimer(group); }
      }, false);
    }

    public IEnumerator ItemDespawnTimer(CanvasGroup group, ItemDrop drop)
    {
      // Initial wait time
      float initialWaitTime = itemDespawnTime - (itemDespawnTime * ServiceLocator.Get<ScalingHandler>().ReturnItemDespawnScaler());
      yield return new WaitForSeconds(initialWaitTime);

      float time = itemDespawnTime;
      // blink every half second

      bool isVisible = true;

      while (time > 0)
      {
        // Add this check
        if (group.gameObject.activeInHierarchy)
        {
          yield return new WaitForSeconds(blinkInterval);

          // reduce time
          time -= blinkInterval;

          // reduce the blink interval (make it blink faster)
          blinkInterval *= 0.9f; // Adjust this number to make it blink faster over time

          // Make sure the blink interval never goes below a certain limit (e.g., 0.05 seconds)
          blinkInterval = Mathf.Max(blinkInterval, 0.2f);

          // toggle visibility
          isVisible = !isVisible;
          group.alpha = isVisible ? 1f : 0f;
        }
        else
        {
          // If GameObject is not active, break the loop
          yield return null;
        }
      }

      // Make sure the item is invisible at the end
      group.alpha = 0f;

      // Turn off the game object here
      group.gameObject.SetActive(false);
      RemoveActiveDrop(drop);
      ServiceLocator.Get<ScavengerHandler>().AddScavengedItem(drop.iD, drop.level, drop.amount);
    }

    #endregion
  }

}