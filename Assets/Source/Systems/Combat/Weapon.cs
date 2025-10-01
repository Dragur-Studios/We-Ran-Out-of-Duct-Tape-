using System;
using UnityEngine;
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
    
    [SerializeField] GameObject laserPointer;
    
    float y = 0;
    
    bool laserEnabled = false;

    public void EnableLaserPointer()
    {
        laserEnabled = true;
    }

    public void DisableLaserPointer()
    {
        laserEnabled = false;
    }

    private void Update()
    {
        laserPointer.SetActive(laserEnabled);
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
        // HANDLE SPAWING OF BULLET.
        SpawnBullet(muzzleOrigin);


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
        GetComponent<SoundWaveEmitter>().Emit();
       
    }

    private void SpawnBullet(Transform muzzleOrigin)
    {
        // SAVE Y before rotation happens...
        y = muzzleOrigin.position.y;

        var go = Instantiate(bulletPrefab);
        var pos = muzzleOrigin.position;
        pos.y = y;
        go.transform.position = pos;

        go.transform.rotation = muzzleOrigin.rotation;

        Destroy(go, 2);
    }
}
