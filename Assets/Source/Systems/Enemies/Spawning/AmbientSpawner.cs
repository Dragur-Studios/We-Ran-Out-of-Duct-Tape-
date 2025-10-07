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
    [Header("Editor Only!")]
    [SerializeField] bool is_simulationMode = false;
    [SerializeField] GameObject[] simulationPrefabs;

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

            GameObject zombie = null;
            if (is_simulationMode)
            {
                // determine zombie by probabillity.
                // right now there is only the shambler types as prefabs.. but should soon be 
                // phased into their own prefabs.

                float roll = Random.value * 100.0f;
                int idx = 0;
                if(roll > 90.0f) // 10% chance for sprinter.
                {
                    idx = 1;
                }


                zombie = Instantiate(simulationPrefabs[idx]);

                zombie.transform.position = hit.position;

                Vector2 offset = Random.insideUnitCircle;
                Vector3 targetPosition = hit.position + new Vector3(offset.x, 0, offset.y);
                var dir = targetPosition - hit.position;

                if (dir.sqrMagnitude < 0.0001f)
                    dir = Vector3.forward;

                Quaternion lookDir = Quaternion.LookRotation(dir);

                zombie.transform.rotation = lookDir;
            }   
            else
            {
               zombie =  GameManager.Instance.SpawnEnemy(hit.position, Quaternion.identity);
            }
            zombies.Add(zombie);

            //var resolver = zombie.GetComponent<EnemyAgent>();
            //if (resolver != null)
            //{
            //    resolver.SetPassive(true);
            //}
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
