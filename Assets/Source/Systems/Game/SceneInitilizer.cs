using UnityEngine;

public class SceneInitilizer : MonoBehaviour
{

    [SerializeField] Transform spawnTransform;

    void Start()
    {
        var gm = GameManager.Instance;
        gm.LoadGame();

        if(spawnTransform == null)
        {
            gm.SpawnPlayer();
        }
        else
        {
            gm.SpawnPlayer(spawnTransform);
        }
        
        gm.TrackPlayer();
    }

}
