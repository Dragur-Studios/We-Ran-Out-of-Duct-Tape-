using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBehaviorResolver : MonoBehaviour
{
    public enum ZombieType { Shambler, Sprinter }
    public enum EnemyState
    {
        Passive,
        Swarming,
        Chasing,
        Attacking,
        Investigating
    }
    EnemyState currentState = EnemyState.Passive;
    [Header("Zombie Type")]
    public ZombieType zombieType = ZombieType.Shambler;
    NavMeshAgent agent;
    Transform target;

    [Header("Combat Settings")]
    public float attackRange = 1.5f;
    public float attackCooldown = 1.0f;
    float lastAttackTime;

    [Header("Swarm Settings")]
    [SerializeField] float neighborRadius = 5f;
    [SerializeField] float separationWeight = 1.5f;
    [SerializeField] float cohesionWeight = 1.0f;
    [SerializeField] float alignmentWeight = 1.0f;
    [SerializeField] LayerMask enemyMask;
    [SerializeField] float targetWeight = 3.0f;

    [Header("Perception")]
    public float visionRange = 15f;
    public float hearingRange = 8f;
    public LayerMask visionMask;

    [Header("Wander Settings")]
    public float wanderRadius = 6f;
    public float wanderInterval = 4f;
    private float wanderTimer;
    private bool passive = false;

    [SerializeField] float aggroMemory = 5f; // seconds they remember the player
    private float lastTimePlayerSeen = Mathf.NegativeInfinity;
    [SerializeField] float investigateDuration = 5f;
    private Vector3 lastKnownPlayerPos;
    private float investigateTimer;

    public void SetPassive(bool value)
    {
        passive = value;
        if (passive) currentState = EnemyState.Passive;
    }

    private void Start()
    {
        // 80% chance shambler, 20% chance sprinter
        int roll = Random.Range(0, 100);
        if (roll < 80)
            zombieType = ZombieType.Shambler;
        else
            zombieType = ZombieType.Sprinter;

        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = false;
        agent.updateRotation = false;

        switch (zombieType)
        {
            case ZombieType.Shambler:
                agent.speed = Random.Range(0.5f, 0.75f); // slow shambling
                break;
            case ZombieType.Sprinter:
                agent.speed = Random.Range(2.5f, 4.0f); // fast sprinting
                break;
        }

        agent.radius = 0.25f;
        agent.avoidancePriority = Random.Range(30, 70);

        //var exists = GameManager.Instance.Player != null;
        //if (exists)
        //{
            target = GameManager.Instance.Player.transform;
        //}
        wanderTimer = wanderInterval;
    }



    private void UpdateRotation()
    {
        if (currentState == EnemyState.Passive)
            return; // don’t override root motion wander facing

        Vector3 desiredVel = agent.desiredVelocity;
        desiredVel.y = 0f;

        if (desiredVel.sqrMagnitude > 0.01f) // only rotate if moving
        {
            Quaternion targetRot = Quaternion.LookRotation(desiredVel.normalized, Vector3.up);
            float minTurnSpeed = 3f;
            float maxTurnSpeed = 12f;
            float normalized = Mathf.Clamp01(desiredVel.magnitude / agent.speed);
            float turnSpeed = Mathf.Lerp(minTurnSpeed, maxTurnSpeed, normalized);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * turnSpeed);
        }
        else if (currentState == EnemyState.Attacking)
        {
            // Face the player directly
            Vector3 toTarget = (target.position - transform.position);
            toTarget.y = 0f;
            if (toTarget.sqrMagnitude > 0.01f)
            {
                Quaternion targetRot = Quaternion.LookRotation(toTarget, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 8f);
            }
        }
    }


    private void Update()
    {
        UpdateRotation();
        
        if (target == null) return;
        
        float distance = Vector3.Distance(transform.position, target.position);

        switch (currentState)
        {
            case EnemyState.Passive:
                agent.updateRotation = true;
                WanderAndCluster();
                if (CanSeePlayer())
                {
                    lastTimePlayerSeen = Time.time;

                    currentState = EnemyState.Chasing;
                }
                   break;

            case EnemyState.Swarming:
                agent.updateRotation = false;
                agent.isStopped = false;
                Swarm();
                if (distance <= 8f)
                    currentState = EnemyState.Chasing;
                break;

            case EnemyState.Chasing:

                if (CanSeePlayer())
                {
                    lastKnownPlayerPos = target.position;
                    agent.SetDestination(target.position);
                }
                else
                {
                    // Lost sight — switch to investigating
                    currentState = EnemyState.Investigating;
                    investigateTimer = investigateDuration;
                    agent.SetDestination(lastKnownPlayerPos);
                }

                agent.isStopped = false;
                agent.stoppingDistance = attackRange - 0.1f;
                agent.SetDestination(target.position);

                if (distance <= attackRange)
                    currentState = EnemyState.Attacking;

                // Lose aggro if player not seen for too long
                if (!CanSeePlayer() && Time.time - lastTimePlayerSeen > aggroMemory)
                {
                    currentState = EnemyState.Passive;
                }

                break;
            case EnemyState.Investigating:
                agent.isStopped = false;
                agent.stoppingDistance = 0f;

                // If they see the player, escalate to chase
                if (CanSeePlayer())
                {
                    currentState = EnemyState.Chasing;
                    break;
                }

                // Countdown search time
                investigateTimer -= Time.deltaTime;

                // If reached current search point, pick a new one
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.2f)
                {
                    if (investigateTimer > 0f)
                        SetNewInvestigatePoint();
                }

                // If timer runs out, give up
                if (investigateTimer <= 0f)
                {
                    currentState = EnemyState.Passive;
                }
                break;
            case EnemyState.Attacking:
                AttackBehavior(distance);
                break;
        }
    }
    public void AlertToPlayer(Vector3 soundPos)
    {
        if (currentState == EnemyState.Passive || currentState == EnemyState.Swarming)
        {
            lastKnownPlayerPos = soundPos;
            investigateTimer = investigateDuration;

            currentState = EnemyState.Investigating;
            agent.isStopped = false;
            SetNewInvestigatePoint();
        }
    }

    [SerializeField] float investigateRadius = 5f;

    private void SetNewInvestigatePoint()
    {
        // Pick a random point near the last known sound position
        Vector2 randomCircle = Random.insideUnitCircle * investigateRadius;
        Vector3 candidate = lastKnownPlayerPos + new Vector3(randomCircle.x, 0, randomCircle.y);

        agent.SetDestination(candidate);
    }


    // ---------------- Passive Wander + Cluster ----------------
    private void WanderAndCluster()
    {
        wanderTimer += Time.deltaTime;

        // Only pick a new destination if timer expired and agent is idle
        if (wanderTimer >= wanderInterval && !agent.hasPath)
        {
            // --- Randomize wander interval slightly so zombies desync ---
            wanderInterval = Random.Range(3f, 6f);

            // --- Optional idle pause before moving again ---
            if (Random.value < 0.3f) // 30% chance to just idle this cycle
            {
                wanderTimer = 0f;
                return;
            }

            // --- Base wander point ---
            Vector3 randomPos = transform.position + Random.insideUnitSphere * wanderRadius;
            randomPos.y = 0;

            // --- Cluster bias: move toward average neighbor position ---
            Collider[] neighbors = Physics.OverlapSphere(transform.position, neighborRadius, enemyMask);
            if (neighbors.Length > 1)
            {
                Vector3 avgPos = Vector3.zero;
                int count = 0;
                foreach (var n in neighbors)
                {
                    if (n.transform == transform) continue;
                    avgPos += n.transform.position;
                    count++;
                }
                if (count > 0)
                {
                    avgPos /= count;

                    // Blend wander with cluster, but with a lighter weight
                    randomPos = Vector3.Lerp(randomPos, avgPos, 0.25f);
                }
            }

            // --- Sample NavMesh and set destination ---
            if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                agent.isStopped = false;
                agent.SetDestination(hit.position);
            }

            wanderTimer = 0f;
        }
    }


    // ---------------- Combat Behaviors ----------------
    private void AttackBehavior(float distance)
    {
        agent.isStopped = true;

        // Face player
        Vector3 toTarget = (target.position - transform.position).normalized;
        toTarget.y = 0f;
        if (toTarget.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(toTarget, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 8f);
        }

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            Attack();
            lastAttackTime = Time.time;
        }

        if (distance > attackRange)
            currentState = EnemyState.Chasing;
    }


    private void Swarm()
    {
        Collider[] neighbors = Physics.OverlapSphere(transform.position, neighborRadius, enemyMask);

        Vector3 separation = Vector3.zero;
        Vector3 alignment = Vector3.zero;
        Vector3 cohesion = Vector3.zero;

        int count = 0;
        foreach (var n in neighbors)
        {
            if (n.transform == transform) continue;
            Vector3 toNeighbor = transform.position - n.transform.position;
            float dist = toNeighbor.magnitude;

            separation += toNeighbor.normalized / Mathf.Max(dist, 0.01f);
            alignment += n.transform.forward;
            cohesion += n.transform.position;
            count++;
        }

        if (count > 0)
        {
            separation /= count;
            alignment.Normalize();
            cohesion = ((cohesion / count) - transform.position).normalized;
        }

        // Strong pull toward the player
        Vector3 toTarget = (target.position - transform.position).normalized;

        Vector3 steering =
            toTarget * targetWeight +
            separation * separationWeight +
            alignment * alignmentWeight +
            cohesion * cohesionWeight;

        // Clamp instead of normalize
        if (steering.magnitude > 1f)
            steering = steering.normalized;

        // Give the agent a meaningful destination
        Vector3 desired = transform.position + steering * 5f; // 5 units ahead, not 2
        agent.stoppingDistance = 0f;
        agent.isStopped = false;

        // Ensure it's on the NavMesh
        if (NavMesh.SamplePosition(desired, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            Vector3 offset = Random.insideUnitCircle.normalized * (attackRange + agent.radius);
            Vector3 attackSpot = target.position + new Vector3(offset.x, 0, offset.y);
            agent.SetDestination(attackSpot);

        }
    }



    [SerializeField, Range(30f, 180f)] float visionAngle = 90f; // cone angle

    private bool CanSeePlayer()
    {
        if (target == null) return false;

        Vector3 dirToPlayer = (target.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, target.position);

        // 1. Check distance
        if (distanceToPlayer > visionRange) return false;

        // 2. Check angle (frustum cone)
        float angle = Vector3.Angle(transform.forward, dirToPlayer);
        if (angle > visionAngle * 0.5f) return false;

        // 3. Raycast for occlusion
        if (Physics.Raycast(transform.position + Vector3.up, dirToPlayer, out RaycastHit hit, visionRange, visionMask))
        {
            if(hit.collider.tag != "Player")
            {
                return false;
            }
            return hit.transform == target;
        }

        return false;
    }


    private void OnDrawGizmosSelected()
    {
        bool canSeePlayer = CanSeePlayer();

        var color =  canSeePlayer ? new Color(0, 0.5f, 0.5f, 0.25f) : new Color(1f, 0.5f, 0f, 0.25f); // translucent orange cone

        // Draw forward direction line
        Vector3 forward = transform.forward * visionRange;
        //Gizmos.DrawRay(transform.position + Vector3.up, forward);

        // Calculate left/right boundaries of the cone
        Quaternion leftRot = Quaternion.AngleAxis(-visionAngle * 0.5f, Vector3.up);
        Quaternion rightRot = Quaternion.AngleAxis(visionAngle * 0.5f, Vector3.up);

        Vector3 leftDir = leftRot * transform.forward;
        Vector3 rightDir = rightRot * transform.forward;

        // Draw cone edges

        // Optional: draw an arc to visualize the cone
#if UNITY_EDITOR
        Handles.color = color;
        Handles.DrawSolidArc(
            transform.position,
            Vector3.up,
            leftDir,
            visionAngle,
            visionRange
        );
#endif
    }



    private void Attack()
    {
        var anim = GetComponentInChildren<Animator>();
        anim.SetInteger("Attack ID", Random.Range(0, 4));
        anim.CrossFade("Attack", 0.1f);
    }
}
