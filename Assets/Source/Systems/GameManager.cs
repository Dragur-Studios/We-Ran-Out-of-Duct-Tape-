using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    public GameObject Player_prefab;
    public GameObject Enemy_prefab;

    [SerializeField] bool lock_cursor = false;
    
    [SerializeField] bool start_scene_auto = false;
    [SerializeField] string scene_name = "";
    
    static PlayerSaveData testSave() => new PlayerSaveData { WorldPosition = Vector3.zero, WorldRotation = Quaternion.identity };
    public Action OnPlayerSpawned;
    public Action OnPlayerDied;

    private void Awake()
    {
        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow, new RefreshRate { numerator = 144, denominator = 1 });

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        Instance = this;

    }

    private void Start()
    {
        if (start_scene_auto && !string.IsNullOrEmpty(scene_name))
        {
            GameSceneLoader.LoadSceneCustom(scene_name);
        }
    }

    public void PlayerDied()
    {
        StartCoroutine(nameof(PlayerDeathDelayMenu));
    }

    IEnumerator PlayerDeathDelayMenu()
    {
        yield return new WaitForSeconds(2);
        OnPlayerDied?.Invoke();
        yield return null;
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


    public void SpawnEnemy(Transform spawnTransform)
    {
        var go = Instantiate(Enemy_prefab);
        go.transform.position = spawnTransform.position;
        go.transform.rotation = spawnTransform.rotation;

        var enemy = go.GetComponent<EnemyBehaviorResolver>();
    }
    public GameObject SpawnEnemy(Vector3 spawnPos, Quaternion spawnRot)
    {
        var go = Instantiate(Enemy_prefab);
        go.transform.position = spawnPos;
        go.transform.rotation = spawnRot;
   
        return go;

    }
    internal void SpawnPlayer(Transform spawnTransform)
    {
        var go = Instantiate(Player_prefab);
        go.transform.position = spawnTransform.position;
        go.transform.rotation = spawnTransform.rotation;

        player = go.GetComponent<Player>();
        player.Initilize();

        OnPlayerSpawned?.Invoke();
    }
    internal void SpawnPlayer()
    {
        var go = Instantiate(Player_prefab);
        go.transform.position = runtimeData.WorldPosition;
        go.transform.rotation = runtimeData.WorldRotation;

        player = go.GetComponent<Player>();
        player.Initilize();

        OnPlayerSpawned?.Invoke();
    }




    internal void PrepareGameCamera()
    {
        var track = player.transform.GetChild(0);
        GameCamera.Track(track);

        if(lock_cursor is true)
            HideCursor();
    }

    bool isPaused = false;

    internal static void TryPause()
    {
        Instance.HandlePause();
    }
    private void Update()
    {
        if(delayPauseTimer > 0)
        {
            delayPauseTimer -= Time.deltaTime; 
        }

        if(delayPauseTimer < 0)
        {
            delayPauseTimer = 0;
        }


    }


    float delayPauseTime = 0.3f;
    float delayPauseTimer = 0.0f;

    private void HandlePause()
    {
        if (delayPauseTimer > 0.0f) return;

        delayPauseTimer = delayPauseTime;
        isPaused = !isPaused;

        OnPause?.Invoke(isPaused);
    }

    Player player;
    public Player Player { get { return player; } }

    public Action<bool> OnPause;
}
