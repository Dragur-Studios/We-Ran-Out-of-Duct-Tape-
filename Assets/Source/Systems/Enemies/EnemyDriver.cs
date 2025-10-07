using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyDriver : MonoBehaviour
{
    Enemy self;
    EnemyAgent agent;
    NavMeshAgent nav;

    [SerializeField, Range(0.1f, 1.25f)]float max_speed = 0.5f;

    void Awake()
    {
        self = GetComponent<Enemy>();
        agent = GetComponent<EnemyAgent>();
        nav = GetComponent<NavMeshAgent>();
        nav.speed = max_speed;

        nav.updatePosition = false; // root motion drives position
        nav.updateRotation = false; // EnemyRotator handles rotation
    }

    void Update()
    {
        if (self.IsDead())
            return;

        var state = agent.GetWorldState();

        // If a MoveTarget is set, ensure nav has a path
        var moveTargetTransform = state.Get<Transform>(GOAPKey.MoveTarget, null);
        Vector3? moveTargetPos = null;

        if (moveTargetTransform != null)
        {
            moveTargetPos = moveTargetTransform.position;
        }
        else
        {
            // maybe MoveTarget was stored as a Vector3 instead
            moveTargetPos = state.Get<Vector3>(GOAPKey.MoveTarget, Vector3.zero);
            if (moveTargetPos == Vector3.zero) moveTargetPos = null;
        }

        if (moveTargetPos.HasValue)
        {
            Vector3 dest = moveTargetPos.Value;

            if (!nav.hasPath || Vector3.Distance(nav.destination, dest) > 0.5f)
            {
                nav.isStopped = false;
                nav.SetDestination(dest);
                state.Set(GOAPKey.Moving, true);
            }

            // Check arrival
            if (!nav.pathPending && nav.remainingDistance <= nav.stoppingDistance + 0.2f)
            {
                state.Set(GOAPKey.Moving, false);
                state.Set(GOAPKey.MoveTarget, null); // clear intent
            }
        }
    }

}


