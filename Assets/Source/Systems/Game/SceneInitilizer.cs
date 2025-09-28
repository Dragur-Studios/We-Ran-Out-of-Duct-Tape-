using UnityEngine;

public class SceneInitilizer : MonoBehaviour
{

    void Start()
    {
        var gm = GameManager.Singleton;
        gm.LoadGame();
        gm.SpawnPlayer();
        gm.PrepareGameCamera();
    }

}
