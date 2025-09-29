using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class AmbientSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public int maxZombies = 10;
    public float spawnRadius = 20f;

    private List<GameObject> zombies = new List<GameObject>();

    private void Start()
    {
        SpawnAllZombies();
    }


    private void SpawnAllZombies()
    {
        for (int i = 0; i < maxZombies; i++)
        {
            Vector3 randomPos = transform.position + Random.insideUnitSphere * spawnRadius;
            randomPos.y = 0;

            if (NavMesh.SamplePosition(randomPos, out var hit, 5f, NavMesh.AllAreas))
            {
                var zombie = GameManager.Instance.SpawnEnemy(hit.position, Quaternion.identity);
                zombies.Add(zombie);

                // Put them into "idle/ambient" mode until they detect the player
                var resolver = zombie.GetComponent<EnemyBehaviorResolver>();
                if (resolver != null)
                {
                    resolver.SetPassive(true);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
