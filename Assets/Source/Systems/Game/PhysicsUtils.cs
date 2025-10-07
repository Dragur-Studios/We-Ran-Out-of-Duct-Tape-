using UnityEngine;

public static class PhysicsUtils
{
    private const float R = 8.314462618f;

    public static float SpeedOfSound(float gamma, float temperatureK, float molarMass)
    {
        return Mathf.Sqrt(gamma * R * temperatureK / molarMass);
    }

    public static float CelsiusToKelvin(float celsius)
    {
        return celsius + 273.15f;
    }

    public static float FahrenheitToKelvin(float fahrenheit)
    {
        return (fahrenheit - 32.0f) * 5.0f / 9.0f + 273.15f;
    }

    public static float FarenheitToCelcius(float fahrenheit)
    {
        return (fahrenheit - 32.0f) * 5.0f / 9.0f;
    }
    public static float CelsiusToFahrenheit(float celsius)
    {
        return (celsius * 9.0f / 5.0f) + 32.0f;
    }

}

