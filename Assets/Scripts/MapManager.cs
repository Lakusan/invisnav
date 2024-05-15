using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MapManager : MonoBehaviour
{
    [SerializeField] ARMeshManager _arMeshManager;
    [SerializeField] MeshFilter _meshPrefab;
    Mesh mapMesh;


    Vector3[] vertices;
    int[] triangles;
    int xSize = 20;
    int zSize = 20;

    void Start()
    {
        mapMesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mapMesh;
        CreateShape();
        UpdateMesh();
    }

    private void OnEnable()
    {
        _arMeshManager.meshesChanged += map;
    }
    private void OnDisable()
    {
        _arMeshManager.meshesChanged -= map;
    }

    void Update()
    {
        //Debug.Log("From Script: " + _arMeshManager.meshes.Count);
    }

    public void map(ARMeshesChangedEventArgs m)
    {
        //Vector3[] existingVertices = _meshPrefab.mesh.vertices;
        //int[] existingTriangles = _meshPrefab.mesh.triangles;
        //Debug.Log("existing vertices: " + existingVertices.Length);
        //if (m.added != null && m.added.Count > 0)
        //{
        //    Debug.Log("Added Count: " +  m.added.Count);
        //    foreach (MeshFilter mf in  m.added)
        //    {
        //        Debug.Log("Add Mesh Data: " + mf.name);
        //        Vector3[] combinedVertices = existingVertices.Concat(mf.mesh.vertices).ToArray();
        //        int[] combinedTriangles = existingTriangles.Concat(mf.mesh.triangles).ToArray();
        //        _mesh.SetVertices(combinedVertices);
        //        _mesh.triangles = combinedTriangles;
        //        _mesh.RecalculateBounds();
        //        Debug.Log("VERTICES: " + _meshPrefab.mesh.vertices.Count());
        //    }
        //    if (m.updated != null)
        //    {
        //        //Debug.Log("Update existing meshes");
        //    }
        //}
    }

    void CreateShape()
    {
        vertices = new Vector3[(xSize + 1) *(zSize +1)];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                vertices[i] = new Vector3(x, 0, z);
                i++;
            }
        }

        triangles = new int[3];

    }

    void UpdateMesh()
    {
        mapMesh.Clear();

        mapMesh.vertices = vertices;
        mapMesh.triangles = triangles;
        mapMesh.RecalculateBounds();
        mapMesh.RecalculateNormals();
    }

    private void OnDrawGizmos()
    {
        if (vertices == null)
            return;

        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], .1f);
        }
    }
}
