using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public enum SpawnVolumeType
{
    Sphere,
    Cuboid
}

public class AmbientSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public int maxZombies = 10;
    public SpawnVolumeType volumeType = SpawnVolumeType.Sphere;

    [Header("Sphere Settings")]
    public float spawnRadius = 20f;

    [Header("Cuboid Settings")]
    public Vector3 spawnExtents = new Vector3(20f, 5f, 20f); // half-size in each axis

    [Header("Clustering Settings")]
    public bool enableClustering = false;
    public int clusters = 3;
    public float clusterRadius = 5f;

    private List<GameObject> zombies = new List<GameObject>();

    private void Start()
    {
        SpawnAllZombies();
    }

    private void SpawnAllZombies()
    {
        if (enableClustering && clusters > 0)
        {
            SpawnClustered();
        }
        else
        {
            for (int i = 0; i < maxZombies; i++)
            {
                TrySpawnZombie(GetRandomPointInVolume());
            }
        }
    }

    private void SpawnClustered()
    {
        int zombiesPerCluster = Mathf.CeilToInt((float)maxZombies / clusters);

        for (int c = 0; c < clusters; c++)
        {
            // Pick a cluster center inside the volume
            Vector3 clusterCenter = GetRandomPointInVolume();

            for (int i = 0; i < zombiesPerCluster; i++)
            {
                Vector3 offset = Random.insideUnitSphere * clusterRadius;
                offset.y = 0;
                TrySpawnZombie(clusterCenter + offset);
            }
        }
    }

    private void TrySpawnZombie(Vector3 pos)
    {
        if (NavMesh.SamplePosition(pos, out var hit, 5f, NavMesh.AllAreas))
        {
            var zombie = GameManager.Instance.SpawnEnemy(hit.position, Quaternion.identity);
            zombies.Add(zombie);

            var resolver = zombie.GetComponent<EnemyBehaviorResolver>();
            if (resolver != null)
            {
                resolver.SetPassive(true);
            }
        }
    }

    private Vector3 GetRandomPointInVolume()
    {
        switch (volumeType)
        {
            case SpawnVolumeType.Cuboid:
                Vector3 randomOffset = new Vector3(
                    Random.Range(-spawnExtents.x, spawnExtents.x),
                    Random.Range(-spawnExtents.y, spawnExtents.y),
                    Random.Range(-spawnExtents.z, spawnExtents.z)
                );
                return transform.position + randomOffset;

            case SpawnVolumeType.Sphere:
            default:
                Vector3 randomPos = transform.position + Random.insideUnitSphere * spawnRadius;
                randomPos.y = transform.position.y; // flatten to ground
                return randomPos;
        }
    }
  
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        if (volumeType == SpawnVolumeType.Sphere)
        {
            Gizmos.DrawWireSphere(transform.position, spawnRadius);
        }
        else if (volumeType == SpawnVolumeType.Cuboid)
        {
            Gizmos.DrawWireCube(transform.position, spawnExtents * 2f);
        }

        if (enableClustering)
        {
            Gizmos.color = Color.cyan;
            for (int i = 0; i < clusters; i++)
            {
                Vector3 clusterCenter = Application.isPlaying ? GetRandomPointInVolume() : transform.position;
                Gizmos.DrawWireSphere(clusterCenter, clusterRadius);
            }
        }
    }
}
