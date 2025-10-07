using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "GOAP/Wander")]
public class WanderAction : GOAPAction
{
    public float radius = 6f;
    public float interval = 3f;
    public float idleMin = 1f;
    public float idleMax = 3f;
    public int sampleRetries = 6;

    Vector3 currentDestination;
    bool hasDestination;
    float idleUntil;
    float nextPickTime;

    private void OnEnable()
    {
        actionName = "Wander";
        preconditions = new List<GOAPGoal>(); // always available
        effects = new List<GOAPGoal> { new GOAPGoal { key = GOAPKey.IsWandering, boolValue=true, type=GOAPValueType.Bool } };
        
    }

    public override bool CheckPreconditions(GOAPWorldState state) => true;

    public override bool StartAction(GOAPWorldState state)
    {
        hasDestination = false;
        idleUntil = 0f;
        nextPickTime = Time.time + Random.Range(0f, interval);
        return true;
    }

    public override bool PerformAction(float deltaTime, GOAPWorldState state)
    {
        // If arrived, idle
        if (hasDestination && !state.Get(GOAPKey.Moving, true))
        {
            if (idleUntil == 0f)
                idleUntil = Time.time + Random.Range(idleMin, idleMax);

            if (Time.time < idleUntil)
                return false; // still idling

            // finished idling, reset and schedule next pick
            hasDestination = false;
            idleUntil = 0f;
            nextPickTime = Time.time + Random.Range(0.2f * interval, 1.5f * interval);

            // clear movement intent
            state.Set(GOAPKey.MoveTarget, null);
            state.Set(GOAPKey.Moving, false);
        }

        // If no destination, pick one
        if (!hasDestination && Time.time >= nextPickTime)
        {
            for (int i = 0; i < sampleRetries; i++)
            {
                Vector3 candidate = agent.transform.position + Random.insideUnitSphere * radius;
                candidate.y = agent.transform.position.y;

                if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                {
                    currentDestination = hit.position;
                    hasDestination = true;

                    // Declare movement intent in world state; EnemyDriver will act on this
                    state.Set(GOAPKey.MoveTarget, currentDestination);
                    state.Set(GOAPKey.Moving, true);

                    state.Set(GOAPKey.WanderDestination, currentDestination);
                    state.Set(GOAPKey.IsWandering, true);

                    break;
                }
            }
        }

        return false; // wander is continuous, never "completes"
    }

    public override bool RequiresInRange() => false;

    public override void OnDrawGizmosSelected()
    {
        if (agent == null) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(agent.transform.position, radius);

        if (hasDestination)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(currentDestination, 0.15f);
            Gizmos.DrawLine(agent.transform.position, currentDestination);
        }
    }
}
