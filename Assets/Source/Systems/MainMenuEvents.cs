using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public static class GameSceneLoader
{
    static string MAIN_MENU = "MAIN_MENU";
    
    static string SCENE_001 = "SCENE_001";

    public static void LoadMainMenu()
    {
        SceneManager.LoadScene(MAIN_MENU);
    }

    public static void LoadSceneOne()
    {
        SceneManager.LoadScene(SCENE_001);
    }

    public static void LoadSceneCustom(string name)
    {
        SceneManager.LoadScene(name);
    }
}

public class MainMenuEvents : MonoBehaviour
{
    UIDocument doc;

    Button btn;
    TextField textField;
    Toggle toggle;

    string customSceneName = "Playground";
    bool allowCustomScenes = false;

    private void Start()
    {
         doc = GetComponent<UIDocument>();

        btn = doc.rootVisualElement.Q("BTN_Play") as Button;
        btn.clicked += () =>
        {
            if (allowCustomScenes)
                GameSceneLoader.LoadSceneCustom(customSceneName);
            else
                GameSceneLoader.LoadSceneOne();
        };

        textField = doc.rootVisualElement.Q("TXF_CustomSceneName") as TextField;
        textField.value = customSceneName;

        textField.RegisterValueChangedCallback((str) => 
        {
            customSceneName = str.newValue;
        });
        textField.SetEnabled(allowCustomScenes);
        
        toggle = doc.rootVisualElement.Q("TGL_AllowCustomScene") as Toggle;
        toggle.RegisterValueChangedCallback((on) => 
        {
            if (on.newValue) // is the toggle now true?
            {
                allowCustomScenes = true;
            }
            else
            {
                allowCustomScenes = false;
            }

           
        });
        



        btn = doc.rootVisualElement.Q("BTN_QuitGame") as Button;
        btn.clicked += () => 
        {
            #if UNITY_EDITOR
                        // In the Editor: stop play mode
                        EditorApplication.isPlaying = false;
            #else
                // In a built game: quit application
                Application.Quit();
            #endif
        };

    }

    private void Update()
    {
        textField.SetEnabled(allowCustomScenes);
    }
}
