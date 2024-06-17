using Newtonsoft.Json;
using Proyecto26;
using System.Collections.Generic;
using UnityEngine;

public class Populate : MonoBehaviour
{
    public class RegisteredLocations
    {
        public List<string> locations;
    }

    RegisteredLocations registeredLocations;
    RegisteredLocations newLocations;
    private void Start()
    {
        newLocations = new RegisteredLocations();
        newLocations.locations = new List<string>(); 

        registeredLocations = new RegisteredLocations();
        registeredLocations.locations = new List<string>(){
            "SRH",
            "Cube",
            "Bibliothek"
        };
        PopulateDBWithRegisteredLocations();
        GetRegisteredLocations();
     
    }

    void PopulateDBWithRegisteredLocations()
    {
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(registeredLocations);
        Debug.Log(json);
        RestClient.Put("https://invisnav-default-rtdb.europe-west1.firebasedatabase.app/registeredLocations.json", json);
    }
    public void GetRegisteredLocations()
    {
        RestClient.Get("https://invisnav-default-rtdb.europe-west1.firebasedatabase.app/registeredLocations.json").Then(response =>
        {
            Debug.Log($"response: {response.Text}");
            newLocations = JsonConvert.DeserializeObject<RegisteredLocations>(response.Text); ;
            Debug.Log($"count: {newLocations.locations.Count}");
            foreach (var location in newLocations.locations)
            {
                Debug.Log($"found: {location}");
            }
        });
      
    }


}
