using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBehaviorResolver : MonoBehaviour
{
    enum EnemyState { Chasing, Attacking }
    EnemyState currentState = EnemyState.Chasing;

    NavMeshAgent agent;
    Transform target;

    [Header("Combat Settings")]
    public float attackRange = 1.5f;
    public float attackCooldown = 1.0f;

    float lastAttackTime;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        target = GameManager.Instance.Player.transform;
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    private void Update()
    {
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        switch (currentState)
        {
            case EnemyState.Chasing:
                agent.isStopped = false;
                agent.stoppingDistance = attackRange - 0.1f;
                agent.SetDestination(target.position);

                if (distance <= attackRange)
                {
                    currentState = EnemyState.Attacking;
                }
                break;

            case EnemyState.Attacking:
                agent.isStopped = true;
                transform.LookAt(target);

                if (Time.time - lastAttackTime >= attackCooldown)
                {
                    Attack();
                    lastAttackTime = Time.time;
                }

                if (distance > attackRange)
                {
                    currentState = EnemyState.Chasing;
                }
                break;
        }
    }

    private void Attack()
    {
        // TODO: Hook into animation system or damage system
        Debug.Log("Enemy attacks the player!");
    }
}
