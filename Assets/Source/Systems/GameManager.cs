using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Singleton;

    public GameObject Player_prefab;

    [SerializeField] bool lock_cursor = false;
    
    [SerializeField] bool playground_scene_auto = false;

    static PlayerSaveData testSave() => new PlayerSaveData { WorldPosition = Vector3.zero, WorldRotation = Quaternion.identity };

    private void Awake()
    {
        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow, new RefreshRate { numerator = 144, denominator = 1 });

        if (Singleton != null && Singleton != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        Singleton = this;

    }

    private void Start()
    {
        if (playground_scene_auto)
        {
            GameSceneLoader.LoadSceneCustom("Playground");
        }
    }

    public void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ShowCursor(bool isFree = false)
    {
        Cursor.lockState = isFree ? CursorLockMode.None : CursorLockMode.Confined;
        Cursor.visible = true;
    }

    PlayerSaveData runtimeData;

    public void LoadGame()
    {
        runtimeData = testSave();
    }

    public void SaveGame()
    {

    }

    internal void SpawnPlayer(Transform spawnTransform)
    {
        var go = Instantiate(Player_prefab);
        go.transform.position = spawnTransform.position;
        go.transform.rotation = spawnTransform.rotation;

        player = go.GetComponent<Player>();
        player.Initilize();
    }
    internal void SpawnPlayer()
    {
        var go = Instantiate(Player_prefab);
        go.transform.position = runtimeData.WorldPosition;
        go.transform.rotation = runtimeData.WorldRotation;

        player = go.GetComponent<Player>();
        player.Initilize();


    }




    internal void PrepareGameCamera()
    {
        var track = player.transform.GetChild(0);
        GameCamera.Track(track);

        if(lock_cursor is true)
            HideCursor();
    }

    Player player;
    public Player Player { get { return player; } }
}
