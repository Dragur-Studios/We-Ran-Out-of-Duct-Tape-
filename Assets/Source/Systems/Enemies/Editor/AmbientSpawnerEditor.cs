using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AmbientSpawner))]
public class AmbientSpawnerEditor : Editor
{
    private void OnSceneGUI()
    {
        AmbientSpawner spawner = (AmbientSpawner)target;

        // Sphere volume handle
        if (spawner.volumeType == SpawnVolumeType.Sphere)
        {
            EditorGUI.BeginChangeCheck();
            float newRadius = Handles.RadiusHandle(
                Quaternion.identity,
                spawner.transform.position,
                spawner.spawnRadius
            );
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(spawner, "Change Spawn Radius");
                spawner.spawnRadius = newRadius;
            }
        }

        // Cuboid volume handle
        else if (spawner.volumeType == SpawnVolumeType.Cuboid)
        {
            EditorGUI.BeginChangeCheck();
            Vector3 newExtents = spawner.spawnExtents;

            // Draw a box handle (centered on transform)
            newExtents = Handles.ScaleHandle(
                newExtents,
                spawner.transform.position,
                spawner.transform.rotation,
                HandleUtility.GetHandleSize(spawner.transform.position)
            );

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(spawner, "Change Spawn Extents");
                spawner.spawnExtents = newExtents;
            }
        }
    }
}
