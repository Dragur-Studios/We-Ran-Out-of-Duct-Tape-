#if UNITY_2021_2_OR_NEWER
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[Overlay(typeof(SceneView), "Build Settings Overlay")]
public class BuildSettingsOverlay : Overlay
{
    // Simple data model mirroring EditorBuildSettingsScene
    private class BuildSceneEntry
    {
        public string path;
        public bool enabled;
        public string Name => string.IsNullOrEmpty(path) ? "(None)" : System.IO.Path.GetFileNameWithoutExtension(path);
    }

    private ListView sceneListView;
    private List<BuildSceneEntry> buildScenes = new List<BuildSceneEntry>();

    public override VisualElement CreatePanelContent()
    {
        var root = new VisualElement { style = { flexDirection = FlexDirection.Column } };

        // Controls row
        var controls = new VisualElement
        {
            style =
            {
                flexDirection = FlexDirection.Row,
                justifyContent = Justify.Center,
                marginBottom = 4,
            }
        };

        var buildButton = new ToolbarButton(BuildAndRun) { text = "▶ Build & Run" };
        buildButton.AddToClassList("unity-toolbar-button");

        var refreshButton = new ToolbarButton(RefreshFromBuildSettings) { text = "⟳" };
        refreshButton.AddToClassList("unity-toolbar-button");

        controls.Add(buildButton);
        controls.Add(refreshButton);
        root.Add(controls);

        // Load from Build Settings
        RefreshFromBuildSettings();

        // ListView setup
        sceneListView = new ListView(buildScenes, itemHeight: 22, makeItem: MakeItem, bindItem: BindItem)
        {
            reorderable = true,
            showBoundCollectionSize = false,
            showFoldoutHeader = false,
            virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
            selectionType = SelectionType.None,
            style = { flexGrow = 1 }
        };

        // Handle reorder: adjust our list and commit
        sceneListView.itemIndexChanged += (oldIndex, newIndex) =>
        {
            if (oldIndex == newIndex || oldIndex < 0 || newIndex < 0 || oldIndex >= buildScenes.Count || newIndex >= buildScenes.Count)
                return;

            var item = buildScenes[oldIndex];
            buildScenes.RemoveAt(oldIndex);
            buildScenes.Insert(newIndex, item);
            CommitToBuildSettings();
            sceneListView.Rebuild();
        };

        root.Add(sceneListView);

        // Add current scene if missing
        var currentScene = EditorSceneManager.GetActiveScene();
        if (!string.IsNullOrEmpty(currentScene.path))
        {
            bool exists = buildScenes.Any(s => s.path == currentScene.path);
            if (!exists)
            {
                var addButton = new Button(() =>
                {
                    buildScenes.Add(new BuildSceneEntry
                    {
                        path = currentScene.path,
                        enabled = true
                    });
                    CommitToBuildSettings();
                    sceneListView.Rebuild();
                })
                { text = $"Add Current Scene ({currentScene.name})" };

                root.Add(addButton);
            }
        }

        return root;
    }

    private VisualElement MakeItem()
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
    }

    private void BindItem(VisualElement element, int index)
    {
        if (index < 0 || index >= buildScenes.Count) return;

        var data = buildScenes[index];
        var toggle = element.Q<Toggle>();
        var field = element.Q<ObjectField>();

        // Bind toggle
        toggle.SetValueWithoutNotify(data.enabled);
        toggle.RegisterValueChangedCallback(evt =>
        {
            data.enabled = evt.newValue;
            CommitToBuildSettings();
        });

        // Bind scene field
        var sceneAsset = string.IsNullOrEmpty(data.path) ? null : AssetDatabase.LoadAssetAtPath<SceneAsset>(data.path);
        field.SetValueWithoutNotify(sceneAsset);
        field.RegisterValueChangedCallback(evt =>
        {
            var newScene = evt.newValue as SceneAsset;
            data.path = newScene ? AssetDatabase.GetAssetPath(newScene) : string.Empty;
            // If path is cleared, default to disabled
            if (string.IsNullOrEmpty(data.path))
                data.enabled = false;

            CommitToBuildSettings();
            // Rebind to reflect any changes
            var updatedAsset = string.IsNullOrEmpty(data.path) ? null : AssetDatabase.LoadAssetAtPath<SceneAsset>(data.path);
            field.SetValueWithoutNotify(updatedAsset);
            toggle.SetValueWithoutNotify(data.enabled);
        });
    }

    private void RefreshFromBuildSettings()
    {
        buildScenes = EditorBuildSettings.scenes
            .Select(s => new BuildSceneEntry { path = s.path, enabled = s.enabled })
            .ToList();

        if (sceneListView != null)
        {
            sceneListView.itemsSource = buildScenes;
            sceneListView.Rebuild();
        }
    }

    private void CommitToBuildSettings()
    {
        // Filter out any entries without a valid path
        var validEntries = buildScenes
            .Where(s => !string.IsNullOrEmpty(s.path))
            .Select(s => new EditorBuildSettingsScene(s.path, s.enabled))
            .ToArray();

        EditorBuildSettings.scenes = validEntries;
        // Keep our model in sync with any canonicalization
        RefreshFromBuildSettings();
    }

    private void BuildAndRun()
    {
        // Ensure latest list is committed
        CommitToBuildSettings();

        var enabledScenes = EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .Select(s => s.path)
            .ToArray();

        if (enabledScenes.Length == 0)
        {
            Debug.LogWarning("No enabled scenes in Build Settings.");
            return;
        }

        string buildPathBase = "Builds/OverlayBuild/OverlayGame";
        string buildPath = buildPathBase;

        switch (EditorUserBuildSettings.activeBuildTarget)
        {
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
                buildPath = buildPathBase + ".exe";
                break;
            case BuildTarget.StandaloneOSX:
                buildPath = buildPathBase + ".app";
                break;
            case BuildTarget.StandaloneLinux64:
                buildPath = buildPathBase; // Linux creates a folder with executable inside
                break;
            default:
                buildPath = buildPathBase; // Other platforms manage their own extensions/folders
                break;
        }

        var dir = System.IO.Path.GetDirectoryName(buildPath);
        if (!string.IsNullOrEmpty(dir))
            System.IO.Directory.CreateDirectory(dir);

        var buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = enabledScenes,
            locationPathName = buildPath,
            target = EditorUserBuildSettings.activeBuildTarget,
            options = BuildOptions.AutoRunPlayer
        };

        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }
}
#endif
