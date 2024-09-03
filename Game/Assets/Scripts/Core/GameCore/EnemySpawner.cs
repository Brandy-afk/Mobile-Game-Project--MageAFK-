using UnityEngine;
using MageAFK.AI;
using MageAFK.Pooling;
using System.Collections;
using MageAFK.Management;
using MageAFK.Stats;
using MageAFK.Tools;
using System.Text;

namespace MageAFK.Core
{
  /// <summary>
  /// Class handles all enemy behaviour and control over enemy spawning. Spawning itself is handled by enemies custom class. 
  /// </summary>
  public class EnemySpawner : MonoBehaviour
  {
    [Header("Player transform")]
    [SerializeField] private Transform player;
    private int lastSpawnSide = -1;




    #region Initialization , Load/Save
    private void Awake() => ServiceLocator.RegisterService(this);
    private void Start()
    {
      WaveHandler.SubToWaveState(HandleWaveStateChange, true);
      WaveHandler.SubToWaveEvent(OnEvent, true);
      WaveHandler.SubToSiegeEvent(OnEvent, true);
    }

    #endregion


    private void HandleWaveStateChange(WaveState state)
    {
      if (state == WaveState.Wave)
      {
        StringBuilder names = new StringBuilder();
        foreach (Mob enemy in EnemyPooler.currentMobs)
        {
          if (enemy == null) continue;
          StartCoroutine(TrackEnemySpawnTimer(enemy));
          names.Append(enemy.name + " ");
        }
        Debug.Log($"Starting spawns for {names}");
      }

    }


    //On Seige or wave ends, stop spawning enemies
    private void OnEvent(Status status)
    {
      if (status == Status.End)
        StopAllCoroutines();
    }


    private IEnumerator TrackEnemySpawnTimer(Mob mob)
    {
      while (WaveHandler.WaveState == WaveState.Wave)
      {
        mob.prefab.GetComponent<Enemy>().Spawn();
        yield return new WaitForSeconds(mob.data.GetStats(AIDataType.Altered)[Stat.EnemySpawnRate]);
      }
    }


    //Default Spawns 
    public GameObject DefaultSpawn(EntityIdentification iD)
    {
      Vector2 spawnPoint;
      int spawnSide = GetSpawnSide();

      switch (spawnSide)
      {
        case 0: // Spawn from bottom
          spawnPoint = new Vector2(Random.Range(-0.2f, 1.2f), -0.2f);
          break;
        case 1: // Spawn from left
          spawnPoint = new Vector2(-0.2f, Random.Range(-0.2f, 0.5f));
          break;
        case 2: // Spawn from right
          spawnPoint = new Vector2(1.2f, Random.Range(-0.2f, 0.5f));
          break;
        default: // This case should never happen, but it's good to have a default just in case
          spawnPoint = new Vector2(Random.Range(0f, 1f), Random.Range(-0.2f, -0.4f));
          break;
      }

      Vector3 worldSpawnPoint = Utility.MainCam.ViewportToWorldPoint(spawnPoint);
      worldSpawnPoint.z = 0;

      GameObject enemyInstance = ServiceLocator.Get<EnemyPooler>().Get(iD);


      if (enemyInstance != null)
      {
        enemyInstance.transform.position = worldSpawnPoint;

        //Set Rotation
        Vector3 enemyDirectionWorld = player.position - worldSpawnPoint;
        SpriteRenderer spriteRenderer = enemyInstance.GetComponent<SpriteRenderer>();

        if (enemyDirectionWorld.x > 0)
        {
          spriteRenderer.transform.localScale = new Vector3(1, spriteRenderer.transform.localScale.y, spriteRenderer.transform.localScale.z);
        }
        else
        {
          spriteRenderer.transform.localScale = new Vector3(-1, spriteRenderer.transform.localScale.y, spriteRenderer.transform.localScale.z);
        }

        StartCoroutine(DelayActiveTillNextFrame(enemyInstance, true));
        enemyInstance.GetComponent<Enemy>().OnSetActive();
      }


      return enemyInstance;
    }

    private IEnumerator DelayActiveTillNextFrame(GameObject gameObject, bool active)
    {
      yield return null;  // Wait until the next frame.
      gameObject.SetActive(active);
    }



    private int GetSpawnSide()
    {
      int side;

      // Loop until we find a side that is different from the last spawned side
      do
      {
        side = Random.Range(0, 3);
      }
      while (side == lastSpawnSide);

      // Update the last spawned side
      lastSpawnSide = side;

      return side;
    }


  }



}