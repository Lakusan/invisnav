using UnityEngine;
using UnityEngine.UI;

public class CompassRotation : MonoBehaviour
{

    [SerializeField] private RawImage arrow;
    [SerializeField] private RawImage arrow2;
    [SerializeField] float loadedHeading;
    [SerializeField] float trueHeading;
    public float rayLength = 10f;
    //private Quaternion targetRotation;
    private Quaternion gyroRotation;
    public float heading;
    private Quaternion targetRotation;

    void Start()
    {
        Input.compass.enabled = true;
        Input.gyro.enabled = true;
    }

    void Update()
    {
        Debug.Log($"TESTakjlsd: {Input.compass.trueHeading}");
        arrow.transform.rotation = Quaternion.Euler(0, 0, Input.compass.trueHeading);
        arrow2.transform.rotation = Quaternion.Euler(0, 0, Input.compass.magneticHeading);
    
    }

    void DrawHeading()
    {
        // Calculate magnetic direction based on device orientation
        float angleInRadians = heading * Mathf.Deg2Rad;
        Vector3 magneticDirection = new Vector3(Mathf.Sin(angleInRadians), 0, Mathf.Cos(angleInRadians));

        // Update gyroscope rotation
        Quaternion gyroRotation = Quaternion.identity;
        gyroRotation *= Quaternion.Euler(Input.gyro.rotationRate * Time.deltaTime);

        // Correct magnetic direction based on device orientation (simplified)
        if (Input.deviceOrientation == DeviceOrientation.Portrait)
        {
            magneticDirection = Quaternion.Euler(0, 90, 0) * magneticDirection;
        }

        // Combine magnetic and gyroscope directions
        Vector3 combinedDirection = gyroRotation * magneticDirection;

        // Create a target rotation (consider filtering for smoother results)
        Quaternion targetRotation = Quaternion.LookRotation(combinedDirection, Vector3.up);

        // Smoothly interpolate rotation
        targetRotation= Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f); // Adjust damping factor

        // Calculate and draw debug ray
        Vector3 direction = transform.forward;
        Debug.DrawRay(transform.position, direction * rayLength, Color.blue);

        Vector3 targetEulerAngles = targetRotation.eulerAngles;
        arrow.transform.rotation = Quaternion.Euler(0, 0, targetEulerAngles.y);
    }

    void DrawTargetRoation(float deltaTime)
    {
        float angleInRadians = loadedHeading * Mathf.Deg2Rad;
        Vector3 magneticDirection = new Vector3(Mathf.Sin(angleInRadians), 0, Mathf.Cos(angleInRadians));

        // Update gyroscope rotation
        Quaternion gyroRotation = Quaternion.identity;
        gyroRotation *= Quaternion.Euler(Input.gyro.rotationRate * deltaTime);

        // Correct magnetic direction based on device orientation (simplified)
        if (Input.deviceOrientation == DeviceOrientation.Portrait)
        {
            magneticDirection = Quaternion.Euler(0, 90, 0) * magneticDirection;
        }

        // Combine magnetic and gyroscope directions
        Vector3 combinedDirection = gyroRotation * magneticDirection;

        // Create a target rotation (consider filtering for smoother results)
        Quaternion targetRotation = Quaternion.LookRotation(combinedDirection, Vector3.up);

        // Smoothly interpolate rotation
        targetRotation = Quaternion.Slerp(transform.rotation, targetRotation, deltaTime * 5f); // Adjust damping factor

        // Calculate and draw debug ray
        Vector3 direction = transform.forward;
        Debug.DrawRay(transform.position, direction * rayLength, Color.red);

        Vector3 targetEulerAngles = targetRotation.eulerAngles;
        arrow2.transform.rotation = Quaternion.Euler(0, 0, targetEulerAngles.y);
    }
    private void DrawOpposite()
    {
        float angleInRadians = loadedHeading * Mathf.Deg2Rad;
        arrow2.transform.rotation = Quaternion.Euler(0, 0, loadedHeading);


        //Vector3 magneticDirection = new Vector3(Mathf.Sin(angleInRadians), 0, Mathf.Cos(angleInRadians));
        //magneticDirection = Quaternion.Euler(0, 90, 0) * magneticDirection;
        //Quaternion targetRotation = Quaternion.LookRotation(magneticDirection, Vector3.up);
        //Vector3 direction = transform.forward;
        //Debug.DrawRay(transform.position, direction * rayLength, Color.red);
        //Vector3 targetEulerAngles = targetRotation.eulerAngles;
        //arrow2.transform.rotation = Quaternion.Euler(0, 0, targetEulerAngles.y);


    }

}
//// Get the raw north heading
//float northHeading = Input.compass.trueHeading;

//// Calculate target direction based on device orientation
//float angleInRadians = northHeading * Mathf.Deg2Rad;
//Vector3 magneticDirection = new Vector3(Mathf.Sin(angleInRadians), 0, Mathf.Cos(angleInRadians));

//// Update gyroscope rotation
//gyroRotation *= Quaternion.Euler(Input.gyro.rotationRate * Time.deltaTime);

//// Combine magnetic and gyroscope directions (basic approach)
//Vector3 combinedDirection = gyroRotation * magneticDirection;

//// Create a target rotation (consider filtering for smoother results)
//targetRotation = Quaternion.LookRotation(combinedDirection, Vector3.up);

//// Smoothly interpolate rotation
//transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f); // Adjust damping factor

//// Calculate and draw debug ray
//Vector3 direction = transform.forward;
//Debug.DrawRay(transform.position, direction * rayLength, Color.blue);


//WORKS
// Get the raw north heading
//float northHeading = Input.compass.trueHeading;

//// Calculate magnetic direction based on device orientation
//float angleInRadians = northHeading * Mathf.Deg2Rad;
//Vector3 magneticDirection = new Vector3(Mathf.Sin(angleInRadians), 0, Mathf.Cos(angleInRadians));

//// Update gyroscope rotation
//gyroRotation *= Quaternion.Euler(Input.gyro.rotationRate * Time.deltaTime);

//// Correct magnetic direction based on device orientation (simplified)
//if (Input.deviceOrientation == DeviceOrientation.Portrait)
//{
//    magneticDirection = Quaternion.Euler(0, 90, 0) * magneticDirection;
//}

//// Combine magnetic and gyroscope directions
//Vector3 combinedDirection = gyroRotation * magneticDirection;

//// Create a target rotation (consider filtering for smoother results)
//targetRotation = Quaternion.LookRotation(combinedDirection, Vector3.up);

//// Smoothly interpolate rotation
//transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f); // Adjust damping factor

//// Calculate and draw debug ray
//Vector3 direction = transform.forward;
//Debug.DrawRay(transform.position, direction * rayLength, Color.red);

//Vector3 targetEulerAngles = targetRotation.eulerAngles;
//arrow.transform.rotation = Quaternion.Euler(0, 0, targetEulerAngles.y);