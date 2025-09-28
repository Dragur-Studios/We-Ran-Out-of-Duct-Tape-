using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;

public enum CompanionBehavior
{
    Heel,       // Walk right beside/behind target
    FlankLeft,  // Offset to left side
    FlankRight, // Offset to right side
    ScoutAhead, // Move ahead of target
    IdleNear,   // Stay close but not locked to a spot
    Circle,      // Orbit around target
    Caged,
}

public interface ICompanionState
{
    void Run(CompanionController runner);
}

// Heel State
public class HeelState : ICompanionState
{
    public void Run(CompanionController runner)
    {
        Vector3 targetPos = runner.followTarget.position
            + (-runner.followTarget.forward * runner.followDistance)
            + (runner.followTarget.right * runner.sideOffset);

        runner.UpdateDestination(targetPos);
    }
}

public class CagedState : ICompanionState
{
    public void Run(CompanionController runner)
    {
        runner.UpdateDestination(runner.transform.position); // dont move.. 
    }
}

// Flank Left State
public class FlankLeftState : ICompanionState
{
    public void Run(CompanionController runner)
    {
        Vector3 targetPos = runner.followTarget.position
            + (-runner.followTarget.forward * runner.followDistance)
            - (runner.followTarget.right * Mathf.Abs(runner.sideOffset));

        runner.UpdateDestination(targetPos);
    }
}

// Flank Right State
public class FlankRightState : ICompanionState
{
    public void Run(CompanionController runner)
    {
        Vector3 targetPos = runner.followTarget.position
            + (-runner.followTarget.forward * runner.followDistance)
            + (runner.followTarget.right * Mathf.Abs(runner.sideOffset));

        runner.UpdateDestination(targetPos);
    }
}

// Scout Ahead State
public class ScoutAheadState : ICompanionState
{
    public void Run(CompanionController runner)
    {
        Vector3 targetPos = runner.followTarget.position
            + (runner.followTarget.forward * runner.followDistance * 2f);

        runner.UpdateDestination(targetPos);
    }
}

// Idle Near State
public class IdleNearState : ICompanionState
{
    public void Run(CompanionController runner)
    {
        Vector3 randomOffset = new Vector3(
            Mathf.Sin(Time.time) * 0.5f,
            0,
            Mathf.Cos(Time.time) * 0.5f
        );

        Vector3 targetPos = runner.followTarget.position + randomOffset;
        runner.UpdateDestination(targetPos);
    }
}

// Circle State
public class CircleState : ICompanionState
{
    public void Run(CompanionController runner)
    {
        float angle = Time.time * 45f; // degrees/sec
        Vector3 circleOffset = Quaternion.Euler(0, angle, 0) * Vector3.forward * runner.followDistance;
        Vector3 targetPos = runner.followTarget.position + circleOffset;

        runner.UpdateDestination(targetPos);
    }
}

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class CompanionController : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform followTarget;
    public float followDistance = 2.0f;
    public float sideOffset = 0.0f;
    public float stoppingDistance = 0.2f;

    [Header("Rotation Settings")]
    public float rotationSpeed = 5f;

    [Header("Animation Settings")]
    public string speedParam = "Speed";
    public float animationSmoothTime = 0.1f;

    [Header("Movement Thresholds")]
    public float comfortRadius = 1.0f;
    public float rePathDistanceThreshold = 0.5f;

    private NavMeshAgent agent;
    private Animator animator;
    private float currentAnimSpeed = 0f;
    private float animVelocity;

    private Dictionary<CompanionBehavior, ICompanionState> states;
    public CompanionBehavior currentBehavior = CompanionBehavior.Heel;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        states = new Dictionary<CompanionBehavior, ICompanionState>
        {
            { CompanionBehavior.Heel, new HeelState() },
            { CompanionBehavior.FlankLeft, new FlankLeftState() },
            { CompanionBehavior.FlankRight, new FlankRightState() },
            { CompanionBehavior.ScoutAhead, new ScoutAheadState() },
            { CompanionBehavior.IdleNear, new IdleNearState() },
            { CompanionBehavior.Circle, new CircleState() },
            { CompanionBehavior.Caged, new CagedState() },
        };
    }

    void Update()
    {
        if (currentBehavior == CompanionBehavior.Caged)
        {
            animator.SetBool("Lay", true);
        }

        if (followTarget == null) return;

        states[currentBehavior].Run(this);
        ResolveRotation();
        ResolveAnimation();
    }

    public void UpdateDestination(Vector3 targetPos)
    {
        float distToTargetSpot = Vector3.Distance(transform.position, targetPos);
        float playerMoveDelta = Vector3.Distance(followTarget.position, targetPos);

        if (distToTargetSpot <= comfortRadius && playerMoveDelta < rePathDistanceThreshold)
            return;

        agent.SetDestination(targetPos);
    }

    void ResolveRotation()
    {
        float distToTargetSpot = Vector3.Distance(transform.position, agent.destination);

        if (distToTargetSpot <= comfortRadius)
        {
            Quaternion lookRot = Quaternion.LookRotation(followTarget.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * rotationSpeed);
        }
        else if (agent.remainingDistance > stoppingDistance)
        {
            Quaternion lookRot = Quaternion.LookRotation(agent.desiredVelocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * rotationSpeed);
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, followTarget.rotation, Time.deltaTime * rotationSpeed);
        }
    }

    void ResolveAnimation()
    {
        float desiredSpeed = agent.desiredVelocity.magnitude;
        float normalizedSpeed = Mathf.Clamp01(desiredSpeed / agent.speed);

        currentAnimSpeed = Mathf.SmoothDamp(
            currentAnimSpeed,
            normalizedSpeed,
            ref animVelocity,
            animationSmoothTime
        );

        animator.SetFloat(speedParam, currentAnimSpeed);
    }
}
