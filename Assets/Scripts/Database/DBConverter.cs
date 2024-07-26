using UnityEngine;

public class DBConverter : MonoBehaviour
{
    public static DBConverter Instance { get; private set; }
   
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public static SerializableMesh SerializeMesh(Mesh mesh)
    {
        SerializableMesh serializableMesh = new SerializableMesh();

        foreach (var vertex in mesh.vertices)
        {
            SerializableVertex serializableVertex = new SerializableVertex()
            {
                x = vertex.x,
                y = vertex.y,
                z = vertex.z
            };

            serializableMesh.vertices.Add(serializableVertex);
        }
        foreach (var triangle in mesh.triangles)
        {
            serializableMesh.triangles.Add(triangle);
        }
        Debug.Log($"Mesh serialized: {serializableMesh.vertices.Count}");
        return serializableMesh;
    }

    public static Mesh DeserializeMesh(SerializableMesh serializableMesh)
    {
        Mesh mesh = new Mesh();
        int numVertices = serializableMesh.vertices.Count;
        int numTriangles = serializableMesh.triangles.Count;

        Vector3[] deserializedVertices = new Vector3[numVertices];
        int[] deserializedTriangles = new int[numTriangles];

        int i = 0;
        foreach (var m in serializableMesh.vertices)
        {
            deserializedVertices[i] = new Vector3(serializableMesh.vertices[i].x, serializableMesh.vertices[i].y, serializableMesh.vertices[i].z);
            i++;
        }
        int k = 0;
        foreach (var t in serializableMesh.triangles)
        {
            deserializedTriangles[k] = t;
            k++;
        }
        mesh.vertices = deserializedVertices;
        mesh.triangles = deserializedTriangles;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }
}
