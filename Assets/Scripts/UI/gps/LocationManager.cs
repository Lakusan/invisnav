using System.Collections;
using UnityEngine;
using UnityEngine.Android;
using Niantic.Lightship.AR.XRSubsystems;
using UnityEngine.XR.ARSubsystems;

public class LocationManager : MonoBehaviour
{
    bool getUpdate = false;
    int iterator = 0;
    private void Update()
    {
        if (getUpdate)
        {
            StartCoroutine(GetGPSLocation());
            getUpdate = !getUpdate;
            iterator++;
            MyConsole.instance.Log("Location Manager Update Called");
        }
    }

    public void SetUpdate(bool update)
    {
        getUpdate = !getUpdate;
        MyConsole.instance.Log("Location Manager SetUpdate Called");
    }

    IEnumerator GetGPSLocation()
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
            MyConsole.instance.Log(iterator.ToString());
            MyConsole.instance.Log("Location service started");
            MyConsole.instance.Log("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
        }

        // Stops the location service if there is no need to query location updates continuously.
        Input.location.Stop();
    }
}
