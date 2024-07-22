using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class LocationManager : MonoBehaviour
{
    List<double> lastGpsCoords = new List<double>();

    public void Start()
    {
        StartCoroutine(GetGPSLocationFromSensors());
    }

    public List<double> GetGPSCoords()
    {
        StartCoroutine (GetGPSLocationFromSensors());
        return this.lastGpsCoords;
    }


    IEnumerator GetGPSLocationFromSensors()
    {
        // Check if the user has location service enabled.
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            MyConsole.instance.Log("Permissions Location not set");
            Permission.RequestUserPermission(Permission.FineLocation);
            Permission.RequestUserPermission(Permission.CoarseLocation);
        }
        // user has enabled locations service?
        if (!Input.location.isEnabledByUser)
        {
            // popup to turn on location services
            yield return new WaitForSeconds(3);
        }

        // Starts the location service.
        Input.location.Start();

        // Waits until the location service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // If the service didn't initialize in 20 seconds this cancels location service use.
        if (maxWait < 1)
        {
            Debug.Log("Timed out");
            yield break;
        }

        // If the connection failed this cancels location service use.
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogError("Unable to determine device location");
            yield break;
        }
        else
        {
            // If the connection succeeded, this retrieves the device's current location and displays it in the Console window.
            //MyConsole.instance.Log("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
            lastGpsCoords.Clear();
            lastGpsCoords.Add(Input.location.lastData.latitude);
            lastGpsCoords.Add(Input.location.lastData.longitude);
        }

        // Stops the location service if there is no need to query location updates continuously.
        Input.location.Stop();
    }
}
