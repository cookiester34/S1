// using UnityEditor;
// using UnityEngine;
//
// public class SphereGenerator : MonoBehaviour
// {
//     [SerializeField] private string savePath = "Assets/GeneratedMeshes/Sphere.asset";
//     [SerializeField] private float radius = 1f;
//     [SerializeField] private int segments = 20;
//
//     [ContextMenu("Generate Sphere Mesh")]
//     private void Generate()
//     {
//         // Generate the sphere mesh
//         Mesh mesh = GenerateSphereMesh(radius, segments);
//
//         // Save the generated mesh as an asset
//         SaveMeshAsset(mesh, savePath);
//     }
//
//     private Mesh GenerateSphereMesh(float radius, int segments)
//     {
//         Mesh mesh = new Mesh();
//
//         // Create lists to hold vertices, normals, and triangles
//         var vertices = new System.Collections.Generic.List<Vector3>();
//         var normals = new System.Collections.Generic.List<Vector3>();
//         var triangles = new System.Collections.Generic.List<int>();
//
//         // Generate vertices and normals for the sphere
//         for (int lat = 0; lat <= segments; lat++)
//         {
//             float normalizedLat = Mathf.PI * ((float)lat / segments);
//             float sinLat = Mathf.Sin(normalizedLat);
//             float cosLat = Mathf.Cos(normalizedLat);
//
//             for (int lon = 0; lon <= segments; lon++)
//             {
//                 float normalizedLon = 2 * Mathf.PI * ((float)lon / segments);
//                 float sinLon = Mathf.Sin(normalizedLon);
//                 float cosLon = Mathf.Cos(normalizedLon);
//
//                 Vector3 vertex = new Vector3(sinLat * cosLon, cosLat, sinLat * sinLon) * radius;
//                 vertices.Add(vertex);
//                 normals.Add(vertex.normalized);
//             }
//         }
//
//         // Generate triangles for the sphere
//         for (int lat = 0; lat < segments; lat++)
//         {
//             for (int lon = 0; lon < segments; lon++)
//             {
//                 int current = lat * (segments + 1) + lon;
//                 int next = current + segments + 1;
//
//                 triangles.Add(current);
//                 triangles.Add(next);
//                 triangles.Add(current + 1);
//
//                 triangles.Add(next);
//                 triangles.Add(next + 1);
//                 triangles.Add(current + 1);
//             }
//         }
//
//         // Assign vertices, normals, and triangles to the mesh
//         mesh.vertices = vertices.ToArray();
//         mesh.normals = normals.ToArray();
//         mesh.triangles = triangles.ToArray();
//
//         return mesh;
//     }
//
//     private void SaveMeshAsset(Mesh mesh, string path)
//     {
//         // Create a new asset instance
//         Mesh meshAsset = new Mesh();
//         meshAsset.vertices = mesh.vertices;
//         meshAsset.normals = mesh.normals;
//         meshAsset.triangles = mesh.triangles;
//
//         // Save the mesh asset to the specified path
//         AssetDatabase.CreateAsset(meshAsset, path);
//         AssetDatabase.SaveAssets();
//         Debug.Log("Sphere mesh saved to: " + path);
//     }
// }
