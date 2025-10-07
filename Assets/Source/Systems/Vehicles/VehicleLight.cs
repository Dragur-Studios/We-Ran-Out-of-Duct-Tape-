using UnityEngine;

public class VehicleLight : MonoBehaviour
{
    [SerializeField, Range(1.0f, 100.0f)] float maxIntensity = 10.0f;
    [SerializeField] Material baseLightMaterial;
    [SerializeField] Light[] lights;

    MeshRenderer lightMesh;
    Material lightMaterial;

    [SerializeField] bool isOn = false;
    [Range(0, 1.0f)] public float intensity_t = 0.0f;
    private void Start()
    {
        lightMesh = GetComponent<MeshRenderer>();
        lightMaterial = new Material(baseLightMaterial);
        TurnOffImmediate();
        lightMesh.sharedMaterial = lightMaterial;

    }
    const string _shader_string_veh_light_intensity = "_Intensity";
    void TurnOffImmediate()
    {
        intensity_t = 0;
        isOn = false;
        lightMaterial.SetFloat(_shader_string_veh_light_intensity, 0.0f);
    }

    public void TurnOff()
    {
        isOn = false;
    }
    public void TurnOn()
    {
        isOn = true;
    }

    void SetLightIntensity(float intensity)
    {
        lightMaterial.SetFloat(_shader_string_veh_light_intensity, intensity);
        foreach (var light in lights)
        {
            light.intensity = intensity;
        }
    }

    private void Update()
    {
        float target = isOn ? 1f : 0f;
        intensity_t = Mathf.MoveTowards(intensity_t, target, 10f * Time.deltaTime);

        float targetIntensity = Mathf.Lerp(0, maxIntensity, intensity_t);
        SetLightIntensity(targetIntensity);
    }

}
