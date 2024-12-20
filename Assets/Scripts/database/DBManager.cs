using UnityEngine;
using System.Collections.Generic;
using Proyecto26;
using Newtonsoft.Json;
using System.Text;


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
        });
    }

    public RegisteredLocations GetRegisteredLocations()
    {
        return registeredLocations;
    }

    public void StoreNewLocation(Dictionary<string, Mesh> meshDict, List<Anchor> anchorlist, float trueHeading, float lat, float lon)
    {
        SerializableMap newMap = new SerializableMap();
        MyConsole.instance.Log($"DBManager: StoreNewLocation() => meshDict total count {meshDict.Count}");
             
        foreach (KeyValuePair<string, Mesh> entry in meshDict)
        {
            Mesh mesh = entry.Value;
            SerializableMesh serializableMesh = DBConverter.SerializeMesh(mesh);
            newMap.root.location.meshes.Add(serializableMesh);
            newMap.root.location.meshNames.Add(entry.Key);
        }
        newMap.root.location.longitude = lon;
        newMap.root.location.latitude = lat;
        newMap.root.location.trueHeading = trueHeading;
        // add anchors
        foreach (Anchor anchor in anchorlist)
        {
            newMap.root.location.anchorList.Add(DBConverter.SerializeAnchor(anchor));
        }
        string mapJson = Newtonsoft.Json.JsonConvert.SerializeObject(newMap.root.location);


        byte[] byteArray = Encoding.UTF8.GetBytes(mapJson);

        // Calculate the size in bytes
        int sizeInBytes = byteArray.Length;

        // Convert the size to megabytes
        double sizeInMegabytes = sizeInBytes / (1024.0 * 1024.0);

        MyConsole.instance.Log($"DBManager: StoreNewLocation() => Size of the string in megabytes => {sizeInMegabytes} MB");

        if (mapJson != null)
        {
            MyConsole.instance.Log($"DBManager: StoreNewLocation() => send map to firebase");
            RestClient.Put(new RequestHelper
            {
                Uri = "https://invisnav-default-rtdb.europe-west1.firebasedatabase.app/locations/" + MapManager.Instance.currentLocation + ".json?writeSizeLimit=unlimited",
                BodyString = mapJson,
                ProgressCallback = percent => MyConsole.instance.Log($"DBManager: StoreNewLocation() => Upload percent => {(int)(percent*100)} %"),
            }).Then(response =>
            {
                MyConsole.instance.Log($"DBManager: StoreMap() Response => {response.StatusCode.ToString()}");
                // register new location
                SaveRegisteredLocation(MapManager.Instance.currentLocation);
            }).Catch(err => {
                var error = err as RequestException;
                MyConsole.instance.Log($"DBManager: ERROR => {error.Response}");
            });
            //RestClient.Put("https://invisnav-default-rtdb.europe-west1.firebasedatabase.app/locations/" +MapManager.Instance.currentLocation + ".json", mapJson).Then(response => {

            //    MyConsole.instance.Log($"DBManager: StoreMap() Response => {response.StatusCode.ToString()}");
            //    });
        } else
        {
            MyConsole.instance.Log("DBManager: Error => Serialization: mapJson is empty");
            MyConsole.instance.Log("DBManager: Error => Map could not bet saved");
        }
    }

    public void SaveRegisteredLocation( string name)
    {
        registeredLocations.locations.Add(name);
        
        string json = JsonConvert.SerializeObject(registeredLocations.locations);
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
        SerializableLocation loadedMap = new();

        RestClient.Get("https://invisnav-default-rtdb.europe-west1.firebasedatabase.app/locations/" + name + ".json").Then(response =>
        {
            loadedMap = JsonConvert.DeserializeObject<SerializableLocation>(response.Text);
            if (loadedMap != null)
            {
                // Add to Map Dict 
                int indexValue = 0;
                foreach (var mesh in loadedMap.meshes)
                {

                    string meshName = loadedMap.meshNames[indexValue];
                    MapManager.Instance.AddMeshToLoadedMap(DBConverter.DeserializeMesh(mesh), meshName);

                    indexValue++;
                }
                foreach(SerializableAnchor a in loadedMap.anchorList)
                {
                    MyConsole.instance.Log("DBManager: GetLocationByName serializable anchor posx: " + a.posX);

                    MapManager.Instance.AddAnchorToLoadedMap(DBConverter.DeserializeAnchor(a));
                }
            }
            else
            {
                MyConsole.instance.Log("DBManager: ERROR => loaded map is null");
            }
        });
    }
}
