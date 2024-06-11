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
        meshFilter = GetComponent<MeshFilter>();
        vertices = meshFilter.sharedMesh.vertices;
    }

    void Update()
    {
        if (!isDone)
        {
            foreach (Vector3 v in vertices)
            {
                Vector3 vertexWorldPosition = transform.TransformPoint(v);
                vertexWorldPosition.y += 1;
                RaycastHit hit;
                Ray ray = new Ray(vertexWorldPosition, Vector3.down);
                Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);
                if (Physics.Raycast(ray, out hit))
                {

                }
                else
                {
                    Instantiate(myPrefab, new Vector3(vertexWorldPosition.x, 0, vertexWorldPosition.z), Quaternion.identity);
                }
            }
            Vector3 centerVector = meshFilter.mesh.bounds.center;
            centerVector = transform.TransformPoint(centerVector);
            centerVector = new Vector3(centerVector.x, (centerVector.y +1), centerVector.z);
            Ray rayCenter = new Ray(centerVector, Vector3.down);
            Debug.DrawRay(rayCenter.origin, rayCenter.direction * 10, Color.red);

            RaycastHit hitCenter;

            if (!Physics.Raycast(rayCenter, out hitCenter))
            {
                Instantiate(myPrefab, new Vector3(centerVector.x, 0, centerVector.z), Quaternion.identity);
            }
            isDone = true;
        }
    }

}
