using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainAdjusterRuntime))]
public class TerrainAdjusterRuntimeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TerrainAdjusterRuntime targetGameObject = (TerrainAdjusterRuntime)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Save Original Terrain"))
        {
            targetGameObject.SaveOriginalTerrainHeights();
        }

        if (GUILayout.Button("Shape Terrain"))
        {
            targetGameObject.ShapeTerrain();
        }

        if (GUILayout.Button("Flatten Entire Terrain"))
        {
            SetTerrainHeight(targetGameObject.terrain, 0f);
        }
    }

    void SetTerrainHeight(Terrain terrain, float height)
    {
        TerrainData terrainData = terrain.terrainData;
        int w = terrainData.heightmapResolution;
        int h = terrainData.heightmapResolution;
        float[,] allHeights = new float[w, h];

        for (int x = 0; x < w; x++)
            for (int y = 0; y < h; y++)
                allHeights[y, x] = height;

        terrainData.SetHeights(0, 0, allHeights);
    }
}
