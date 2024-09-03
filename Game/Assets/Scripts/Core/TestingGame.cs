using System.Collections.Generic;
using MageAFK.AI;
using MageAFK.Items;
using MageAFK.Management;
using MageAFK.Player;
using MageAFK.Pooling;
using MageAFK.Tools;
using MageAFK.UI;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class TestingGame : MonoBehaviour
{

  [SerializeField, TabGroup("Layer Testing")] private ProjectLayerMask layer;

  [SerializeField, TabGroup("Layer Testing"), Button("Print")]
  public void Print()
  {
    print($"GetMask : {LayerMask.GetMask(layer.ToString())}");
    print($"NameToLayer : {LayerMask.NameToLayer(layer.ToString())}");
  }

  [SerializeField, TabGroup("Layer Testing"), Button("Set")]
  public void SetLayer()
  {

    try
    {
      gameObject.layer = LayerMask.NameToLayer(layer.ToString());
    }
    catch
    {
      print($"NameToLayer had issues : {LayerMask.NameToLayer(layer.ToString())}");
    }

  }


  public bool isTesting;
  [SerializeField, InfoBox("Used For All Testing Systems", InfoMessageType.None)] private List<Transform> objects;



  #region Entity UI testing
  [SerializeField, TabGroup("Entity UI")] private Transform ui;
  [SerializeField, TabGroup("Entity UI")] private Transform parent;

  [Button("Get Next Entity"), TabGroup("Entity UI")]
  private void GetNextEntity()
  {
    var entity = objects[0];
    entity.position = Vector2.zero;
    ui.transform.position = entity.GetComponent<NPEntity>().body.position;
    entity.SetParent(parent);
    Selection.activeTransform = entity;
  }

  [Button("Set Variables"), TabGroup("Entity UI")]
  private void SetVariables()
  {
    var nPEntity = objects[0].GetComponent<NPEntity>();
    var scriptUI = ui.GetComponent<EntityUIController>();

    nPEntity.healthBarLoc = scriptUI.healthBar.transform.localPosition;
    nPEntity.effectsLoc = scriptUI.effects.localPosition;
  }

  [Button("Remove and get next entity (Will Destroy)"), TabGroup("Entity UI")]
  private void RemoveAndGetNext()
  {
    var entity = objects[0];
    objects.RemoveAt(0);
    DestroyImmediate(entity.gameObject);
    GetNextEntity();
  }

  #endregion

  #region Item Factory
  [SerializeField, TabGroup("ItemFactory")] private ItemIdentification iD;
  [SerializeField, TabGroup("ItemFactory")] private ItemLevel level;
  [SerializeField, TabGroup("ItemFactory")] private int qty;
  [Button("Add Item"), TabGroup("ItemFactory")]
  public void AddItems()
  {
    ServiceLocator.Get<InventoryHandler>().AddItem(iD, level, qty);
  }
  #endregion

  #region Ability Testing
  [SerializeField, TabGroup("AbilityTesting")] private ObjectPooler objectPooler;
  [SerializeField, TabGroup("AbilityTesting")] private Transform[] testingMobs;
  [SerializeField, TabGroup("AbilityTesting")]
  private int spawnIndex;
  [SerializeField, TabGroup("AbilityTesting")] private GameObject prefabSpell;
  [SerializeField, TabGroup("AbilityTesting")] private GameObject secondaryPrefab;


  private void Awake()
  {
    if (isTesting)
    {
      objectPooler.CreatePool(PoolingObjects.DamageText);
      objectPooler.CreatePool(PoolingObjects.EnemyUI);
    }
  }

  [Button("Spawn At"), TabGroup("AbilityTesting")]
  public void TestAbility()
  {
    if (!isTesting) return;

    // Instantiate(secondaryPrefab, testingMobs[spawnIndex]);

    var gameObj = Instantiate(prefabSpell, testingMobs[spawnIndex].position, Quaternion.identity);
    gameObj.GetComponent<Stormbringer_chain>().Initialize(5, 3, new System.Collections.Generic.HashSet<Collider2D>());
    gameObj.SetActive(true);

  }

  [Button("Shoot ability"), TabGroup("AbilityTesting")]
  public void TestAbility2()
  {


  }
  #endregion


  #region Spell Testing
  [TabGroup("ObjectTesting"), SerializeField] private GameObject newPrefab;
  [TabGroup("ObjectTesting"), SerializeField] private NPEntity testEnemy;
  [TabGroup("ObjectTesting"), SerializeField] private PlayerController testPlayer;

  [TabGroup("ObjectTesting"), Button("Get Next Spell")]
  private void GetNextObject()
  {
    var obj = objects[0];
    obj.position = Vector2.zero;
  }

  [TabGroup("ObjectTesting"), Button("Spawn at body")]
  private void SpawnCurrentAtEnemyBody()
  {
    objects[0].position = testEnemy.body.position;
  }

  [TabGroup("ObjectTesting"), Button("Spawn at feet")]
  private void SpawnCurrentAtFeet()
  {
    objects[0].position = testEnemy.feet.position;
  }

  [TabGroup("ObjectTesting"), Button("Spawn at pivot")]
  private void SpawnCurrentAtPivot()
  {
    objects[0].position = testEnemy.transform.position;
  }

  [TabGroup("ObjectTesting"), Button("Spawn at player body")]
  private void SpawnCurrentAtPlayerBody()
  {
    objects[0].position = testPlayer.body.position;
  }

  [TabGroup("ObjectTesting"), Button("Spawn at player feet")]
  private void SpawnCurrentAtPlayerFeet()
  {
    objects[0].position = testPlayer.feet.position;
  }

  [TabGroup("ObjectTesting"), Button("Spawn at player pivot")]
  private void SpawnCurrentAtPlayerpivot()
  {
    objects[0].position = testPlayer.transform.position;
  }


  [TabGroup("ObjectTesting"), Button("DeleteAndGetNext")]
  private void DeleteAndGetNextSpell()
  {
    var obj = objects[0];
    objects.RemoveAt(0);
    DestroyImmediate(obj.gameObject);
    GetNextObject();
  }


  #endregion





}