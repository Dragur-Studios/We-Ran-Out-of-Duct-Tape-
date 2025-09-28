using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

[ExecuteInEditMode]
public class TerrainAdjusterRuntime : MonoBehaviour
{
    public Terrain terrain;

    [Range(0f, 1f)]
    public float brushFallOff = 0.3f;

    [Range(1f, 10f)]
    public float brushSpacing = 1f;

    public SplineContainer splineContainer;

    private float[,] originalTerrainHeights;

    public int[] initialPassRadii = { 15, 7, 2 };

    void Start()
    {
        splineContainer = GetComponent<SplineContainer>();

        if (splineContainer == null)
            Debug.LogError("Script must be attached to a GameObject with a SplineContainer");
    }

    public void SaveOriginalTerrainHeights()
    {
        if (terrain == null || splineContainer == null)
            return;

        TerrainData terrainData = terrain.terrainData;
        int w = terrainData.heightmapResolution;
        int h = terrainData.heightmapResolution;

        originalTerrainHeights = terrainData.GetHeights(0, 0, w, h);
    }

    public void CleanUp()
    {
        originalTerrainHeights = null;
    }

    public void ShapeTerrain()
    {
        if (terrain == null || splineContainer == null)
            return;

        if (originalTerrainHeights == null)
            SaveOriginalTerrainHeights();

        Vector3 terrainPosition = terrain.transform.position;
        TerrainData terrainData = terrain.terrainData;

        float terrainMin = terrainPosition.y;
        float terrainMax = terrainPosition.y + terrainData.size.y;
        float totalHeight = terrainMax - terrainMin;

        int w = terrainData.heightmapResolution;
        int h = terrainData.heightmapResolution;

        float[,] allHeights = originalTerrainHeights.Clone() as float[,];

        // Work with the first spline in the container
        var spline = splineContainer.Spline;

        float splineLength = SplineUtility.CalculateLength(
            spline,
            splineContainer.transform.localToWorldMatrix
        );

        for (int pass = 0; pass < initialPassRadii.Length; pass++)
        {
            int radius = initialPassRadii[pass];
            List<Vector3> distancePoints = new List<Vector3>();

            for (float d = 0; d <= splineLength; d += brushSpacing)
            {
                float t = d / splineLength;
                Vector3 point = splineContainer.transform.TransformPoint(
                    SplineUtility.EvaluatePosition(spline, t)
                );
                distancePoints.Add(point);
            }

            // Sort by height descending
            distancePoints.Sort((a, b) => -a.y.CompareTo(b.y));

            foreach (var point in distancePoints)
            {
                float targetHeight = (point.y - terrainPosition.y) / totalHeight;

                int centerX = (int)((point.x - terrainPosition.x) / terrainData.size.x * w);
                int centerY = (int)((point.z - terrainPosition.z) / terrainData.size.z * h);


                AdjustTerrain(allHeights, radius, centerX, centerY, targetHeight);
            }
        }

        terrainData.SetHeights(0, 0, allHeights);
    }

    private void AdjustTerrain(float[,] heightMap, int radius, int centerX, int centerY, float targetHeight)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        for (int offsetY = -radius; offsetY <= radius; offsetY++)
        {
            for (int offsetX = -radius; offsetX <= radius; offsetX++)
            {
                int brushX = centerX + offsetX;
                int brushY = centerY + offsetY;

                if (brushX < 0 || brushY < 0 || brushX >= width || brushY >= height)
                    continue;

                float sqrDst = offsetX * offsetX + offsetY * offsetY;
                if (sqrDst > radius * radius) continue;

                float dst = Mathf.Sqrt(sqrDst);
                float t = dst / radius;
                float brushWeight = Mathf.Exp(-t * t / brushFallOff);

                float deltaHeight = targetHeight - heightMap[brushY, brushX];
                heightMap[brushY, brushX] += deltaHeight * brushWeight;

                if (heightMap[brushY, brushX] > targetHeight)
                    heightMap[brushY, brushX] = targetHeight;
            }
        }
    }
}
