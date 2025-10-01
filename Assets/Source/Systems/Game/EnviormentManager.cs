using System;
using UnityEngine;

public class EnviormentManager : MonoBehaviour
{
    public static EnviormentManager Instance;
    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

    }

    public Biome CurrentBiome;

    public Action<Biome> OnBiomeChanged;

    public static void SetCurrentBiome(Biome biome)
    {
        Instance.OnBiomeChanged?.Invoke(biome);
    }
}
