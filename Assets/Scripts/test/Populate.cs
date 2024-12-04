using Newtonsoft.Json;
using Proyecto26;
using System.Collections.Generic;
using UnityEngine;

public class Populate : MonoBehaviour
{
    RegisteredLocations registeredLocations;
    RegisteredLocations newLocations;

    void Start()
    {

        registeredLocations = new RegisteredLocations()
        {
            locations = new List<string>()
                 {
                "TestEnv"
            },
        };

        foreach (var location in registeredLocations.locations)
        {
            Debug.Log($"reg Loc: {location}");
        }
       
        PopulateDBWithRegisteredLocations();
        //GetRegisteredLocations();

    }

    void PopulateDBWithRegisteredLocations()
    {
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(registeredLocations.locations);
        Debug.Log($"Json output: {json}");
        RestClient.Put("https://invisnav-default-rtdb.europe-west1.firebasedatabase.app/registeredLocations/locations.json", json);
    }
    public void GetRegisteredLocations()
    {
        RestClient.Get("https://invisnav-default-rtdb.europe-west1.firebasedatabase.app/registeredLocations.json").Then(response =>
        {
            Debug.Log($"response: {response.Text}");
            newLocations= JsonConvert.DeserializeObject<RegisteredLocations>(response.Text); ;
            Debug.Log($"count: {newLocations.locations.Count}");
            foreach (var location in newLocations.locations)
            {
                Debug.Log($"found: {location}");
            }
        });

    }


}
