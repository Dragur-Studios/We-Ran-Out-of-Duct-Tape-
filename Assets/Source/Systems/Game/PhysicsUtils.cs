using UnityEngine;

public static class PhysicsUtils
{
    // Universal gas constant (J/(mol·K))
    private const float R = 8.314462618f;

    /// <summary>
    /// Calculates the speed of sound in an ideal gas.
    /// </summary>
    /// <param name="gamma">Adiabatic index (Cp/Cv). For air ~1.4</param>
    /// <param name="temperatureK">Temperature in Kelvin</param>
    /// <param name="molarMass">Molar mass of the gas in kg/mol. For dry air ~0.02897</param>
    /// <returns>Speed of sound in m/s</returns>
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

