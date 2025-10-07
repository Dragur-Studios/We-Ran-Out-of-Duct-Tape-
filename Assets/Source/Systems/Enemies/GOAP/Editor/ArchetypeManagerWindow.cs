using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class ArchetypeManagerWindow : EditorWindow
{
    private Vector2 scroll;
    private List<GOAPActionSet> archetypes;
    private string searchFilter = "";

    // Track foldout states per archetype
    private Dictionary<GOAPActionSet, bool> foldouts = new();
    // Cache editors so SerializedObjects don’t get disposed
    private Dictionary<GOAPActionSet, Editor> cachedEditors = new();

    [MenuItem("Window/GOAP/Archetype Manager")]
    public static void ShowWindow()
    {
        var window = GetWindow<ArchetypeManagerWindow>("Archetype Manager");
        window.Refresh();
    }

    void OnEnable()
    {
        Refresh();
    }

    void OnDisable()
    {
        // Clean up cached editors
        foreach (var e in cachedEditors.Values)
            DestroyImmediate(e);
        cachedEditors.Clear();
    }

    void Refresh()
    {
        string[] guids = AssetDatabase.FindAssets("t:GOAPActionSet");
        archetypes = new List<GOAPActionSet>();
        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var asset = AssetDatabase.LoadAssetAtPath<GOAPActionSet>(path);
            if (asset != null)
            {
                archetypes.Add(asset);
                if (!foldouts.ContainsKey(asset))
                    foldouts[asset] = false; // default collapsed
            }
        }
    }

    void OnGUI()
    {
        // Toolbar
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(60)))
            Refresh();

        GUILayout.FlexibleSpace();
        searchFilter = GUILayout.TextField(
            searchFilter,
            GUI.skin.FindStyle("ToolbarSearchTextField") ?? GUI.skin.textField,
            GUILayout.Width(200)
        );
        EditorGUILayout.EndHorizontal();

        // Scrollable list
        scroll = EditorGUILayout.BeginScrollView(scroll);

        foreach (var set in archetypes)
        {
            if (!string.IsNullOrEmpty(searchFilter) &&
                !set.name.ToLower().Contains(searchFilter.ToLower()))
                continue;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            // Collapsible header
            EditorGUILayout.BeginHorizontal();
            foldouts[set] = EditorGUILayout.Foldout(foldouts[set], set.name, true);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Select", GUILayout.Width(60)))
                Selection.activeObject = set;
            EditorGUILayout.EndHorizontal();

            // Inline inspector only if expanded
            if (foldouts[set])
            {
                
                if (!cachedEditors.TryGetValue(set, out var editor) || editor == null)
                {
                    editor = Editor.CreateEditor(set);
                    cachedEditors[set] = editor;
                }
                editor.OnInspectorGUI();
                
            }

            EditorGUILayout.EndVertical();
            GUILayout.Space(4);
        }

        EditorGUILayout.EndScrollView();

        // Create button pinned at bottom
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Create New Archetype", GUILayout.Height(30)))
        {
            var newSet = ScriptableObject.CreateInstance<GOAPActionSet>();
            string path = EditorUtility.SaveFilePanelInProject("Save Archetype", "NewArchetype", "asset", "Save new archetype");
            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.CreateAsset(newSet, path);
                AssetDatabase.SaveAssets();
                Refresh();
            }
        }
    }
}
