using UnityEngine;

public class MapComponentControler : MonoBehaviour
{
    MeshFilter meshFilter;
    [SerializeField] bool wasSeen = false;
    [SerializeField] bool isColliding = false;
    [SerializeField] public GameObject prefab;
    [SerializeField] public GameObject container;

    string attachedMapComponentName = "";

    void Start()
    { 
        meshFilter = GetComponent<MeshFilter>();
        container = GameObject.Find("MapContainer");
    }
    private void Update()
    {
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            MapManager.Instance.AddMeshToMap(meshFilter.mesh);
            Destroy(gameObject);
        }
    }
}
