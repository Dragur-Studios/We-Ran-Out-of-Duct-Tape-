using UnityEngine;
using System.Collections;

public class WaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public int count;
        public float interval;
    }

    public Wave[] waves;
    public Transform[] spawnPoints;

    private int currentWave = 0;

    public void StartNight()
    {
        StartCoroutine(SpawnWave(waves[currentWave]));
    }

    private IEnumerator SpawnWave(Wave wave)
    {
        for (int i = 0; i < wave.count; i++)
        {
            SpawnZombie();
            yield return new WaitForSeconds(wave.interval);
        }
        currentWave++;
    }

    private void SpawnZombie()
    {
        Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameManager.Instance.SpawnEnemy(point.position, point.rotation);
    }
}
