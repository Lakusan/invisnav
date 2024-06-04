using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class FrameAnalyser : MonoBehaviour
{
    ARTrackedImageManager imageManager;
    ARCameraManager cameraManager;

    void Start()
    {
       imageManager = GetComponent<ARTrackedImageManager>();
       cameraManager = Camera.main.GetComponent<ARCameraManager>();
    }

    private void OnEnable()
    {
        Camera.main.GetComponent<ARCameraManager>().frameReceived += OnFrameReceived;
    }

    private void OnDisable()
    {
        Camera.main.GetComponent<ARCameraManager>().frameReceived -= OnFrameReceived;
    }

    void Update()
    {
    }

    private void OnFrameReceived(ARCameraFrameEventArgs args)
    {
        Debug.Log("Image received");
        Debug.Log(args.timestampNs);
        if (args.lightEstimation.averageBrightness.HasValue)
        {
            Debug.Log("good image");
        }   
    }
}
