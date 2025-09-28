using System;
using Unity.Cinemachine;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    public static GameCamera Singleton;
    CinemachineCamera cam;


    private void Awake()
    {
        if(Singleton != null && Singleton != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);        
        Singleton = this;
    }

    private void Start()
    {
        cam = GetComponentInChildren<CinemachineCamera>();
    }

    void LinkTrackingTarget(Transform target)
    {
        cam.Target.TrackingTarget = target;
    }

    public static void Track(Transform target)
    {
        Singleton.LinkTrackingTarget(target);
    }
}
