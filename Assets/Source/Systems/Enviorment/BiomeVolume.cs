using UnityEngine;

public class BiomeVolume : MonoBehaviour
{
    public Biome Biome;
    private void OnTriggerEnter(Collider other)
    {
        EnviormentManager.SetCurrentBiome(Biome);
    }
}
