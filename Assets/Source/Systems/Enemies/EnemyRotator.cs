using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyRotator : MonoBehaviour
{
    [SerializeField] float minTurnSpeed = 3f;
    [SerializeField] float maxTurnSpeed = 12f;

    EnemyAgent enemyAgent;
    NavMeshAgent nav;

    void Awake()
    {
        enemyAgent = GetComponent<EnemyAgent>();
        nav = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (enemyAgent == null || nav == null) return;

        var currentAction = enemyAgent.GetCurrentAction();
        Vector3 forwardSource = Vector3.zero;

        // Prefer action target
        if (currentAction != null && currentAction.target != null)
            forwardSource = currentAction.target.position - transform.position;

        // Otherwise use navmesh desired velocity
        if (forwardSource.sqrMagnitude < 0.0001f)
            forwardSource = nav.desiredVelocity;

        // Or steering target
        if (forwardSource.sqrMagnitude < 0.0001f && nav.hasPath)
            forwardSource = nav.steeringTarget - transform.position;

        forwardSource.y = 0f;

        if (forwardSource.sqrMagnitude > 0.001f)
        {
            float velocityMag = nav.velocity.magnitude;
            float speedNormalized = Mathf.Clamp01(velocityMag / (nav.speed > 0 ? nav.speed : 1f));

            float turnSpeed = Mathf.Lerp(minTurnSpeed, maxTurnSpeed, speedNormalized);

            Quaternion want = Quaternion.LookRotation(forwardSource.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, want, Time.deltaTime * turnSpeed);
        }
    }
}
