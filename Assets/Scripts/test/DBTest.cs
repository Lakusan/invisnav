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
        //*** get map in DB and Back -> Works

        //Mesh testMesh = GetUnityMesh();
        //Debug.Log($" test Mesh: {testMesh.vertices.Length}");
        //SerializableMesh b = DBConverter.SerializeMesh(testMesh);
        //Debug.Log($"serlializable Mesh: {b.vertices.Count}");
        //string json = Newtonsoft.Json.JsonConvert.SerializeObject(b);
        //Debug.Log($"json: {json}");
        //RestClient.Put("https://invisnav-default-rtdb.europe-west1.firebasedatabase.app/test.json", json);

        //RestClient.Get("https://invisnav-default-rtdb.europe-west1.firebasedatabase.app/locations.json").Then(response =>
        //{
        //    SerializableMesh jsonMesh = JsonConvert.DeserializeObject<SerializableMesh>(json);
        //    Mesh newMesh = DBConverter.DeserializeMesh(jsonMesh);
        //    Debug.Log($"DB Mesh: {newMesh.vertices.Length}");
        //    GameObject go = new GameObject();
        //    go.AddComponent<MeshFilter>();
        //    go.AddComponent<MeshRenderer>();
        //    MeshFilter mf = go.GetComponent<MeshFilter>();
        //    newMesh.RecalculateBounds();
        //    newMesh.RecalculateNormals();
        //    mf.mesh = newMesh;
        //    mf.sharedMesh = newMesh;
        //    MeshRenderer mr = go.GetComponent<MeshRenderer>();
        //    mr.material.SetColor("_Color", Color.blue);
        //});

        //*** get map in DB and Back -> Works


        // *** get location to DB ---> Works

        // create map and put stuff


        //SerializableMap newMap = new SerializableMap();
        //Dictionary<string, Mesh> tmpMap = MapManager.meshDict;

        //Debug.Log($"Mesh Dict: {MapManager.meshDict.Count}");

        //foreach (KeyValuePair<string, Mesh> entry in tmpMap)
        //{
        //    Debug.Log($"work on: {entry.Key}");
        //    Mesh mesh = entry.Value;
        //    Debug.Log($"Vertices: {mesh.vertices.Count()}");
        //    Debug.Log($"entry is type of: {mesh.GetType()}");
        //    SerializableMesh serializableMesh = DBConverter.SerializeMesh(mesh);
        //    newMap.root.location.meshes.Add(serializableMesh);
        //    Debug.Log($"newMap meshes count {newMap.root.location.meshes.Count}");
        //}
        //newMap.root.location.name = "Testlocation";
        //newMap.root.location.longitude = 0.0;
        //newMap.root.location.latitude = 0.0;

        //string mapJson= Newtonsoft.Json.JsonConvert.SerializeObject(newMap.root.location);
        //if (mapJson != null)
        //{
        //    Debug.Log($"newmap json {mapJson}");
        //    //RestClient.Put("https://invisnav-default-rtdb.europe-west1.firebasedatabase.app/TESTlocations.json", mapJson);

        //}
        // *** get location to DB ---> Works
        SerializableLocation loadedMap = new();
        /// *** Restore Map
        RestClient.Get("https://invisnav-default-rtdb.europe-west1.firebasedatabase.app/TESTlocations.json").Then(response =>
        {
            Debug.Log($"response: {response.Text}");
            loadedMap = JsonConvert.DeserializeObject<SerializableLocation>(response.Text);
            if (loadedMap != null)
            {
                Debug.Log($"Meshes count: {loadedMap.meshes.Count}");
            }
            // convert Serialized Mesh to Unity MeshList
            List<Mesh> meshes = new List<Mesh>();
            foreach (var mesh in loadedMap.meshes)
            {
                meshes.Add(DBConverter.DeserializeMesh(mesh));
            }
            Debug.Log($"Deserialized Meshes count: {meshes.Count}");

            // create gos for testing
            int iterator = 0;
            foreach (Mesh newMesh in meshes)
            {
                Debug.Log($"iterator {iterator}");
                GameObject go = new GameObject();
                go.AddComponent<MeshFilter>();
                go.AddComponent<MeshRenderer>();
                MeshFilter mf = go.GetComponent<MeshFilter>();
                newMesh.RecalculateBounds();
                newMesh.RecalculateNormals();
                mf.mesh = newMesh;
                mf.sharedMesh = newMesh;
                iterator++;
            }
        });
        // *** get location to DB ---> Works

        //SerializeMesh(mesh1);
        //SerializeMesh( mesh1);
        //root.meshes = AddMesh();
        //root.meshes = AddMesh();
        ////foreach (var mesh in root.meshes.vertices)
        ////{
        ////    Debug.Log(mesh.x);
        ////}
        //string json = v
        //Debug.Log($"json: {json}");
        //if (json != null)
        //{
        //    RestClient.Put("https://invisnav-default-rtdb.europe-west1.firebasedatabase.app/locations.json", json);
        //}


        //RestClient.Get("https://invisnav-default-rtdb.europe-west1.firebasedatabase.app/locations.json").Then(response =>
        //{

        //    Debug.Log($" Response: {response.Text}");


        //    ////Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(json);
        //    MeshRoot myDeserializedClass = JsonConvert.DeserializeObject<MeshRoot>(response.Text);
        //    Debug.Log($" Deserialized: {myDeserializedClass.meshes.vertices.Count}");

        //    //add deserialized vertices to new Vector3[]
        //    Mesh newMesh = new Mesh();
        //    Vector3[] newVertices = new Vector3[myDeserializedClass.meshes.vertices.Count];
        //    Debug.Log($"vertices deserialized: {myDeserializedClass.meshes.vertices.Count}");
        //    int i = 0;
        //    foreach (var m in myDeserializedClass.meshes.vertices)
        //    {

        //        Debug.Log($"vertex x: {m.x}");
        //        newVertices[i++] = new Vector3(m.x, m.y, m.z);
        //    }
        //    Debug.Log($"new vertices: {newVertices.Length}");

        //    // new triangles
        //    int[] newTriangles = new int[myDeserializedClass.meshes.triangles.Count];
        //    int k = 0;
        //    foreach (var t in myDeserializedClass.meshes.triangles)
        //    {
        //        newTriangles[k] = t;
        //        k++;
        //    }
        //    Debug.Log($"trangles: {newTriangles.Length}");

        //    newMesh.vertices = newVertices;
        //    newMesh.triangles = newTriangles;
        //    newMesh.RecalculateNormals();
        //    newMesh.RecalculateBounds();
        //    MeshFilter filter = GameObject.FindObjectOfType<MeshFilter>();
        //    filter.mesh = newMesh;
        //});

    }


    Mesh GetUnityMesh()
    {
        Mesh mesh = new Mesh();
        Vector3[] v = new Vector3[3];
        int[] t = new int[3];
        Vector3 v1 = new Vector3()
        {
            x = 0,
            y = 0,
            z = 0,
        };
        Vector3 v2 = new Vector3()
        {
            x = 0,
            y = 0,
            z = 1,
        };
        Vector3 v3 = new Vector3()
        {
            x = 1,
            y = 0,
            z = 0,
        };
        v[0] = v1;
        v[1] = v2;
        v[2] = v3;
        t [0] = 0;
        t [1] = 1;
        t [2] = 2;
        mesh.vertices = v;
        mesh.triangles = t;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        return mesh;
    }


    Meshes AddMesh() 
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
        return mesh;
    }

    void AddLocation()
    {
        // dummy data

        //SerializationMap serializationMap = new();
    }

}
