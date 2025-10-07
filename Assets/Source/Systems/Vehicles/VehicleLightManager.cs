using System;
using UnityEngine;

public class VehicleLightManager : MonoBehaviour
{
    VehicleLight[] lights;

    VehicleController vehicle;
    bool on = false;
    public void ToggleLights()
    {
        on = !on;
    }

    private void Start()
    {
        lights = GetComponentsInChildren<VehicleLight>();


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
    }

    void TurnOff()
    {
        on = false;
        
        foreach (VehicleLight light in lights) 
        {
            light.TurnOff();
        }
    }

    void TurnOn()
    {
        on = true;
        
        foreach (VehicleLight light in lights)
        {
            light.TurnOn();
        }
    }
}
