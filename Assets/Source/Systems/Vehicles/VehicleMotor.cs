using System;
using Unity.VisualScripting;
using UnityEngine;

public enum DrivetrainType
{
    // steering from front axle
    FFWD,   // Front steer + Front motor
    FRWD,   // Front steer + Rear motor
    FAWD,   // Front steer + Both axles motor

    // steering from rear axle
    RFWD,   // Rear steer + Front motor
    RRWD,   // Rear steer + Rear motor
    RAWD,   // Rear steer + Both axles motor

    // steering from both axles
    AFWD,   // All steer + Front motor
    ARWD,   // All steer + Rear motor
    AAWD,   // All steer + Both axles motor

    Custom  // Manual inspector setup
}

public class VehicleMotor : MonoBehaviour
{
    bool motorEnabled = false;

    [Header("Input")]
    [Range(-1f, 1f)] public float throttlePressure = 0.0f;
    [Range(0f, 1f)] public float brakePressure = 0.0f;
    [Range(-1f, 1f)] public float steeringAngle = 0.0f;


    [Header("Vehicle Settings")]
    public float brakeStrength = 5000.0f;
    public float motorTorque = 1500.0f;
    public float maxSteerAngle = 30.0f;

    [SerializeField] private float engineBraking = 200f; // tweak to taste

    [Header("Axles")]
    public VehicleAxle[] axles = new VehicleAxle[2];

    [Header("Traction")]
    public WheelTractionSettings traction = new WheelTractionSettings();

    [Header("Drivetrain")]
    public DrivetrainType drivetrain = DrivetrainType.FFWD;

    [Header("Physics")]
    [SerializeField] private float rollingResistance = 0.02f;
    [Header("Steering Feel")]
    [SerializeField] private float steerReturnSpeed = 5f;   // how fast it centers
    [SerializeField] private float steerResponsiveness = 10f; // how fast it follows input

    VehicleController vehicle;
    void Start()
    {
        vehicle = GetComponent<VehicleController>();
        vehicle.OnIgnition += (on) =>
        {
            if (on)
            {
                TurnOn();
            }
            else
            {
                TurnOff();
            }
        };

        
        ApplyDrivetrainSetup();
    }

    public void TurnOn()
    {
      
        motorEnabled = true;

    }

    public void TurnOff()
    {
        motorEnabled = false;
    }

    void ApplyDrivetrainSetup()
    {
        var rb = GetComponent<Rigidbody>();
        rb.AddForce(-rb.linearVelocity * rollingResistance, ForceMode.Acceleration);

        if (axles == null || axles.Length < 2) return;

        // Reset all axles
        foreach (var axle in axles)
        {
            axle.motor = false;
            axle.steering = false;
        }

        switch (drivetrain)
        {
            // --- Front steer ---
            case DrivetrainType.FFWD:
                foreach (var axle in axles)
                    if (axle.Location == AxelLocation.Front)
                    { axle.motor = true; axle.steering = true; }
                break;

            case DrivetrainType.FRWD:
                foreach (var axle in axles)
                {
                    if (axle.Location == AxelLocation.Front) axle.steering = true;
                    if (axle.Location == AxelLocation.Rear) axle.motor = true;
                }
                break;

            case DrivetrainType.FAWD:
                foreach (var axle in axles)
                {
                    axle.motor = true;
                    if (axle.Location == AxelLocation.Front) axle.steering = true;
                }
                break;

            // --- Rear steer ---
            case DrivetrainType.RFWD:
                foreach (var axle in axles)
                {
                    if (axle.Location == AxelLocation.Rear) axle.steering = true;
                    if (axle.Location == AxelLocation.Front) axle.motor = true;
                }
                break;

            case DrivetrainType.RRWD:
                foreach (var axle in axles)
                    if (axle.Location == AxelLocation.Rear)
                    { axle.motor = true; axle.steering = true; }
                break;

            case DrivetrainType.RAWD:
                foreach (var axle in axles)
                {
                    axle.motor = true;
                    if (axle.Location == AxelLocation.Rear) axle.steering = true;
                }
                break;

            // --- All steer ---
            case DrivetrainType.AFWD:
                foreach (var axle in axles)
                {
                    if (axle.Location == AxelLocation.Front) axle.motor = true;
                    axle.steering = true;
                }
                break;

            case DrivetrainType.ARWD:
                foreach (var axle in axles)
                {
                    if (axle.Location == AxelLocation.Rear) axle.motor = true;
                    axle.steering = true;
                }
                break;

            case DrivetrainType.AAWD:
                foreach (var axle in axles)
                {
                    axle.motor = true;
                    axle.steering = true;
                }
                break;

            case DrivetrainType.Custom:
                // Leave inspector settings as-is
                break;
        }
    }

    void FixedUpdate()
    {

        // POWERED OFF... MAKE IT STOP (E-BRAKE)
        if (!motorEnabled)
        {
            foreach (var axle in axles)
            {
                {
                    foreach (var wheel in axle.Wheels)
                    {
                        wheel.Collider.motorTorque = 0;
                        wheel.Collider.brakeTorque = 9999;

                        wheel.Sync();

                    }
                }


            }



        }
        else
        {

            // POWERED ON... ALLOW USER INPUT...
            foreach (var axle in axles)
            {
                foreach (var wheel in axle.Wheels)
                {
                    if (axle.steering)
                        wheel.Collider.steerAngle = steeringAngle * maxSteerAngle;

                    if (axle.motor)
                    {
                        if (Mathf.Abs(throttlePressure) > 0.01f)
                        {
                            wheel.Collider.motorTorque = throttlePressure * motorTorque;
                        }
                        else
                        {
                            wheel.Collider.motorTorque = 0f;
                            wheel.Collider.brakeTorque = engineBraking;
                        }
                    }
                    else
                    {
                        wheel.Collider.motorTorque = 0f;
                    }


                    wheel.Collider.brakeTorque = brakePressure * brakeStrength;

                    wheel.ApplyTraction(traction);
                    wheel.Sync();

                }
            }
        }
    }

    public void SetThrottle(float throttle)
    {
        throttlePressure = throttle;
    }

    public void SetBrake(float brake)
    {
        brakePressure = brake;
    }

    public void SetSteeringAngle(float steering)
    {
        steeringAngle = steering;
    }
}
