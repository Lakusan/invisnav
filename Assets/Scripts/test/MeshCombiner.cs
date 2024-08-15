using UnityEditor;
using UnityEngine;

public class ComineTest : MonoBehaviour
{
    //[SerializeField] private GameObject mesh1;
    //[SerializeField] private GameObject mesh2;
    //GameObject combinedObject;
    //[SerializeField] private bool findNeigbor;
    //void Start()
    //{
    //    CombineTwoMeshes(mesh1, mesh2);
    //}
    //public float growthRate = 0.1f;
    //public Vector3 growthDirection = Vector3.down;
    //void Update()
    //{
    //   combinedObject.transform.localScale += growthDirection * growthRate * Time.deltaTime;
    //}
    //void CombineTwoMeshes(GameObject mesh1, GameObject mesh2)
    //{
    //    CombineInstance[] combine = new CombineInstance[2];
    //    combine[0].mesh = mesh1.GetComponent<MeshFilter>().sharedMesh;
    //    combine[0].transform = mesh1.transform.localToWorldMatrix;
    //    combine[1].mesh = mesh2.GetComponent<MeshFilter>().sharedMesh;
    //    combine[1].transform = mesh2.transform.localToWorldMatrix;

    //    Mesh combinedMesh = new Mesh();
    //    combinedMesh.CombineMeshes(combine);

    //    Mesh optimizedMesh = OptimizeMesh(combinedMesh);
    //    combinedObject = new GameObject("CombinedCube");
    //    combinedObject.AddComponent<MeshFilter>().sharedMesh = new Mesh();
    //    combinedObject.GetComponent<MeshFilter>().sharedMesh = optimizedMesh;
    //    combinedObject.AddComponent<MeshRenderer>().sharedMaterial = mesh1.GetComponent<MeshRenderer>().sharedMaterial;
    //    // Access the current center of the bounding box
    //    Vector3 currentCenter = optimizedMesh.bounds.center;

    //    // Calculate the offset required to move the center to Vector3.zero
    //    Vector3 offset = Vector3.zero - currentCenter;

    //    // Apply the offset to the combined mesh's transform position
    //    combinedObject.transform.position += offset;
    //}

    //Mesh OptimizeMesh(Mesh combinedMesh)
    //{
    //    //MeshUtility.Optimize(combinedMesh); // Adjust decimation factor

    //    //// Recalculate UVs
    //    //combinedMesh.RecalculateBounds();
        
    //    //combinedMesh.RecalculateNormals();
    //    //return combinedMesh;
    //}


}
