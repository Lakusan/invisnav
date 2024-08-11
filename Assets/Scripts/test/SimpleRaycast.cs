using UnityEngine;

public class SimpleRaycast : MonoBehaviour
{

    MeshFilter m_MeshFilter;
    Vector3[] m_vertices;
    void Start()
    {
        m_MeshFilter = GetComponent<MeshFilter>();
        m_vertices = new Vector3[m_MeshFilter.mesh.vertices.Length];
        m_vertices = m_MeshFilter.mesh.vertices;
    }

    void Update()
    {
        foreach (Vector3 v in m_vertices)
        {
            Vector3 worldVertex = transform.TransformPoint(v); // Convert to world space
            RaycastHit hitVertex;
            Ray rayVertex = new Ray(worldVertex, Vector3.down); // Create a ray pointing down
            Debug.DrawRay(rayVertex.origin, rayVertex.direction * 10, Color.green);
            if (Physics.Raycast(rayVertex, out hitVertex))
            {
                Renderer renderer = hitVertex.collider.gameObject.GetComponent<Renderer>();
                if (renderer != null) // Make sure the target has a Renderer component
                {
                    Color randomColor = new Color(Random.value, Random.value, Random.value);
                    renderer.material.color = Color.red; // Change the color to green
                }
            }

        }


        Ray ray = new Ray(transform.position, Vector3.down);
        Debug.DrawRay(ray.origin, ray.direction * 10, Color.red);
        
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // If the ray hit a GameObject, log its name
            //Debug.Log("Hit GameObject: " + hit.collider.gameObject.name);
            Renderer renderer = hit.collider.gameObject.GetComponent<Renderer>();
            if (renderer != null) // Make sure the target has a Renderer component
            {
                Color randomColor = new Color(Random.value, Random.value, Random.value);
                renderer.material.color = randomColor; // Change the color to green
            }
        }
    }
}
