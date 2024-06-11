using Newtonsoft.Json;
using Proyecto26;
using System.Collections.Generic;
using UnityEngine;


public class MeshRoot
{
    public Meshes meshes { get; set; }
}

public class Meshes
{
    public List<Vertex> vertices { get; set; }
    public List<int> triangles { get; set; }
}


public class Vertex
{
    public int x { get; set; }
    public int y { get; set; }
    public int z { get; set; }
}


public class DBTest : MonoBehaviour
{

    public MeshRoot root = new MeshRoot()
    {
        meshes = new Meshes()
        {
            vertices = new List<Vertex>()
            {

            },
            triangles = new List<int>(),
        },
    };

    private void Start()
    {
    

        foreach (var mesh in root.meshes.vertices)
        {
            Debug.Log(mesh.x);
        }
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(root);
        Debug.Log($"json: {json}");
        RestClient.Put("https://invisnav-default-rtdb.europe-west1.firebasedatabase.app/locations.json", json);


        RestClient.Get("https://invisnav-default-rtdb.europe-west1.firebasedatabase.app/locations.json").Then(response =>
        {

            Debug.Log($" Response: {response.Text}");


            ////Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(json);
            MeshRoot myDeserializedClass = JsonConvert.DeserializeObject<MeshRoot>(response.Text);
            Debug.Log($" Deserialized: {myDeserializedClass.meshes.vertices.Count}");

            //add deserialized vertices to new Vector3[]
            Mesh newMesh = new Mesh();
            Vector3[] newVertices = new Vector3[myDeserializedClass.meshes.vertices.Count];
            Debug.Log($"vertices deserialized: {myDeserializedClass.meshes.vertices.Count}");
            int i = 0;
            foreach (var m in myDeserializedClass.meshes.vertices)
            {

                Debug.Log($"vertex x: {m.x}");
                newVertices[i++] = new Vector3(m.x, m.y, m.z);
            }
            Debug.Log($"new vertices: {newVertices.Length}");

            // new triangles
            int[] newTriangles = new int[myDeserializedClass.meshes.triangles.Count];
            int k = 0;
            foreach (var t in myDeserializedClass.meshes.triangles)
            {
                newTriangles[k] = t;
                k++;
            }
            Debug.Log($"trangles: {newTriangles.Length}");

            newMesh.vertices = newVertices;
            newMesh.triangles = newTriangles;
            newMesh.RecalculateNormals();
            newMesh.RecalculateBounds();
            MeshFilter filter = GameObject.FindObjectOfType<MeshFilter>();
            filter.mesh = newMesh;
        });

    }

    void AddMesh() 
    {

        Vertex vertex1 = new Vertex()
        {
            x = 0,
            y = 0,
            z = 0,
        };
        Vertex vertex2 = new Vertex()
        {
            x = 0,
            y = 0,
            z = 1,
        };
        Vertex vertex3 = new Vertex()
        {
            x = 1,
            y = 0,
            z = 0,
        };


        Meshes mesh = new Meshes()
        {
            vertices = new List<Vertex>(),
            triangles = new List<int>(),
        };

        mesh.vertices.Add(vertex1);
        mesh.vertices.Add(vertex2);
        mesh.vertices.Add(vertex3);


        mesh.triangles.Add(0);
        mesh.triangles.Add(1);
        mesh.triangles.Add(2);
    }

}
