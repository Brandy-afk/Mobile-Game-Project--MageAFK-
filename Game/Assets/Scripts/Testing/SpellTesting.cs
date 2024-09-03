using UnityEngine;

public class SpellTesting : MonoBehaviour
{

  [SerializeField] private GameObject enemyToSpawn;
  [SerializeField] private Transform spawnLocation;
  public void SpawnEnemy()
  {
    GameObject Instance = Instantiate(enemyToSpawn);

    enemyToSpawn.transform.position = spawnLocation.position;

  }
}
