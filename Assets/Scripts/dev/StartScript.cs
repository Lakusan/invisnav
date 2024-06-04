using UnityEngine;
using UnityEngine.Android;

public class StartScript : MonoBehaviour
{
    public void Start()
    {
        // get permission to use location data if on Android
        // then enable location and compass services
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            var androidPermissionCallbacks = new PermissionCallbacks();
            androidPermissionCallbacks.PermissionGranted += permissionName =>
            {
                if (permissionName == "android.permission.ACCESS_FINE_LOCATION")
                {
                    Start();
                }
            };

            Permission.RequestUserPermission(Permission.FineLocation, androidPermissionCallbacks);
            return;
        }
#endif
        Input.compass.enabled = true;
        Input.location.Start();
    }
}