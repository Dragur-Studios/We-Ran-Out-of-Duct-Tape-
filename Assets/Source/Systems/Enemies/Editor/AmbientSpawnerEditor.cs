using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class AmbientSpawner : MonoBehaviour
{
    public SpawnVolumeType volumeType;
    public float spawnRadius = 10f;
    public Vector3 spawnExtents = new Vector3(5, 5, 5);
    public LayerMask obstructionMask;
    public int maxSpawnTries = 5;
    public float clearanceRadius = 0.5f;

    Queue<GameObject> retryQueue = new Queue<GameObject>();

    public void Spawn(GameObject prefab)
    {
        Vector3 pos;
        if (TryFindValidSpawnPoint(out pos))
        {
            Instantiate(prefab, pos, Quaternion.identity);
        }
        else
        {
            retryQueue.Enqueue(prefab);
        }
    }

    bool TryFindValidSpawnPoint(out Vector3 result)
    {
        for (int i = 0; i < maxSpawnTries; i++)
        {
            Vector3 candidate = RandomPointInVolume();
            candidate += Vector3.up * 2f; 

            if (Physics.SphereCast(candidate, clearanceRadius, Vector3.down, out RaycastHit hit, 10f, ~0, QueryTriggerInteraction.Ignore))
            {
                Vector3 groundPos = hit.point;

                if (!Physics.CheckSphere(groundPos, clearanceRadius, obstructionMask))
                {
                    if (NavMesh.SamplePosition(groundPos, out NavMeshHit navHit, 1.0f, NavMesh.AllAreas))
                    {
                        result = navHit.position;
                        return true;
                    }
                }
            }
        }

        result = Vector3.zero;
        return false;
    }

    Vector3 RandomPointInVolume()
    {
        if (volumeType == SpawnVolumeType.Sphere)
        {
            return transform.position + Random.insideUnitSphere * spawnRadius;
        }
        else 
        {
            return transform.position + new Vector3(
                Random.Range(-spawnExtents.x, spawnExtents.x),
                Random.Range(-spawnExtents.y, spawnExtents.y),
                Random.Range(-spawnExtents.z, spawnExtents.z)
            );
        }
    }

    void Update()
    {
        if (retryQueue.Count > 0)
        {
            GameObject prefab = retryQueue.Dequeue();
            Spawn(prefab);
        }
    }
}
