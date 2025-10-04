using System;
using UnityEngine;

public class EnviormentManager : MonoBehaviour
{
    static EnviormentManager _instance;

    public static EnviormentManager Instance { get { return _instance; } }
    //{
    //    get { 
    //        if(_instance == null)
    //        {
    //            _instance = new GameObject("Enviorment Manager(SIMULATION MODE)", typeof(EnviormentManager)).GetComponent<EnviormentManager>();
    //            var simBiom = new GameObject("Simulation Biome", typeof(Biome)).GetComponent<Biome>();
    //            simBiom.Temperature = 76;
    //            _instance.CurrentBiome = simBiom;

    //        }
    //        return _instance; 
    //    }
    //}
    
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
