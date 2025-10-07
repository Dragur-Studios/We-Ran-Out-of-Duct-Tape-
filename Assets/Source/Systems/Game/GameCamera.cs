using System;
using Unity.Cinemachine;
using UnityEngine;

[Serializable]
public class FeedbackPacket
{
    public float CameraShakeFrequency;
    public float CameraShakeAmplitude;
    public float CameraShakeDuration;

    public bool IsGamepad;
    public float GamepadMotorLowFrequency;
    public float GamepadMotorHighFrequency;
    public float GamepadMotorFeedbackDuration;

}


public class GameCamera : MonoBehaviour
{
    public static GameCamera Instance;
    CinemachineCamera cam;
    CinemachineBasicMultiChannelPerlin cameraShaker;
    public Camera Camera { get => Camera.main; }
    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);        

        Instance = this;
       
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
        Instance.LinkTrackingTarget(target);
    }
    
    public static Camera GetCamera()
    {
        return Instance?.Camera;
    }
}
