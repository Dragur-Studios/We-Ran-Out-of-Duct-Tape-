using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    public HandSocket LeftHandSocket;
    public HandSocket RightHandSocket;

    [SerializeField] GameObject vfx_MuzzleFlash;

    [SerializeField] Transform muzzleOrigin;
    [SerializeField] GameObject bulletPrefab;

    [SerializeField] WeaponData data;
    public float FireRate { get => data.FireRate; }

    private void Start()
    {

    }

    void SpawnMuzzleFlash(Transform t)
    {
        var go = Instantiate(vfx_MuzzleFlash);
        go.transform.SetParent(t, false);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.SetParent(null);
        // FIRES AUTOMATICALLY.
    }

    public void Fire()
    {

        Gamepad pad = Gamepad.all.Count > 0 ? Gamepad.all[0] : null;

        var pkt = new FeedbackPacket()
        {
            CameraShakeAmplitude = data.ShakeAmplitude,
            CameraShakeFrequency = data.ShakeFrequency,
            CameraShakeDuration = data.ShakeDuration,

            IsGamepad = pad != null,
            GamepadMotorLowFrequency = data.GamepadLowFrequency,
            GamepadMotorHighFrequency = data.GamepadHighFrequency,
            GamepadMotorFeedbackDuration = data.GamepadVibrateDuration,
        };

        Haptics.ApplyFeedback(pkt);

        // HANDLE SPAWNING OF PARTICLE EFFECT.
        SpawnMuzzleFlash(muzzleOrigin);
        // HANDLE SPAWING OF BULLET.
        SpawnBullet(muzzleOrigin);

    }

    private void SpawnBullet(Transform muzzleOrigin)
    {
        var go = Instantiate(bulletPrefab);
        go.transform.position = muzzleOrigin.position;
        go.transform.rotation = muzzleOrigin.rotation;
    }
}
