using UnityEngine;

namespace Prototyping
{
    public class QuadGenerator : MonoBehaviour
    {
        [SerializeField] private string savePath = "Assets/GeneratedMeshes/Quad.asset";
        [SerializeField] private Vector3 size = new Vector3(1f, 1f, 1f);

        [ContextMenu("Generate Quad Mesh")]
        private void Generate()
        {
            // Generate the quad mesh
            Mesh mesh = GenerateQuadMesh(size);
            // Save the generated mesh as an asset
            SaveMeshAsset(mesh, savePath);
        }

        private Mesh GenerateQuadMesh(Vector3 size)
        {
            Mesh mesh = new Mesh();

            // Define vertices for the quad
            Vector3[] vertices =
            {
                new Vector3(-size.x / 2f, -size.y / 2f, 0f), // Bottom left
                new Vector3(-size.x / 2f, size.y / 2f, 0f),  // Top left
                new Vector3(size.x / 2f, size.y / 2f, 0f),    // Top right
                new Vector3(size.x / 2f, -size.y / 2f, 0f)    // Bottom right
            };

            // Define normals for each vertex (assuming all face the same direction)
            Vector3[] normals =
            {
                Vector3.back,
                Vector3.back,
                Vector3.back,
                Vector3.back
            };

            // Define UVs for the quad
            Vector2[] uvs =
            {
                new Vector2(0f, 0f), // Bottom left
                new Vector2(0f, 1f), // Top left
                new Vector2(1f, 1f), // Top right
                new Vector2(1f, 0f)  // Bottom right
            };

            // Define triangles (two triangles to form the quad)
            int[] triangles = { 0, 1, 2, 0, 2, 3 };

            // Assign vertices, normals, UVs, and triangles to the mesh
            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.uv = uvs;
            mesh.triangles = triangles;

            return mesh;
        }

        private void SaveMeshAsset(Mesh mesh, string path)
        {
            // Create a new asset instance
            Mesh meshAsset = new Mesh();
            meshAsset.vertices = mesh.vertices;
            meshAsset.normals = mesh.normals;
            meshAsset.uv = mesh.uv;
            meshAsset.triangles = mesh.triangles;

            // Save the mesh asset to the specified path
            UnityEditor.AssetDatabase.CreateAsset(meshAsset, path);
            UnityEditor.AssetDatabase.SaveAssets();
            Debug.Log("Quad mesh saved to: " + path);
        }
    }
}
