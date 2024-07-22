using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MapScanner : MonoBehaviour
{
    public bool isMapping;


    private void Start()
    {
        isMapping = true;
    }

    private void Awake()
    {
        isMapping = true;
    }

    LayerMask mask;
    void Update()
    {

        // jetzt mach ich das mit dem Cube und mach collition detection. Wenn nix getroffenb wird mesh , sonst aus


        //var origin = playerPosition + Vector3.ProjectOnPlane(playerForward, Vector3.up).normalized;
        //var origin = Camera.main.transform.position + Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);
        //origin.z = origin.z + 1.5f;
        //gameObject.transform.rotation = Quaternion.LookRotation(origin);
        //gameObject.transform.position = origin;
        //gameObject.transform.rotation = Quaternion.euler(gameObject.transform.rotation.x-Camera.main.transform.rotation.x, 0, 0);
        //foreach (Vector3 v in m_vertices)
        //{
        //    Vector3 worldVertex = transform.TransformPoint(v); // Convert to world space
        //    RaycastHit hitVertex;
        //    Ray rayVertex = new Ray(worldVertex, Vector3.down); // Create a ray pointing down
        //    Debug.DrawRay(rayVertex.origin, rayVertex.direction * 10, Color.green);
        //    if (Physics.Raycast(rayVertex, out hitVertex))
        //    {
        //        Renderer renderer = hitVertex.collider.gameObject.GetComponent<Renderer>();
        //        if (renderer != null) // Make sure the target has a Renderer component
        //        {
        //            Color randomColor = new Color(Random.value, Random.value, Random.value);
        //            renderer.material.color = Color.red; // Change the color to green
        //        }
        //    }

        //}

        //Vector3 centerWorldPostiion = gameObject.transform.position;
        //RaycastHit hit;
        //Ray ray = new Ray(centerWorldPostiion, Vector3.down);
        //Debug.DrawRay(ray.origin, ray.direction * 10, Color.green);
        //if (Physics.Raycast(ray, out hit))
        //{
        //    Debug.Log($"hit: {hit.collider.gameObject.name}");
        //    Renderer renderer = hit.collider.gameObject.GetComponent<Renderer>();
        //    if (renderer != null) // Make sure the target has a Renderer component
        //    {
        //        Color randomColor = new Color(Random.value, Random.value, Random.value);
        //        renderer.material.color = Color.red; // Change the color to green
        //    }
        //} else
        //{
        //    Debug.Log($"no hit");

        //}
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log($"collision with: {other.gameObject.name}");
        
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("left"); 
    }
}

