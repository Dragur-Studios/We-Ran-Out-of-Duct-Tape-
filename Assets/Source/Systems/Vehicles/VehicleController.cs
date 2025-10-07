using System;
using Unity.VisualScripting;
using UnityEngine;


public class VehicleController : MonoBehaviour
{
    public Transform FollowTarget { get; private set; }

    public Action<bool> OnIgnition;

    VehicleMotor motor;
    GameInputReciever input;
    public void TurnOn()
    {
        OnIgnition?.Invoke(true);
        isPlayerInVehicle = true;
    }

    public void TurnOff()
    {
        OnIgnition?.Invoke(false);
        isPlayerInVehicle = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        motor = GetComponent<VehicleMotor>();
        input = GameInputReciever.Instance;

        FollowTarget = transform.GetChild(0);
        
    }


    // translate Game inputs to motor...
    // if holding the gas button ramp up to 1.0f (MAX THROTTLE)
    // if brake is pressed exp rap up to 1.0f (MAX BRAKE)
    // if brake is held and is stopped. reverse.

    bool isPlayerInVehicle = false;

    void Update()
    {

        if (isPlayerInVehicle)
        {
            if (input.Exit)
            {
                isPlayerInVehicle = false;
                GameVehicleManager.ExitVehicle(this);
                return;
            }

            motor.SetThrottle(input.Gas);
            motor.SetBrake(input.Brake);
            motor.SetSteeringAngle(input.Steering);

            if(input.Lights && toggleLightTimer <= 0)
            {
                toggleLightTimer = 0.3f;
                var lm = GetComponent<VehicleLightManager>();
                lm.ToggleLights();
            }
            if(toggleLightTimer > 0)
            {
                toggleLightTimer -= Time.deltaTime;
            }
        }
    }

    float toggleLightTimer = 0.0f;

}
