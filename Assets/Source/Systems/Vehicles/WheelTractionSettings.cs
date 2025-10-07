using UnityEngine;

[System.Serializable]
public class WheelTractionSettings
{
    [Range(0.1f, 5f)] public float forwardStiffness = 1.0f;
    [Range(0.1f, 5f)] public float sidewaysStiffness = 1.0f;
}

