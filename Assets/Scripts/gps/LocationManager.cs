using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class LocationManager : MonoBehaviour
{
    public List<float> lastGpsCoords = new List<float>();
    private const float EarthRadius = 6371e3f;
    public Quaternion deviceRotation;

    public static LocationManager Instance { get; private set; }

    public bool isLocationServicesRunning = false;

    public Quaternion realRotation;
    public float realLat;
    public float realLon;
    public float trueHeading;
    public float magneticNorth;


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
    private void Update()
    {
        if (isLocationServicesRunning) {
            GetData();
        }
    }
    public List<float> GetGPSCoords()
    {
        return this.lastGpsCoords;
    }
    public Quaternion GetDeviceRealWorldRotation()
    {
        return this.deviceRotation;
    }

    public IEnumerator GetGPSLocationFromSensors()
    {
        // Check if the user has location service enabled.
        Debug.Log("LocationManager: start gps discovery");
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
            Debug.Log("LocationManager: wait");

            yield return new WaitForSeconds(3);
        }
        // enable Compass and Gyro
        Input.compass.enabled = true;
        Input.gyro.enabled = true;
        // Starts the location service.
        Debug.Log("LocationManager: start");

        Input.location.Start(1, updateDistanceInMeters:1);

        // Waits until the location service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            Debug.Log("LocationManager: location status init - wait");

            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // If the service didn't initialize in 20 seconds this cancels location service use.
        if (maxWait < 1)
        {
            Debug.Log("LocationManager: Timed out");
            yield break;
        }

        // If the connection failed this cancels location service use.
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("LocationManager: Unable to determine device location");
            yield break;
        }
        else
        {
            isLocationServicesRunning = true;
            //// If the connection succeeded, this retrieves the device's current location and displays it in the Console window.
            //Debug.Log("LocationManager: Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude);
            //Debug.Log("LocationManager: "+Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy);
            //Debug.Log("LocationManager: "+ Input.location.lastData.timestamp);
            //// get GPS Cords
            //if (lastGpsCoords.Count < 2) 
            //{
            //    lastGpsCoords.Clear();
            //    Debug.Log("LocationManager: input lat: " + Input.location.lastData.latitude + ", lon " + Input.location.lastData.longitude);
            //    lastGpsCoords.Add(Input.location.lastData.latitude);
            //    lastGpsCoords.Add(Input.location.lastData.longitude);
            //}

            //lastGpsCoords[0] = Input.location.lastData.latitude;
            //lastGpsCoords[1] = Input.location.lastData.longitude;
            //deviceRotation = Quaternion.Euler(Input.compass.rawVector.x, Input.compass.rawVector.y, Input.compass.rawVector.z);
            //Debug.Log($"Device rotation: {deviceRotation}");
            yield break;
        }
        
        // Stops the location service if there is no need to query location updates continuously.
        //Input.location.Stop();
    }

    // Calculate distance using Vincenty's formula
    // lat1 + lon1 devicepos as refence point -> latlon2 map origin 
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
    // lat1/lon1 -> device as refrence point
    // lat lon2 map origin
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
        //// Draw bearing as red ray
        //Debug.DrawRay(referenceObject.transform.position, bearingDirection * (int)distance, Color.red);
        //// Draw north as blue ray
        //Debug.DrawRay(referenceObject.transform.position, northDirection * 10, Color.blue);
    }
    // get unity Position from gps coords
    public Vector3 GetUnityXYZFromGPS(float lat1, float lon1, float lat2, float lon2 )
    {
        GetGPSLocationFromSensors();
        // get distance in meters from origin to object
        //double distance = CalculateVincentyDistance(lastGpsCoords[0], lastGpsCoords[1], lat, lon);
        double distance = CalculateVincentyDistance(lat1, lon1, lat2, lon2);
        // get direction vector from origon to obejct
        //Quaternion bearing = CalculateBearing(lastGpsCoords[0], lastGpsCoords[1], lat, lon);
        Quaternion bearing = CalculateBearing(lat1, lon1, lat2, lon2);
        Vector3 bearingVector = bearing * (Vector3.forward * (int)distance);
        
        // create Vector3 for object placement
        return bearingVector;
    }
    private Quaternion _rotation;
    private Quaternion _accelerationRotation;
    private Quaternion _gyroRotation;
    private Quaternion _magneticRotation;

    private float _filterCoefficient = 0.1f;
    void GetDeviceRotation()
    {
        // Accelerometer (for initial orientation)
        Vector3 acceleration = Input.acceleration;
        Quaternion accelerationQuaternion = Quaternion.FromToRotation(Vector3.up, -acceleration);
        _accelerationRotation = Quaternion.Slerp(_accelerationRotation, accelerationQuaternion, _filterCoefficient);

        // Gyroscope (for continuous updates)
        _gyroRotation *= Quaternion.Euler(Input.gyro.rotationRate * Time.deltaTime);

        // Magnetometer (for correcting yaw drift)
        Vector3 magneticField = Input.compass.rawVector;

        // Assuming a simple heading calculation (replace with more accurate methods)
        float magneticHeading = Mathf.Atan2(magneticField.x, magneticField.z) * Mathf.Rad2Deg;

        // Create a quaternion representing the magnetic heading
        _magneticRotation = Quaternion.Euler(0, magneticHeading, 0);

        // Combine rotations (implement a suitable fusion algorithm)
        _rotation = Quaternion.Slerp(_rotation, _gyroRotation * _accelerationRotation * _magneticRotation, _filterCoefficient);

        deviceRotation = _rotation;
    }

    public void LogLocationData()
    {
        Debug.Log("Button");
        Debug.Log($"latitude: {lastGpsCoords[0]}");
        Debug.Log($"longitude: {lastGpsCoords[1]}");

            GetDeviceRotation();
        Quaternion rot = deviceRotation;
        Debug.Log($"rot: {rot.ToString()}");
        Debug.DrawRay(Vector3.zero * 10, rot.eulerAngles, Color.yellow);
    }


    public void GetData()
    {
        if(Input.location.status == LocationServiceStatus.Running) {
            float lat = Input.location.lastData.latitude;
            realLat = lat;
            float lon = Input.location.lastData.longitude;
            realLon = lon;
            float altitude = Input.location.lastData.altitude;
            double timestamp = Input.location.lastData.timestamp;
            Quaternion deviceRotation = Quaternion.Euler(Input.compass.rawVector.x, Input.compass.rawVector.y, Input.compass.rawVector.z);
            trueHeading = Input.compass.trueHeading;
            magneticNorth = Input.compass.magneticHeading;
        }
    }

    public void StopLocationServices()
    {
        isLocationServicesRunning = false;
        StopAllCoroutines();
        Input.location.Stop();
    }
}
