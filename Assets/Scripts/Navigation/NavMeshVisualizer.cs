using UnityEngine;
using UnityEngine.AI;

public class NavMeshVisualizer : MonoBehaviour
{
    public bool ShowNavMesh = true;
    [SerializeField]
    private Material _material;
    [SerializeField]
    private Vector3 GeneratedMeshOffset = new Vector3(0, 0.05f, 9);
    private GameObject NavMeshVisualizationGo;

    private MeshRenderer NavMeshRenderer;
    private MeshFilter NavMeshFilter;
    private MeshCollider NavMeshCollider;
    void Start()
    {
        NavMeshVisualizationGo = new ("NavMeshVisualization");
        NavMeshRenderer = NavMeshVisualizationGo.AddComponent<MeshRenderer>();
        NavMeshFilter = NavMeshVisualizationGo.AddComponent<MeshFilter>();
        NavMeshCollider = NavMeshVisualizationGo.AddComponent<MeshCollider>();
        NavMeshVisualizationGo.layer = 10;
    }

    void Update()
    {
        if(ShowNavMesh)
        {
            NavMeshVisualizationGo.SetActive(ShowNavMesh);
            CalculateNavMesh();
            //NavMeshVisualizationGo.transform.position = GeneratedMeshOffset;
        }
    }

    private void CalculateNavMesh()
    {
        Mesh navMesh = new Mesh();
        NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();

        navMesh.SetVertices(triangulation.vertices);
        navMesh.SetIndices(triangulation.indices, MeshTopology.Triangles, 0);

        NavMeshRenderer.sharedMaterial = _material;
        NavMeshFilter.mesh = navMesh;
        NavMeshCollider.sharedMesh = navMesh;
    }
}
