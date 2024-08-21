using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class GPSFakeData : MonoBehaviour
{
    private const float EarthRadius = 6371e3f;

    // SRH 49.413892877933876, 8.651050709960874
    [SerializeField]
    public float originLatitude = 49.413892877933876f;
    [SerializeField]
    public float originLongitude = 8.651050709960874f;
    [SerializeField]
    Quaternion rotation;
    [SerializeField]
    double distance;

    Quaternion rotationToObject;
    double distanceToObject;

    // 49.41302429480974, 8.65435086106081 CUBE
    [SerializeField]
    public float deviceLatitude = 49.41302429480974f;
    [SerializeField]
    public float deviceLongitude = 8.65435086106081f;

    // Distance Luftlinie: 256m

    [SerializeField] GameObject prefab;


    [SerializeField]
    GameObject originGO;
    [SerializeField]
    GameObject deviceGO;

    Quaternion deviceOrientation;

    double distanceDeviceToMapOrigin = 0f;
    Quaternion deviceOrientationToMapOrigin = Quaternion.identity;
    Vector3 distanceVectorDeviceToMapOrigin = new Vector3 (0,0,0);
    void Update()
    {
        //Debug.DrawRay(originGO.transform.position, rotationToObject, Color.red);
        //Vector2 deviceGPSPosition = new Vector2(deviceLatitude, deviceLongitude);
        //Vector2 originGPSPosition = new Vector2(originLatitude, originLongitude);
        // calculate Distance
        Vector3 originPos = LocationManager.Instance.GetUnityXYZFromGPS((float) deviceLatitude, (float) deviceLongitude, (float)originLatitude, (float)originLongitude);
        distance = LocationManager.Instance.CalculateVincentyDistance(deviceLatitude, deviceLongitude, originLatitude, originLongitude);
        rotation = LocationManager.Instance.CalculateBearing(deviceLatitude, deviceLongitude, originLatitude, originLongitude);
        originGO.transform.position = originPos;
    }

    public float CalculateHaversineDistance(Vector2 point1, Vector2 point2)
    {
        float lat1 = Mathf.Deg2Rad * point1.x;
        float lon1 = Mathf.Deg2Rad * point1.y;
        float lat2 = Mathf.Deg2Rad * point2.x;
        float lon2 = Mathf.Deg2Rad * point2.y;

        float dlat = (lat2 - lat1) / 2;
        float dlon = (lon2 - lon1) / 2;

        float a = Mathf.Sin(dlat) * Mathf.Sin(dlat) +
                    Mathf.Cos(lat1) * Mathf.Cos(lat2) * Mathf.Sin(dlon) * Mathf.Sin(dlon);

        float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));
        float distance = EarthRadius * c;

        // Convert to Unity units (1 Unity unit = 1 meter)
        return distance;
    }

    // Calculate distance using Vincenty's formula
    private double CalculateVincentyDistance(double deviceLat, double deviceLon, double origonLat, double originLon)
    {
        double lat1 = Mathf.Deg2Rad * deviceLat;
        double lon1 = Mathf.Deg2Rad * deviceLon;
        double lat2 = Mathf.Deg2Rad * origonLat;
        double lon2 = Mathf.Deg2Rad * originLon;

        double dLon = lon2 - lon1;

        double numerator = Mathf.Pow(Mathf.Cos((float)lat2) * Mathf.Sin((float)dLon), 2) +
                           Mathf.Pow(Mathf.Cos((float)lat1) * Mathf.Sin((float)lat2) -
                                     Mathf.Sin((float)lat1) * Mathf.Cos((float)lat2) * Mathf.Cos((float)dLon), 2);
        double denominator = Mathf.Sin((float)lat1) * Mathf.Sin((float)lat2) +
                             Mathf.Cos((float)lat1) * Mathf.Cos((float)lat2) * Mathf.Cos((float)dLon);

        double deltaSigma = Mathf.Atan2(Mathf.Sqrt((float)numerator), (float)denominator);

        double distance = EarthRadius * deltaSigma;
        this.distance = distance;

        return distance;
    }


    public Quaternion CalculateBearingAnkle(double lat1, double lon1, double lat2, double lon2)
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
        //Debug.DrawRay(originGO.transform.position, GetNorthVectorFromBearing(bearingDeg, originGO.transform), Color.blue);
        Quaternion rotation = Quaternion.Euler(0, bearingDeg, 0);
        return rotation;
    }

    private void BearingToRay(Quaternion bearing)
    {
        // Convert bearing to quaternion
        //Quaternion rotation = Quaternion.Euler(0, bearing, 0);

        // Create direction vector
        Vector3 direction = bearing * Vector3.forward;

        // Draw debug ray
        Debug.DrawRay(transform.position, direction * 10, Color.red); // Adjust length as needed

    }
    public static Vector3 GetNorthVector(Transform referenceObject)
    {
        return referenceObject.forward;
    }

    public void DrawBearingAndNorth(Vector3 startPosition, Quaternion bearingRotation, Transform referenceObject)
    {
        // Calculate bearing direction
        Vector3 bearingDirection = bearingRotation * Vector3.forward;

        // Calculate north direction
        Vector3 northDirection = GetNorthVector(referenceObject);

        // Draw bearing as red ray
        Debug.DrawRay(startPosition, bearingDirection * (int)distanceToObject, Color.red);
        // Draw north as blue ray
        Debug.DrawRay(startPosition, northDirection * 10, Color.blue);
    }
}
