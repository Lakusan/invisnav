using UnityEngine;

public class MapComponentControler : MonoBehaviour
{
    [SerializeField] GameObject myPrefab;
    MeshFilter meshFilter;

    Vector3[] vertices;
    Vector3[] worldVertices;
    private bool isDone = false;

    void Start()
    { 
        //meshFilter = GetComponent<MeshFilter>();
        //vertices = meshFilter.mesh.vertices;
        //Vector3 centerVector = meshFilter.mesh.bounds.center;
        //centerVector = transform.TransformPoint(centerVector);
        //centerVector = new Vector3(centerVector.x, (centerVector.y + 1), centerVector.z);
        //Ray rayCenter = new Ray(centerVector, Vector3.down);
        //Debug.DrawRay(rayCenter.origin, rayCenter.direction * 10, Color.red);

        //RaycastHit hitCenter;

        //if (!Physics.Raycast(rayCenter, out hitCenter))
        //{

        //    GameObject go = Instantiate(gameObject);

        //} else
        //{
        //    MeshRenderer renderer = hitCenter.collider.gameObject.GetComponent<MeshRenderer>();
        //    if (renderer != null) // Make sure the target has a Renderer component
        //    {
        //        Color randomColor = new Color(Random.value, Random.value, Random.value);
        //        renderer.material.color = Color.red; // Change the color to green
        //    }
        //}
    }

    void Update()
    {
        if (!isDone)
        {
            // checks if there is a map segment, if not add to map

            //foreach (Vector3 v in vertices)
            //{
            //    Vector3 vertexWorldPosition = transform.TransformPoint(v);
            //    vertexWorldPosition.y += 1;
            //    RaycastHit hit;
            //    Ray ray = new Ray(vertexWorldPosition, Vector3.down);
            //    Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);
            //    if (Physics.Raycast(ray, out hit))
            //    {

            //    }
            //    else
            //    {
            //        Instantiate(myPrefab, new Vector3(vertexWorldPosition.x, vertexWorldPosition.y-1, vertexWorldPosition.z), Quaternion.identity);
            //    }
            //}

            // use center vector down and check if hit layer map
            // if hit skip, if not add to Mesh List and render map part

            // render meshes with the job system as game objects
            // load and reload system
            isDone = true;
        }
    }

}
