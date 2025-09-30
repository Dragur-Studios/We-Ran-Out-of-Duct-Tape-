using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameOverEvents : MonoBehaviour
{
    UIDocument gameOverScreen;

    VisualElement root;
    Button respawnBTN;

    private void Start()
    {
        gameOverScreen = GetComponent<UIDocument>();

       
        
        GameManager.Instance.OnPlayerDied += () =>
        {
            gameObject.SetActive(true);

            root = gameOverScreen.rootVisualElement;

            var el = root.Q("RespawnAtCampButton");
            respawnBTN = el as Button;

            respawnBTN.clicked += (() => HandleTemporaryRespawnEvent());
        };

        GameManager.Instance.OnPlayerSpawned += () =>
        {
            gameObject.SetActive(false);
        };

        gameObject.SetActive(false);

    }

    void HandleTemporaryRespawnEvent()
    {
        // for now just reload the scene..
        var scene = SceneManager.GetActiveScene().name;
        GameSceneLoader.LoadSceneCustom($"{scene}");

    }

}
