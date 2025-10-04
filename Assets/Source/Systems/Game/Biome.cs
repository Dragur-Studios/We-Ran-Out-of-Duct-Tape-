using UnityEngine;

public class Biome : MonoBehaviour
{
    public float Temperature;

    private void OnTriggerEnter(Collider other)
    {
        EnviormentManager.SetCurrentBiome(this);
    }
}
