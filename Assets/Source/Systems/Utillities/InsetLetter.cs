using UnityEngine;




public class InsetLetter : MonoBehaviour
{
    Mesh mesh;

    private void Start()
    {
        mesh = GetComponent<MeshFilter>().sharedMesh;
        InvertNormals(mesh);

    }

    public static void InvertNormals(Mesh mesh)
    {
        var normals = mesh.normals;
        for (int i = 0; i < normals.Length; i++)
            normals[i] = -normals[i];
        mesh.normals = normals;

        // Flip triangle winding
        for (int sub = 0; sub < mesh.subMeshCount; sub++)
        {
            var tris = mesh.GetTriangles(sub);
            for (int i = 0; i < tris.Length; i += 3)
            {
                int temp = tris[i];
                tris[i] = tris[i + 1];
                tris[i + 1] = temp;
            }
            mesh.SetTriangles(tris, sub);
        }
    }

}
