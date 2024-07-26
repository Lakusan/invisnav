using UnityEngine;
using System.Collections.Generic;
using Proyecto26;
using Newtonsoft.Json;


public class DBManager : MonoBehaviour
{
    public RegisteredLocations registeredLocations;
    public static DBManager Instance { get; private set; }

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

    private void Start()
    {
        // find registered Locations
        registeredLocations = new RegisteredLocations();
        GetRegisteredLocationsFromDB();
    }

    private void GetRegisteredLocationsFromDB()
    {
        RestClient.Get("https://invisnav-default-rtdb.europe-west1.firebasedatabase.app/registeredLocations.json").Then(response =>
        {
            registeredLocations = JsonConvert.DeserializeObject<RegisteredLocations>(response.Text);
            Debug.Log($"got registered Locations: {response.Text}");
            Debug.Log($"got registered Locations Count : {registeredLocations.locations.Count}");
        });
    }

    public RegisteredLocations GetRegisteredLocations()
    {
        return registeredLocations;
    }

    public void StoreNewLocation(Dictionary<string, Mesh> meshDict)
    {
        SerializableMap newMap = new SerializableMap();

        foreach (KeyValuePair<string, Mesh> entry in meshDict)
        {
            Mesh mesh = entry.Value;
            SerializableMesh serializableMesh = DBConverter.SerializeMesh(mesh);
            newMap.root.location.meshes.Add(serializableMesh);
        }
#if UNITY_EDITOR
        newMap.root.location.longitude = 0.0;
        newMap.root.location.latitude = 0.0;
#else
        newMap.root.location.longitude = MapManager.Instance.longitude;
        newMap.root.location.latitude = MapManager.Instance.latitude;
#endif
        string mapJson = Newtonsoft.Json.JsonConvert.SerializeObject(newMap.root.location);
        if (mapJson != null)
        {
            RestClient.Put("https://invisnav-default-rtdb.europe-west1.firebasedatabase.app/locations/" + MapManager.Instance.currentLocation + ".json", mapJson);
        }
        // register new location
        SaveRegisteredLocation(MapManager.Instance.currentLocation);
    }

    public void SaveRegisteredLocation( string name)
    {
        registeredLocations.locations.Add(name);
        Debug.Log($"COUNT: {registeredLocations.locations.Count}");
        
        string json = JsonConvert.SerializeObject(registeredLocations.locations);
        Debug.Log($"JSON: {json}");
        RestClient.Put("https://invisnav-default-rtdb.europe-west1.firebasedatabase.app/registeredLocations/locations.json", json);
    }

    public void GetAllLocations()
    {
        RestClient.Get("https://invisnav-default-rtdb.europe-west1.firebasedatabase.app/registeredLocations.json").Then(response =>
        {
            registeredLocations = JsonConvert.DeserializeObject<RegisteredLocations>(response.Text); ;
        });
    }

    public void GetLocationByName(string name) 
    {
        //// UI Select registered Location and deserialize it
        //SerializableMap loadedMap = new SerializableMap();
        //RestClient.Get("https://invisnav-default-rtdb.europe-west1.firebasedatabase.app/locations/" + name +".json").Then(response =>
        //{
        //    Debug.Log($"loaded map Response: {response.Text}");
        //    loadedMap = JsonConvert.DeserializeObject<SerializableMap>(response.Text);
        //    Debug.Log($"loaded map: {loadedMap.root.location.meshes.Count}");
        //});
        SerializableLocation loadedMap = new();

        RestClient.Get("https://invisnav-default-rtdb.europe-west1.firebasedatabase.app/locations/" + name + ".json").Then(response =>
        {
            Debug.Log($"response: {response.Text}");
            loadedMap = JsonConvert.DeserializeObject<SerializableLocation>(response.Text);
            if (loadedMap != null)
            {
                Debug.Log($"Meshes count: {loadedMap.meshes.Count}");
            } else
            {
                Debug.Log("loaded map is null");
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
                go.transform.position = new Vector3(go.transform.position.x, go.transform.position.y + 1.1176f, go.transform.position.z);
                MeshFilter mf = go.GetComponent<MeshFilter>();
                newMesh.RecalculateBounds();
                newMesh.RecalculateNormals();
                mf.mesh = newMesh;
                mf.sharedMesh = newMesh;
                iterator++;
            }
        });
    }
}
