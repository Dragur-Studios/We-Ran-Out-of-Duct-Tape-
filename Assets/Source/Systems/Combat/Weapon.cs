using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

[Serializable]
public class HandSocket
{
    public Transform RootSocket;
    public Transform ThumbSocket;
    public Transform IndexSocket;
    public Transform OuterSocket;
}

public class Weapon : MonoBehaviour
{
    public HandSocket LeftHandSocket;
    public HandSocket RightHandSocket;

    public float FireRate = 240; // 240 Bullets/Sec

    [SerializeField] GameObject vfx_MuzzleFlash;

    [SerializeField] Transform muzzleOrigin;

    private void Start()
    {

    }

    void SpawnMuzzleFlash(Transform t)
    {
        var go = Instantiate(vfx_MuzzleFlash);
        go.transform.SetParent(t, false);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        // FIRES AUTOMATICALLY.
    }

    public void Fire()
    {
        // HANDLE SPAWNING OF PARTICLE EFFECT.
        SpawnMuzzleFlash(muzzleOrigin);
        // HANDLE SPAWING OF BULLET.


    }

}
