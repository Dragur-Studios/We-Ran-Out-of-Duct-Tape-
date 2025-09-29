#if UNITY_2021_2_OR_NEWER
using System.Linq;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[Overlay(typeof(SceneView), "Play From Scene (List)")]
public class PlayFromSceneListOverlay : Overlay
{
    private static string cachedScenePath;
    private static SceneAsset playModeOverrideScene;

    private ListView sceneListView;
    private SceneAsset[] allScenes;

    private const string PrefKey = "PlayFromSceneListOverlay.SelectedScenePath";

    public override VisualElement CreatePanelContent()
    {
        var root = new VisualElement { style = { flexDirection = FlexDirection.Column } };

        // --- Controls row at top, centered ---
        var controls = new VisualElement
        {
            style =
            {
                flexDirection = FlexDirection.Row,
                justifyContent = Justify.Center,
                marginBottom = 4
            }
        };

        var playButton = new ToolbarToggle { text = EditorApplication.isPlaying ? "■" : "▶" };
        playButton.AddToClassList("unity-toolbar-button");
        playButton.RegisterValueChangedCallback(evt =>
        {
            if (evt.newValue) CacheAndPlay();
            else EditorApplication.isPlaying = false;
        });

        var pauseButton = new ToolbarToggle { text = "⏸" };
        pauseButton.AddToClassList("unity-toolbar-button");
        pauseButton.RegisterValueChangedCallback(evt => EditorApplication.isPaused = evt.newValue);

        var stepButton = new ToolbarButton(() => EditorApplication.Step()) { text = "⏭" };
        stepButton.AddToClassList("unity-toolbar-button");

        EditorApplication.playModeStateChanged += _ =>
        {
            playButton.text = EditorApplication.isPlaying ? "■" : "▶";
            playButton.SetValueWithoutNotify(EditorApplication.isPlaying);
            pauseButton.SetValueWithoutNotify(EditorApplication.isPaused);
        };

        controls.Add(playButton);
        controls.Add(pauseButton);
        controls.Add(stepButton);
        root.Add(controls);

        // --- Gather all scenes (only Assets/Scenes) ---
        string[] guids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets/Scenes" });
        allScenes = guids
            .Select(g => AssetDatabase.LoadAssetAtPath<SceneAsset>(AssetDatabase.GUIDToAssetPath(g)))
            .Where(s => s != null)
            .OrderBy(s => s.name)
            .ToArray();

        if (allScenes.Length == 0)
        {
            root.Add(new Label("No scenes found in Assets/Scenes"));
            return root;
        }

        // Restore last selection from prefs
        var savedPath = EditorPrefs.GetString(PrefKey, "");
        if (!string.IsNullOrEmpty(savedPath))
        {
            playModeOverrideScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(savedPath);
        }

        // --- Scene List with Toggle + ObjectField ---
        sceneListView = new ListView
        {
            itemsSource = allScenes,
            fixedItemHeight = 22,
            selectionType = SelectionType.None,
            style = { flexGrow = 1 }
        };

        sceneListView.makeItem = () =>
        {
            var row = new VisualElement { style = { flexDirection = FlexDirection.Row } };

            var toggle = new Toggle { style = { width = 18, marginRight = 4 } };
            var field = new ObjectField
            {
                objectType = typeof(SceneAsset),
                allowSceneObjects = false,
                style = { flexGrow = 1 }
            };

            row.Add(toggle);
            row.Add(field);
            return row;
        };

        sceneListView.bindItem = (element, i) =>
        {
            var row = (VisualElement)element;
            var toggle = row.Q<Toggle>();
            var field = row.Q<ObjectField>();

            var scene = allScenes[i];
            field.value = scene;

            // Enable/disable ObjectField based on toggle
            toggle.value = (playModeOverrideScene == scene);
            field.SetEnabled(toggle.value);

            toggle.RegisterValueChangedCallback(evt =>
            {
                field.SetEnabled(evt.newValue);
                if (evt.newValue)
                {
                    playModeOverrideScene = scene;
                    EditorPrefs.SetString(PrefKey, AssetDatabase.GetAssetPath(scene));

                    // Uncheck all other toggles
                    for (int j = 0; j < sceneListView.itemsSource.Count; j++)
                    {
                        if (j == i) continue;
                        var otherRow = sceneListView.GetRootElementForIndex(j);
                        if (otherRow != null)
                        {
                            var otherToggle = otherRow.Q<Toggle>();
                            if (otherToggle != null)
                                otherToggle.SetValueWithoutNotify(false);
                            var otherField = otherRow.Q<ObjectField>();
                            if (otherField != null)
                                otherField.SetEnabled(false);
                        }
                    }
                }
                else if (playModeOverrideScene == scene)
                {
                    playModeOverrideScene = null;
                    EditorPrefs.DeleteKey(PrefKey);
                }
            });

            // Double‑click to open scene
            field.RegisterCallback<MouseDownEvent>(evt =>
            {
                if (evt.clickCount == 2 && toggle.value && scene != null)
                {
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    {
                        EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(scene));
                    }
                }
            });

            // Assigning a new scene via ObjectField
            field.RegisterValueChangedCallback(evt =>
            {
                if (toggle.value)
                {
                    playModeOverrideScene = (SceneAsset)evt.newValue;
                    if (playModeOverrideScene != null)
                        EditorPrefs.SetString(PrefKey, AssetDatabase.GetAssetPath(playModeOverrideScene));
                }
            });
        };

        root.Add(sceneListView);
        return root;
    }

    private void CacheAndPlay()
    {
        if (playModeOverrideScene == null) return;

        var currentScene = EditorSceneManager.GetActiveScene();
        if (currentScene.isDirty)
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return;
        }
        cachedScenePath = currentScene.path;

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
