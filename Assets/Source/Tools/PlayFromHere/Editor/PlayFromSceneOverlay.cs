//#if UNITY_2021_2_OR_NEWER
//using System.Reflection;
//using UnityEditor;
//using UnityEditor.Overlays;
//using UnityEditor.SceneManagement;
//using UnityEditor.UIElements;
//using UnityEngine;
//using UnityEngine.UIElements;

//[Overlay(typeof(SceneView), "Play Mode Override")]
//public class PlayFromSceneOverlay : Overlay
//{
//    private SceneAsset sceneToPlay;
//    private static string cachedScenePath;
//    private static SceneAsset playModeOverrideScene;

//    public override VisualElement CreatePanelContent()
//    {
//        var root = new VisualElement { style = { flexDirection = FlexDirection.Row } };

//        // Auto‑assign "Main Menu" if found
//        if (sceneToPlay == null)
//        {
//            string[] guids = AssetDatabase.FindAssets("t:Scene Main Menu");
//            if (guids.Length > 0)
//            {
//                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
//                sceneToPlay = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
//            }
//        }

//        var objectField = new ObjectField("Start Scene")
//        {
//            objectType = typeof(SceneAsset),
//            allowSceneObjects = false,
//            style = { flexGrow = 1 },
//            value = sceneToPlay
//        };
//        objectField.RegisterValueChangedCallback(evt => sceneToPlay = (SceneAsset)evt.newValue);

//        // --- Play Button ---
//        var playButton = new ToolbarToggle
//        {
//            style = { width = 24, height = 24 }
//        };
//        playButton.AddToClassList("unity-toolbar-button");
//        playButton.text = EditorApplication.isPlaying ? "■" : "▶";
//        playButton.value = EditorApplication.isPlaying;

//        playButton.RegisterValueChangedCallback(evt =>
//        {
//            if (evt.newValue)
//                CacheAndPlay();
//            else
//                EditorApplication.isPlaying = false;
//        });

//        EditorApplication.playModeStateChanged += _ =>
//        {
//            playButton.text = EditorApplication.isPlaying ? "■" : "▶";
//            playButton.SetValueWithoutNotify(EditorApplication.isPlaying);
//        };

//        // --- Play Mode Behavior Button (Maximize toggle) ---
//        Texture2D fullscreenTex = Resources.Load<Texture2D>("Expand");
//        var fullscreenButton = new ToolbarToggle
//        {
//            style = { width = 24, height = 24 }
//        };
//        fullscreenButton.AddToClassList("unity-toolbar-button");
//        if (fullscreenTex != null)
//            fullscreenButton.style.backgroundImage = new StyleBackground(fullscreenTex);

//        // Sync with current setting
//        fullscreenButton.value = GetPlayModeBehavior() == 1;

//        fullscreenButton.RegisterValueChangedCallback(evt =>
//        {
//            SetPlayModeBehavior(evt.newValue ? 1 : 0);
//        });

//        root.Add(objectField);
//        root.Add(playButton);
//        root.Add(fullscreenButton);
//        return root;
//    }

//    private void SetPlayModeBehavior(int mode)
//    {
//        var gameViewType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
//        var prop = gameViewType.GetProperty("playModeBehavior",
//            BindingFlags.NonPublic | BindingFlags.Static);
//        if (prop != null)
//            prop.SetValue(null, mode, null);

//        // Also persist to EditorPrefs so Unity remembers across sessions
//        EditorPrefs.SetInt("PlayModeBehavior", mode);
//    }

//    private int GetPlayModeBehavior()
//    {
//        return EditorPrefs.GetInt("PlayModeBehavior", 0);
//    }

//    private void CacheAndPlay()
//    {
//        if (sceneToPlay == null) return;

//        var currentScene = EditorSceneManager.GetActiveScene();
//        if (currentScene.isDirty)
//        {
//            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
//                return;
//        }
//        cachedScenePath = currentScene.path;

//        playModeOverrideScene = sceneToPlay;
//        EditorSceneManager.playModeStartScene = playModeOverrideScene;

//        EditorApplication.isPlaying = true;
//        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
//    }

//    private void OnPlayModeStateChanged(PlayModeStateChange state)
//    {
//        if (state == PlayModeStateChange.EnteredEditMode)
//        {
//            EditorSceneManager.playModeStartScene = null;

//            if (!string.IsNullOrEmpty(cachedScenePath))
//            {
//                EditorSceneManager.OpenScene(cachedScenePath);
//                cachedScenePath = null;
//            }

//            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
//        }

//        if(state == PlayModeStateChange.EnteredPlayMode)
//        {

//            var gameViewType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
//            var wnd = EditorWindow.GetWindow(gameViewType);
//            int m = EditorPrefs.GetInt("PlayModeBehavior");
//            wnd.maximized = (m == 1) ? true : false;
//        }
//    }
//}
//#endif
#if UNITY_2021_2_OR_NEWER
using System.Reflection;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine.UIElements;


// This class never gets shown, it's just a proxy marker

[Overlay(typeof(SceneView), "Start Override")]
public class PlayFromSceneOverlay : Overlay
{
    private SceneAsset sceneToPlay;
    private static string cachedScenePath;
    private static SceneAsset playModeOverrideScene;

    public override VisualElement CreatePanelContent()
    {
        var root = new VisualElement { style = { flexDirection = FlexDirection.Row } };

        // Auto‑assign "Main Menu" if found
        if (sceneToPlay == null)
        {
            string[] guids = AssetDatabase.FindAssets("t:Scene Main Menu");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                sceneToPlay = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
            }
        }

        var objectField = new ObjectField("")
        {
            objectType = typeof(SceneAsset),
            allowSceneObjects = false,
            style = { flexGrow = 1 },
            value = sceneToPlay
        };
        objectField.RegisterValueChangedCallback(evt => sceneToPlay = (SceneAsset)evt.newValue);

        // --- Play Button ---
        var playButton = new ToolbarToggle { style = { width = 24, height = 24 } };
        playButton.AddToClassList("unity-toolbar-button");
        playButton.text = EditorApplication.isPlaying ? "■" : "▶";
        playButton.value = EditorApplication.isPlaying;

        playButton.RegisterValueChangedCallback(evt =>
        {
            if (evt.newValue)
                CacheAndPlay();
            else
                EditorApplication.isPlaying = false;
        });

        EditorApplication.playModeStateChanged += _ =>
        {
            playButton.text = EditorApplication.isPlaying ? "■" : "▶";
            playButton.SetValueWithoutNotify(EditorApplication.isPlaying);
        };



        root.Add(objectField);
        root.Add(playButton);
        return root;
    }


    private void CacheAndPlay()
    {
        if (sceneToPlay == null) return;

        var currentScene = EditorSceneManager.GetActiveScene();
        if (currentScene.isDirty)
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return;
        }
        cachedScenePath = currentScene.path;

        playModeOverrideScene = sceneToPlay;
        EditorSceneManager.playModeStartScene = playModeOverrideScene;

        EditorApplication.isPlaying = true;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredEditMode)
        {
            EditorSceneManager.playModeStartScene = null;

            if (!string.IsNullOrEmpty(cachedScenePath))
            {
                EditorSceneManager.OpenScene(cachedScenePath);
                cachedScenePath = null;
            }

            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }
    }
}
#endif

