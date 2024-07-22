using UnityEngine;
using System.Collections.Generic;
using System;
using Proyecto26;
using Newtonsoft.Json;

public class RegisteredLocations
{
    public List<string> locations;
}


[System.Serializable]
public class Locations
{
    public Dictionary<string, Location> locations;
}

public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;

    // Constructor
    public MeshData()
    {
        vertices = new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector3(0, 0, 1) };
        triangles = new int[] { 1, 2, 3 };
    }
}
public class Location
{
    public string locationName;
    public MeshData meshData;

}

public class DBManager : MonoBehaviour
{
    public bool fireBaseIsReady = false;
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
        registeredLocations = new RegisteredLocations();

        Locations locations = new Locations();
        locations.locations = new Dictionary<string, Location>();
        GetRegisteredLocationsFromDB();
        //Location location1 = new Location()
        //{
        //    locationName = "location1",
        //    meshData = new MeshData()
        //};
        //Location location2 = new Location()
        //{
        //    locationName = "location2",
        //    meshData = new MeshData()
        //};

        //locations.locations.Add(location1.locationName, location1);
        //locations.locations.Add(location2.locationName, location2);



        //foreach (KeyValuePair<string, Location> l in locations.locations)
        //{
        //    Debug.Log($"l: {l.Value.GetType()}");
        //    Location location = l.Value;
        //    Debug.Log($"working on: {l.Value.locationName}");
        //    RestClient.Put("https://invisnav-default-rtdb.europe-west1.firebasedatabase.app/maps/" + l.Value.locationName + ".json", l.Value);
        //    Debug.Log("added");
        //}
        //RestClient.Get<Dictionary<string, Location>>("https://invisnav-default-rtdb.europe-west1.firebasedatabase.app/maps.json").Then(response =>
        //{
        //    Debug.Log(response.Count);

        //});
    }
    
    private void GetRegisteredLocationsFromDB()
    {
        RestClient.Get("https://invisnav-default-rtdb.europe-west1.firebasedatabase.app/registeredLocations.json").Then(response =>
        {
            Debug.Log($"response: {response.Text}");
            registeredLocations = JsonConvert.DeserializeObject<RegisteredLocations>(response.Text); ;
            Debug.Log($"count: {registeredLocations.locations.Count}");
            foreach (var location in registeredLocations.locations)
            {
                Debug.Log($"found: {location}");
            }
        });

    }

    public RegisteredLocations GetRegisteredLocations()
    {
        return registeredLocations;
    }
}
