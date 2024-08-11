using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class LocationManager : MonoBehaviour
{
    public List<float> lastGpsCoords = new List<float>();
    private const float EarthRadius = 6371e3f;
    public static LocationManager Instance { get; private set; }

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
    public void Start()
    {
        StartCoroutine(GetGPSLocationFromSensors());
    }

    public List<float> GetGPSCoords()
    {
        StartCoroutine (GetGPSLocationFromSensors());
        return this.lastGpsCoords;
    }

    public IEnumerator GetGPSLocationFromSensors()
    {
        // Check if the user has location service enabled.
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            //MyConsole.instance.Log("Permissions Location not set");
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

    // Calculate distance using Vincenty's formula
    public float CalculateVincentyDistance(float lat1, float lon1, float lat2, float lon2)
    {
        lat1 = Mathf.Deg2Rad * lat1;
        lon1 = Mathf.Deg2Rad * lon1;
        lat2 = Mathf.Deg2Rad * lat2;
        lon2 = Mathf.Deg2Rad * lon2;

        float dLon = lon2 - lon1;

        float numerator = Mathf.Pow(Mathf.Cos((float)lat2) * Mathf.Sin((float)dLon), 2) +
                           Mathf.Pow(Mathf.Cos((float)lat1) * Mathf.Sin((float)lat2) -
                                     Mathf.Sin((float)lat1) * Mathf.Cos((float)lat2) * Mathf.Cos((float)dLon), 2);
        float denominator = Mathf.Sin((float)lat1) * Mathf.Sin((float)lat2) +
                             Mathf.Cos((float)lat1) * Mathf.Cos((float)lat2) * Mathf.Cos((float)dLon);

        float deltaSigma = Mathf.Atan2(Mathf.Sqrt((float)numerator), (float)denominator);

        float distance = EarthRadius * deltaSigma;

        return distance;
    }
    // frist lat lng pair is reference point
    // second destination
    // bearin is radius in which destination from perspective of reference lies
    public Quaternion CalculateBearing(float lat1, float lon1, float lat2, float lon2)
    {
        // Convert degrees to radians
        double lat1Rad = Mathf.Deg2Rad * (float)lat1;
        double lon1Rad = Mathf.Deg2Rad * (float)lon1;
        double lat2Rad = Mathf.Deg2Rad * (float)lat2;
        double lon2Rad = Mathf.Deg2Rad * (float)lon2;

        double dLon = lon2Rad - lon1Rad;

        double y = Math.Sin(dLon) * Math.Cos(lat2Rad);
        double x = Math.Cos(lat1Rad) * Math.Sin(lat2Rad) - Math.Sin(lat1Rad) * Math.Cos(lat2Rad) * Math.Cos(dLon);

        double bearingRad = Math.Atan2(y, x);

        // Convert radians to degrees and normalize to 0 - 360
        float bearingDeg = (float)(Mathf.Rad2Deg * bearingRad);
        if (bearingDeg < 0)
        {
            bearingDeg += 360;
        }
        // Convert bearing to quaternion
        Quaternion rotation = Quaternion.Euler(0, bearingDeg, 0);
        return rotation;
    }
    // true north vector from gps coordinate frame
    public  Vector3 GetNorthVector(Transform referenceObject)
    {
        return referenceObject.forward;
    }

    public void DrawBearingAndNorth(Quaternion bearingRotation, Transform referenceObject, GameObject target)
    {
        // Calculate bearing direction
        Vector3 bearingDirection = bearingRotation * Vector3.forward;
        // Calculate north direction
        Vector3 northDirection = GetNorthVector(referenceObject);
        // Draw bearing as red ray
        // Debug.DrawRay(referenceObject.transform.position, bearingDirection * (int), Color.red);
        // Draw north as blue ray
        // Debug.DrawRay(referenceObject.transform.position, northDirection * 10, Color.blue);
    }
    // get unity Position from gps coords
    public Vector3 GetUnityXYZFromGPS(float lat, float lon)
    {
        GetGPSLocationFromSensors();
        // get distance in meters from origin to object
        double distance = CalculateVincentyDistance(lastGpsCoords[0], lastGpsCoords[1], lat, lon);
        // get direction vector from origon to obejct
        Quaternion bearing = CalculateBearing(lastGpsCoords[0], lastGpsCoords[1], lat, lon);
        Vector3 bearingVector = bearing * (Vector3.forward * (int)distance);
        // create Vector3 for object placement
        return bearingVector;
    }
}
