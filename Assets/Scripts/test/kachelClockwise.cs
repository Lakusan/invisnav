using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class kachelClockwise : MonoBehaviour
{
    [SerializeField] public GameObject prefab;
    private GameObject mapContainer;
    float impulseTime = 1.0f;
   int iterator = 0;
    List<Vector3> origins;

    UnityEngine.Bounds bounds;
    float rayLength;
    Vector3 center;
    float halfWidth;
    float halfHeight;
    IEnumerator DoSomethingEverySecond()
    {
        while (true)
        {
            switch (iterator)
            {
                case 0:

                    SendRayCast(origins[iterator], transform.forward * rayLength, rayLength, "forward");
                    iterator++;
                    break;
                case 1:
                    SendRayCast(origins[iterator], transform.right * rayLength, rayLength, "right");
                    iterator++;
                    break;
                case 2:
                    SendRayCast(origins[iterator], -transform.right * rayLength, rayLength, "left");
                    iterator++;
                    break;
                case 3:
                    SendRayCast(origins[iterator], -transform.forward * rayLength, rayLength, "backwards");
                    iterator++;
                    break;
                case 4:
                    iterator = 0;
                    Debug.Log($"done iterator: {iterator}");
                    StopAllCoroutines();
                    break;
                default:
                    break;
            }
            yield return new WaitForSeconds(impulseTime);
        }
    }

    void Start()
    {
        mapContainer = GameObject.Find("MapContainer");
        Vector3 forwardCenter = center + transform.forward * halfWidth;
        Vector3 rightCenter = center + transform.right * halfWidth;
        Vector3 leftCenter = center - transform.right * halfWidth;
        Vector3 backwardsCenter = center - transform.forward * halfWidth;

        origins = new List<Vector3>(){
            forwardCenter,
            rightCenter,
            leftCenter,
            backwardsCenter
        };

        bounds = GetComponent<Renderer>().bounds;
        center = bounds.center;
        halfWidth = bounds.extents.x;
        halfHeight = bounds.extents.z;
        rayLength = bounds.size.z;

        StartCoroutine(DoSomethingEverySecond());
    }

    void SendRayCast(Vector3 origin, Vector3 direction, float length, string invokerName)
    {
        if (Physics.Raycast(origin, direction, length))
        {
            Debug.Log($"Hit Something {invokerName}");
        }
        else
        {
            Debug.Log($"REPLICATE in direction: {direction.ToString()} on {invokerName}");
            GameObject newTile = Instantiate(prefab, gameObject.transform.position + direction, Quaternion.identity);
            //GameObject newTile = new ;
            //MeshFilter mf = newTile.AddComponent<MeshFilter>();
            //mf.mesh = gameObject.GetComponent<MeshFilter>().mesh;
            //newTile.transform.position = gameObject.transform.position + direction;
            //newTile.transform.rotation = gameObject.transform.rotation;
            //newTile.transform.position = direction;
            newTile.SetActive(false);
            newTile.transform.parent = mapContainer.transform;
            newTile.name = invokerName;
            newTile.SetActive(true);
        }
    }
}
