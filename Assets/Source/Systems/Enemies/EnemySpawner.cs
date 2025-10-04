using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public int numberToSpawn = 5;
    public float spawnDuration = 10f;   // total time to spawn all enemies
    public Transform[] spawnPoints;     // optional: assign in inspector

    private void Start()
    {
        StartCoroutine(SpawnEnemiesOverTime());
    }

    private IEnumerator SpawnEnemiesOverTime()
    {
        float interval = spawnDuration / numberToSpawn;

        for (int i = 0; i < numberToSpawn; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(interval);
        }
    }

    private void SpawnEnemy()
    {
        // Pick a random spawn point if provided, otherwise use spawner's position
        Vector3 spawnPos = transform.position;
        Quaternion spawnRot = Quaternion.identity;

        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
            spawnPos = point.position;
            spawnRot = point.rotation;
        }

        GameManager.Instance.SpawnEnemy(spawnPos, spawnRot);
    }
}
