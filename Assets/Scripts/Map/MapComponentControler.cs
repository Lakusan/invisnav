using System.Collections;
using UnityEngine;

public class MapComponentControler : MonoBehaviour
{
    MeshFilter meshFilter;
    [SerializeField] bool wasSeen = false;  
    [SerializeField] bool isColliding = false;

    void Start()
    { 
        meshFilter = GetComponent<MeshFilter>();

    }
    private void Update()
    {
        //if (wasSeen && !isColliding)
        //{
        //    MapManager.Instance.AddMeshToMap(meshFilter.mesh);
        //    Destroy(gameObject);
        //}
        //StartCoroutine(WaitForXFrames(300));

    }
    //public IEnumerator WaitForXFrames(int frames)
    //{
    //    for (int i = 0; i < frames; i++)
    //    {
    //        yield return null;
    //    }

    //    Debug.Log($"{frames} frames have passed on {gameObject.name}");
    //    isColliding = false;
    //}
    private void OnDestroy()
    {
        //Debug.Log($"{gameObject.name} got destroyed");
        //MapManager.Instance.AddMeshToMap(meshFilter.sharedMesh);
    }
    //private void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log($"Colision enter on {gameObject.name}");
    //    if (collision.gameObject.layer == 9)
    //    {
    //        MeshFilter meshFilter = GetComponent<MeshFilter>();
    //        MapManager.Instance.AddMeshToMap(meshFilter.mesh);
    //        Destroy(gameObject);
    //    }
    //}
    //private void OnCollisionExit(Collision collision)
    //{
    //    Debug.Log($"Collision stays on: {gameObject.name}");
    //    if (collision.gameObject.layer == 9)
    //    {
    //        //MeshFilter meshFilter = GetComponent<MeshFilter>();
    //        //MapManager.Instance.AddMeshToMap(meshFilter.mesh);
    //        //Destroy(gameObject);
    //    }
    //}

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            MapManager.Instance.AddMeshToMap(meshFilter.mesh);
            Destroy(gameObject);
            //isColliding = false;
        }

    }
    private void OnTriggerEnter(Collider other)
    {

        //if (other.gameObject.layer == 9)
        //{
        //    MeshFilter meshFilter = GetComponent<MeshFilter>();
        //    MapManager.Instance.AddMeshToMap(meshFilter.mesh);
        //    Destroy(gameObject);
        //}
     
    }

    private void OnTriggerStay(Collider other)
    {
        //if (other.gameObject.layer == 9)
        //{
        //    if (!wasSeen)
        //    {
        //        wasSeen = true;
        //    }
        //    isColliding = true;
        //}
        //if (other.gameObject.layer == 9)
        //{
        //    MeshFilter meshFilter = GetComponent<MeshFilter>();
        //    MapManager.Instance.AddMeshToMap(meshFilter.mesh);
        //    Destroy(gameObject);
        //}
        //if (other.gameObject.layer == 9)
        //{
        //string otherGameObejctName = other.gameObject.name;
        //string substringName = otherGameObejctName.Substring(0, (otherGameObejctName.Length - 4)).Trim();
        //Debug.Log($"Update iam: {gameObject.name}  other: {substringName}");
        //if (gameObject.name == substringName)
        //{
        //    MapManager.Instance.UpdateMapComponent(meshFilter.mesh);
        //MapManager.Instance.DeleteMapComponent(other.name);
        //Destroy(other);
        //}
        //}
    }
}
