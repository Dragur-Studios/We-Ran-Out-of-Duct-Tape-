using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(EnemyAgent))]
public class EnemyVisionSensor : MonoBehaviour
{
    [Header("Vision Settings")]
    [SerializeField, Range(0.0f, 50.0f)] float visionRange = 15f;
    [SerializeField, Range(1f, 180f)] float fieldOfView = 90f;
    [SerializeField] LayerMask targetMask;
    [SerializeField] LayerMask occlusionMask;
    [SerializeField, Range(1, 256)] int occlusionSamples = 16; // clamped to max 256

    [Header("Origin")]
    [SerializeField] Transform eyeOrigin;

    EnemyAgent agent;
    Enemy enemy;
    [SerializeField] bool drawVisionGizmos = true;   
    // cache the last rays we cast for gizmo drawing
    List<(Vector3 start, Vector3 end, bool hit)> lastRays = new();

    void Awake()
    {
        agent = GetComponent<EnemyAgent>();
        enemy = GetComponent<Enemy>();
        if (eyeOrigin == null) eyeOrigin = this.transform;
    }

    void Update()
    {
        if (enemy != null && enemy.IsDead()) return;

        Collider[] candidates = Physics.OverlapSphere(eyeOrigin.position, visionRange, targetMask);
        foreach (var c in candidates)
        {
            Transform target = c.transform;

            Vector3 dirToTarget = (target.position - eyeOrigin.position).normalized;
            float angle = Vector3.Angle(eyeOrigin.forward, dirToTarget);
            if (angle > fieldOfView * 0.5f) continue;

            if (HasLineOfSight(c))
            {
                agent.HandleEvent(new EnemyEvent(
                    EnemyEventType.SawSomething,
                    target.position,
                    target
                ));
            }
        }
    }

    bool HasLineOfSight(Collider target)
    {
        Bounds b = target.bounds;
        lastRays.Clear();

        // clamp to max 256
        int samples = Mathf.Clamp(occlusionSamples, 1, 256);

        for (int i = 0; i < samples; i++)
        {
            // sample random point inside bounds
            float rx = Random.Range(-0.5f, 0.5f);
            float ry = Random.Range(-0.5f, 0.5f);
            float rz = Random.Range(-0.5f, 0.5f);
            Vector3 samplePoint = b.center + Vector3.Scale(b.extents, new Vector3(rx, ry, rz));

            Vector3 dir = (samplePoint - eyeOrigin.position).normalized;
            float dist = Vector3.Distance(eyeOrigin.position, samplePoint);

            bool blocked = Physics.Raycast(eyeOrigin.position, dir, dist, occlusionMask);
            lastRays.Add((eyeOrigin.position, samplePoint, !blocked));

            if (!blocked)
                return true; // at least one clear line
        }
        return false;
    }

    void OnDrawGizmosSelected()
    {
        if (!drawVisionGizmos) return;
        if (eyeOrigin == null) eyeOrigin = this.transform;

        Gizmos.color = Color.yellow;
        // draw the actual rays we tested last frame
        int sampleCount = Mathf.Clamp(occlusionSamples, 1, 256);

        for (int i = 0; i < sampleCount; i++)
        {
            float yaw = Random.Range(-fieldOfView * 0.5f, fieldOfView * 0.5f);
            float pitch = Random.Range(-fieldOfView * 0.25f, fieldOfView * 0.25f);
            Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 dir = rot * eyeOrigin.forward;

            Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f);
            Gizmos.DrawLine(eyeOrigin.position, eyeOrigin.position + dir * visionRange);
        }

        // Cone boundaries
        Vector3 forward = eyeOrigin.forward;
        Vector3 leftBoundary = Quaternion.Euler(0, -fieldOfView * 0.5f, 0) * forward;
        Vector3 rightBoundary = Quaternion.Euler(0, fieldOfView * 0.5f, 0) * forward;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(eyeOrigin.position, eyeOrigin.position + leftBoundary * visionRange);
        Gizmos.DrawLine(eyeOrigin.position, eyeOrigin.position + rightBoundary * visionRange);
    }
}
