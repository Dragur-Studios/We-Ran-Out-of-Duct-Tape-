using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class Haptics : MonoBehaviour
{
    public static Haptics Instance;

    CinemachineBasicMultiChannelPerlin cameraShaker;


    float gamepadLowFrequency;
    float gamepadHighFrequency;

    float cameraShakeTimer;
    float gamepadVibrateTimer;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        cameraShaker = GetComponentInChildren<CinemachineBasicMultiChannelPerlin>();

        if (Gamepad.current != null)
        {
            Debug.Log("Forcing rumble test...");
            Gamepad.current.SetMotorSpeeds(0.5f, 1.0f);
            Invoke(nameof(StopRumble), 1.0f);
        }
    }

    private void StopRumble()
    {
        if (Gamepad.current != null)
            Gamepad.current.SetMotorSpeeds(0, 0);
    }

    private void Update()
    {
        if (cameraShakeTimer > 0)
        {
            cameraShakeTimer -= Time.deltaTime;
            if (cameraShakeTimer < 0)
            {
                cameraShakeTimer = 0;

                cameraShaker.AmplitudeGain = 0.0f;
                cameraShaker.FrequencyGain = 0.0f;
            }
        }


        if(gamepadVibrateTimer > 0)
        {
            gamepadVibrateTimer -= Time.deltaTime;
            
            if(gamepadVibrateTimer <= 0)
            {
                gamepadVibrateTimer = 0;

                gamepadLowFrequency = 0;
                gamepadHighFrequency = 0;
            }
        }

        if(Gamepad.current != null)
            Gamepad.current.SetMotorSpeeds(gamepadLowFrequency, gamepadHighFrequency);
    }

    void StartShakingCamera(float amplitude, float frequency, float duration)
    {
        cameraShaker.AmplitudeGain = amplitude;
        cameraShaker.FrequencyGain = frequency;

        cameraShakeTimer = duration;
    }
    void StartVibratingController(float lowFreq, float highFreq, float duration)
    {
        gamepadHighFrequency = highFreq;
        gamepadLowFrequency = lowFreq;

        gamepadVibrateTimer = duration;
    }

    public static void ApplyFeedback(FeedbackPacket pkt)
    {
        if (pkt.IsGamepad)
            Instance.StartVibratingController(pkt.GamepadMotorLowFrequency, pkt.GamepadMotorHighFrequency, pkt.GamepadMotorFeedbackDuration);

        Instance.StartShakingCamera(pkt.CameraShakeAmplitude, pkt.CameraShakeFrequency, pkt.CameraShakeDuration);

    }




}
