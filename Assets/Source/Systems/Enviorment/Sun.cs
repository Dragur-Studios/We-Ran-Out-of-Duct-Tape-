using UnityEngine;

public class Sun : MonoBehaviour
{
    [Tooltip("Degrees per second. 360 = full rotation per second.")]
    public float degreesPerSecond = 10f;

    [Tooltip("Local axis to rotate around.")]
    public Vector3 rotationAxis = Vector3.right;

    [Tooltip("If true rotation is paused.")]
    public bool paused = false;

    void Reset()
    {
        rotationAxis = Vector3.right;
        degreesPerSecond = 10f;
    }

    void Update()
    {
        if (paused) return;
        transform.Rotate(rotationAxis.normalized, degreesPerSecond * Time.deltaTime, Space.Self);
    }
}
