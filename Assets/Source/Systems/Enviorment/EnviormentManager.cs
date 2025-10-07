using System;
using UnityEngine;

public class EnviormentManager : MonoBehaviour
{
    static EnviormentManager _instance;

    public static EnviormentManager Instance { get { return _instance; } }
   
    
    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

    }

    public Biome CurrentBiome;

    public Action<Biome> OnBiomeChanged;

    public static void SetCurrentBiome(Biome biome)
    {
        Instance.OnBiomeChanged?.Invoke(biome);
    }
}
