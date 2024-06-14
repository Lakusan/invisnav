using UnityEngine;
using System.Collections.Generic;
using System;
using Proyecto26;

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
    public static DBManager Instance { get; private set; }

    //public List<MapData> mapDataList;

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
        Locations locations = new Locations();
        locations.locations = new Dictionary<string, Location>();

        Location location1 = new Location()
        {
            locationName = "location1",
            meshData = new MeshData()
        };
        Location location2 = new Location()
        {
            locationName = "location2",
            meshData = new MeshData()
        };

        locations.locations.Add(location1.locationName, location1);
        locations.locations.Add(location2.locationName, location2);

        foreach (KeyValuePair<string, Location> l in locations.locations)
        {
            Debug.Log($"l: {l.Value.GetType()}");
            Location location = l.Value;
            Debug.Log($"working on: {l.Value.locationName}");
            RestClient.Put("https://invisnav-default-rtdb.europe-west1.firebasedatabase.app/maps/" + l.Value.locationName + ".json", l.Value);
            Debug.Log("added");
        }
        RestClient.Get<Dictionary<string, Location>>("https://invisnav-default-rtdb.europe-west1.firebasedatabase.app/maps.json").Then(response =>
        {
            Debug.Log(response.Count);
            
        });
    }

    void GetMapFromDB()
    {
        RestClient.Get("https://invisnav-default-rtdb.europe-west1.firebasedatabase.app/maps.json").Then(response =>
        {
            Debug.Log(response.Text);
            
        });

    }


    //private bool CheckIfNameExists(string name)
    //{
    //    // check if locationName exists in DB Data
    //    foreach (var item in mapDataList)
    //    {
    //        if (item.locationName == name)
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}

    //public bool SaveNewMapData(MapData mD)
    //{
    //    // generates new entry in Maps.json 
    //    try
    //    {

    //        RestClient.Put("https://invisnav-default-rtdb.europe-west1.firebasedatabase.app/maps/" + mD.locationName + ".json", mD);
    //        return true;
    //    }
    //    catch (Exception e)
    //    {
    //        Debug.Log("Error saving map: " + e.Message);
    //        return false;
    //    }
    //}

    //public bool ChangeExistingMapData(MapData oldMapData, MapData newMapData)
    //{
    //    // generates new entry in Maps.json with unique 
    //    try
    //    {
    //        // find Existing Map
    //        RestClient.Get("https://invisnav-default-rtdb.europe-west1.firebasedatabase.app/map.json");

    //        RestClient.Patch("https://invisnav-default-rtdb.europe-west1.firebasedatabase.app/maps/", newMapData);
    //        // https://invisnav-default-rtdb.europe-west1.firebasedatabase.app/maps/-NzlfznKIfAq5JJi-xHi
    //        return true;
    //    }
    //    catch (Exception e)
    //    {
    //        Debug.Log("Error saving map: " + e.Message);
    //        return false;
    //    }
    //}

    public string ConvertObjToJSONString<T>(T obj)
    {
        try
        {
            return JsonUtility.ToJson(obj);
        }
        catch (Exception ex)
        {
            Debug.Log("Error Converting obj to JSON string: " + ex.Message);
            return default;
        }
    }

    public T? ConvertJSONStringToObj<T>(string jsonString)
    {
        try
        {
            T? obj = JsonUtility.FromJson<T>(jsonString);
            return obj;
        }
        catch (Exception ex)
        {
            Debug.Log("Error reading JSON string: " + ex.Message);
            return default;
        }
    }

    //public List<string> GetRegisteredLocations()
    //{

    //}
}
