using Newtonsoft.Json;
using Proyecto26;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapAligner : MonoBehaviour
{
    public bool isLocationServicesRunning = false;
    public bool findPoint = false;
    public bool gotMapData = false;
    private Coroutine startLocationServicesCoroutine;

    private float deviceLat;
    private float deviceLon;
    private float deviceTrueHeading;

    private float mapLat;
    private float mapLon;
    private float mapTrueHeading;
    private const float EarthRadius = 6371e3f;
    [SerializeField] public float distanceDeviceMapStart;
    private float bearingDeviceMapStart;


    private RegisteredLocations registeredLocations;
    [SerializeField] private string currentSelectedLocation;

    [SerializeField] public TextMeshProUGUI deviceLatText;
    [SerializeField] public TextMeshProUGUI deviceLonText;
    [SerializeField] public TextMeshProUGUI deviceTrueHeadingText;

    [SerializeField] public TextMeshProUGUI mapLatText;
    [SerializeField] public TextMeshProUGUI mapLonText;
    [SerializeField] public TextMeshProUGUI mapTrueHeadingText;

    [SerializeField] public TextMeshProUGUI distanceToMapStart;
    [SerializeField] public TextMeshProUGUI bearingToMapStartText;

    [SerializeField] public RawImage directionToMapArrowRawImage;
    [SerializeField] public RawImage mapTrueHeadingArrowRawImage;


    [SerializeField] private TMP_Dropdown dropdown;

    void Start()
    {
        dropdown.onValueChanged.AddListener(DropdownValueChanged);
        GetRegisteredLocations();
    } 

    void Update()
    {
        if (LocationManager.Instance.isLocationServicesRunning)
        {
            //update Device data 
            deviceLat = Input.location.lastData.latitude;
            deviceLon = Input.location.lastData.longitude;
            deviceTrueHeading = Input.compass.trueHeading;

            deviceLatText.text = deviceLat.ToString();
            deviceLonText.text = deviceLon.ToString();
            deviceTrueHeadingText.text = deviceTrueHeading.ToString();

        }
        if (gotMapData)
        {
            RotateDirectionDeviceToMapTrueHeading();

            CalculateDistanceDeviceMapStart(deviceLat, deviceLon, mapLat, mapLon);
            RotateBearingDeviceToMapTrueHeading();
        }
    }

    private void PopulateDropdown(List<string> locationsList)
    {
        dropdown.ClearOptions();
        foreach (string option in locationsList)
        {
            TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData();
            optionData.text = option;
            dropdown.options.Add(optionData);
        }
    }

    private void GetRegisteredLocations()
    {
        RestClient.Get("https://invisnav-default-rtdb.europe-west1.firebasedatabase.app/registeredLocations.json").Then(response =>
        {
            registeredLocations = JsonConvert.DeserializeObject<RegisteredLocations>(response.Text); ;
            Debug.Log($"registeredLocations: {registeredLocations.locations.Count}");
            PopulateDropdown(registeredLocations.locations);
        });
    }

    private bool GetSelectedLocationData(string name)
    {
        SerializableLocation loadedMap = new();

        RestClient.Get("https://invisnav-default-rtdb.europe-west1.firebasedatabase.app/locations/" + name + ".json").Then(response =>
        {
            loadedMap = JsonConvert.DeserializeObject<SerializableLocation>(response.Text);
            mapLat = loadedMap.latitude;
            mapLon = loadedMap.longitude;
            mapTrueHeading = loadedMap.trueHeading;
            mapLatText.text = mapLat.ToString();
            mapLonText.text = mapLon.ToString();
            mapTrueHeadingText.text = mapTrueHeading.ToString();
            gotMapData = true;
            return true;
        });
        return false;
    }
    void DropdownValueChanged(int index)
    {
        string selectedOption = dropdown.options[index].text;
        currentSelectedLocation = selectedOption;
    }

    public void OnGetDataButtonPressed()
    {
        GetSelectedLocationData(currentSelectedLocation);
    }

    void RotateDirectionDeviceToMapTrueHeading()
    {
        Quaternion rotation = Quaternion.Euler(0, 0, mapTrueHeading-deviceTrueHeading);
        mapTrueHeadingArrowRawImage.transform.rotation = rotation;

        Debug.DrawRay(mapTrueHeadingArrowRawImage.transform.position, Vector2.up * (mapTrueHeading), Color.green);
    }

    void RotateBearingDeviceToMapTrueHeading()
    {
        bearingToMapStartText.text = bearingDeviceMapStart.ToString();
        directionToMapArrowRawImage.transform.rotation =  Quaternion.Euler(0,0, bearingDeviceMapStart-deviceTrueHeading);
    }

    void CalculateDistanceDeviceMapStart(float latDevice, float lonDevice, float latMap, float lonMap)
    {
        (distanceDeviceMapStart, bearingDeviceMapStart) = CalculateDistanceAndBearing(latDevice, lonDevice, latMap, lonMap);
        distanceToMapStart.text = distanceDeviceMapStart.ToString();
    }
    private Quaternion CalculateBearing(float lat1, float lon1, float lat2, float lon2)
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
        Debug.Log($"bearing: {bearingDeg}");
        // Convert bearing to quaternion
        // Convert bearing to quaternion relative to true north
        Quaternion rotation = Quaternion.Euler(0, 0, bearingDeg);
        return rotation;
    }
    //     Vincenty, T. (1975). Direct solution of geodesics on the ellipsoid.
    //      Survey Review, 23(176), 87-93.
    private float CalculateVincentyDistance(float lat1, float lon1, float lat2, float lon2)
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
    // refined with Copilot

    public static (float, float) CalculateDistanceAndBearing(float lat1, float lon1, float lat2, float lon2)
    {
        // Convert degrees to radians
        double lat1Rad = Mathf.Deg2Rad * lat1;
        double lon1Rad = Mathf.Deg2Rad * lon1;
        double lat2Rad = Mathf.Deg2Rad * lat2;
        double lon2Rad = Mathf.Deg2Rad * lon2;

        // Calculate the difference between longitudes
        double dLon = lon2Rad - lon1Rad;

        // Calculate the average latitude
        double latm = (lat1Rad + lat2Rad) / 2;

        // Calculate the differences in latitude and longitude
        double f = 1 / 298.257223563;  // Flattening of the Earth
        double b = (1 - f) * (1 - f);
        double a = 6378137;  // Semi-major axis of the Earth (meters)

        // Calculate the distance
        double s = a * Mathf.Sqrt(
            Mathf.Pow((float)(Mathf.Cos((float)latm) * Mathf.Sin((float)dLon)), 2) +
            Mathf.Pow((float)(Mathf.Cos((float)lat1Rad) * Mathf.Sin((float)lat2Rad) - Mathf.Sin((float)lat1Rad) * Mathf.Cos((float)lat2Rad) * Mathf.Cos((float)dLon)), 2)
        );

        // Calculate the bearing
        double bearingRad = Mathf.Atan2((float)(Mathf.Sin((float)dLon) * Mathf.Cos((float)lat2Rad)),
                                       (float)(Mathf.Cos((float)lat1Rad) * Mathf.Sin((float)lat2Rad) - Mathf.Sin((float)lat1Rad) * Mathf.Cos((float)lat2Rad) * Mathf.Cos((float)dLon)));
        float bearingDeg = Mathf.Rad2Deg * (float)bearingRad;

        if (bearingDeg < 0)
        {
            bearingDeg += 360;
        }

        return ((float)s, bearingDeg);
    }
}
