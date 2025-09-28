using UnityEngine;

public class SceneInitilizer : MonoBehaviour
{

    [SerializeField] Transform spawnTransform;

    void Start()
    {
        var gm = GameManager.Singleton;
        gm.LoadGame();
        if(transform == null)
        {
            gm.SpawnPlayer();
        }
        else
        {
            gm.SpawnPlayer(spawnTransform);
        }
        gm.PrepareGameCamera();
    }

}
