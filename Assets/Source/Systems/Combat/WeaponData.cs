using UnityEngine;

[CreateAssetMenu(menuName ="Database/WeaponData")]
public class WeaponData : ScriptableObject
{
    public float FireRate = 240; // 240 Bullets/Sec


    [Header("Haptics Settings")]
    public float ShakeAmplitude = 5.0f;
    public float ShakeFrequency = 1.0f;
    public float ShakeDuration = 0.1f;

    public float GamepadLowFrequency = 0.1f;
    public float GamepadHighFrequency = 0.3f;
    public float GamepadVibrateDuration = 0.1f;
}
