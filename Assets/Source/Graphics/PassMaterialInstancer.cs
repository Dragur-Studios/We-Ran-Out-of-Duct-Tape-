using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

[ExecuteAlways] // so it works in editor and play mode
public class PassMaterialInstancer : MonoBehaviour
{
    [SerializeField] CustomPassVolume passVolume;   // drag your CustomPassVolume here
    [SerializeField] Material sourceMaterial;       // the base material (e.g. ObjectOutline)

    Material runtimeInstance;

    void OnEnable()
    {
        CreateAndAssignInstance();
    }

    void OnValidate()
    {
        if (isActiveAndEnabled)
            CreateAndAssignInstance();
    }

    void OnDisable()
    {
        if (runtimeInstance != null)
        {
            DestroyImmediate(runtimeInstance);
            runtimeInstance = null;
        }
    }

    void CreateAndAssignInstance()
    {
        if (passVolume == null || sourceMaterial == null)
            return;

        // Clean up old instance
        if (runtimeInstance != null)
            DestroyImmediate(runtimeInstance);

        // Create a unique runtime copy
        runtimeInstance = new Material(sourceMaterial)
        {
            name = sourceMaterial.name + " (HDRP Runtime Instance)"
        };

        // Find only the pass named "Outline Pass"
        foreach (var pass in passVolume.customPasses)
        {
            if (pass is DrawRenderersCustomPass drawPass && pass.name == "Outline Pass")
            {
                drawPass.overrideMaterial = runtimeInstance;
            }
        }
    }
}