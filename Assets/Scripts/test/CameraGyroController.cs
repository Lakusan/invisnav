using UnityEngine;

public class CameraGyroController : MonoBehaviour
{
    float deviceLat;
    float deviceLon;

    float startLat;
    float startLon;
    bool done = false;
    [SerializeField] GameObject prefab;

    private void Start()
    {
        //startLat = 49.22017868580164f;
        //startLon = 8.656280786736996f;
        startLat = 49.22058862762731f;
        startLon = 8.655009419701486f;
    }


    private void Update()
    {
        if (LocationManager.Instance.isLocationServicesRunning)
        {
            deviceLat = Input.location.lastData.latitude;
            deviceLon = Input.location.lastData.longitude;
            Debug.Log(deviceLat + " " + deviceLon);
            if (!done && deviceLat != 0.0f)
            {
                 GetDevicePosition();
            }
        }
    }


    void GetDevicePosition()
    {
        float distanceToStartPoint = LocationManager.Instance.CalculateVincentyDistance(deviceLat, deviceLon, startLat, startLon);
        Quaternion bearing = LocationManager.Instance.CalculateBearing(deviceLat, deviceLon, startLat, startLon);
        Vector3 startPos = bearing * (Vector3.forward * (int)distanceToStartPoint);
        Instantiate(prefab, startPos, Quaternion.identity);
        done = true;
        Debug.Log($"distance: {distanceToStartPoint}");
    }


}
