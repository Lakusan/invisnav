using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }

    [SerializeField] GameObject arMeshingGO;
    [SerializeField] ARMeshManager m_arMeshManager;
    [SerializeField] MeshFilter m_meshPrefab;
    [SerializeField] XROrigin m_xrOrigin;
    [SerializeField] GameObject mapContainer;
    [SerializeField] Material MapMaterial;
    [SerializeField] GameObject anchorPrefab;
    [SerializeField] GameObject anchorContainer;
    [SerializeField] GameObject anchorNavPrefab;
    [SerializeField] GameObject mapTilePrefab;
    [SerializeField] GameObject mapTileContainer;

    [SerializeField] public int tilesCount;

    [SerializeField] public Dictionary<string, string> tilesDict;

    public static Dictionary<string, Mesh> meshDict;
    public string currentLocation = string.Empty;

    public static float latitudeOrigin;
    public static float longitudeOrigin;
    public static float trueHeading;

    public static List<Anchor> anchorList = new List<Anchor>();
    [SerializeField]
    public Vector3 lastTackerPositionOnNavMesh;
    public Vector3 originTilePosition;
    public bool goundLevelIsKnown = false;
    public bool enableMapMeshRendering = false;

    public List<GameObject> navigateableAnchors = new List<GameObject>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        meshDict = new Dictionary<string, Mesh>();
        tilesDict = new Dictionary<string, string>();
        lastTackerPositionOnNavMesh = Vector3.zero;
    }
    void Update()
    {
        tilesCount = tilesDict.Count;
    }

    public void ToggleMeshing()
    {
        arMeshingGO = GameObject.Find("AR Meshing");
        m_arMeshManager = arMeshingGO.GetComponent<ARMeshManager>();
        m_arMeshManager.enabled = !m_arMeshManager.enabled;
    }
    public void DeactivateMeshing()
    {
        arMeshingGO = GameObject.Find("AR Meshing");
        m_arMeshManager = arMeshingGO.GetComponent<ARMeshManager>();
        m_arMeshManager.enabled = false;
    }
    public void ActivateMeshing()
    {
        arMeshingGO = GameObject.Find("AR Meshing");
        m_arMeshManager = arMeshingGO.GetComponent<ARMeshManager>();
        m_arMeshManager.enabled = true;

    }

    public void DBButtonPressed()
    {
        Debug.Log($"try to save meshes");
    
        StoreMap();
        Debug.Log("Meshes Saved!!");
    }

    public void StoreMap()
    {
        DeactivateMeshing();
        DBManager.Instance.StoreNewLocation(meshDict, anchorList, trueHeading, latitudeOrigin, longitudeOrigin);
        Debug.Log($"lon: {longitudeOrigin}");
        Debug.Log($"lat: {latitudeOrigin}");
        Debug.Log($"head: {trueHeading}");
    }

    private void MapComponentRenderer(string name, Mesh mesh)
    {
        GameObject gO = new GameObject();
        gO.transform.SetParent(mapContainer.transform);
        gO.SetActive(false);
        //gO.transform.position = new Vector3(gO.transform.position.x, gO.transform.position.y + 1.1176f, gO.transform.position.z);
        gO.name = name;
        gO.AddComponent<MeshFilter>();
        if (enableMapMeshRendering)
        {
            MeshRenderer mr = gO.AddComponent<MeshRenderer>();
            mr.enabled = true;
            mr.material = MapMaterial;
        }
        MeshCollider meshCollider = gO.AddComponent<MeshCollider>();
        MeshFilter mf = gO.GetComponent<MeshFilter>();
        mf.mesh = mesh;
        meshCollider.sharedMesh = mesh;
        gO.layer = 8;
        gO.SetActive(true);
    }

    public void AddMeshToMap(Mesh mesh)
    {
        string newMeshName = mesh.name + "-MAP";
        if (!meshDict.ContainsKey(newMeshName))
        {
            meshDict.Add(newMeshName, mesh);
            MapComponentRenderer(newMeshName, mesh);
        }
        else
        {
            UpdateMapComponent(mesh);
        }
    }

    public void AddMeshToLoadedMap(Mesh mesh, string index)
    {
        string newMeshName = index;
        if (!meshDict.ContainsKey(newMeshName))
        {
            meshDict.Add(newMeshName, mesh);
            //MapRenderer(newMeshName, mesh);
        }
    }

    public void MapRenderer()
    {
        foreach (KeyValuePair<string, Mesh> kvp in meshDict)
        {
            MapComponentRenderer(kvp.Key,kvp.Value);
        }
    }

    public void RenderAllAnchors()
    {
        foreach(Anchor anchor in anchorList)
        {
            RenderAnchor(anchor);
        }
    }


    public void AddAnchorToLoadedMap(Anchor anchor)
    {
        MyConsole.instance.Log("Add Anchor to Loaded Map: " + anchor.posX);

        anchorList.Add(anchor);
        //RenderAnchor(anchor);
    }
    public void RenderAnchor(Anchor anchor)
    {
        // new GO with anchor name
        Debug.Log($"anchorname : {anchor.anchorName}");
        Debug.Log($"anchor desc : {anchor.anchorDescription}");
        Debug.Log($"posX : {anchor.posX}");
        Debug.Log($"posY : {anchor.posY}");
        Debug.Log($"posZ : {anchor.posZ}");

        GameObject go = Instantiate(anchorNavPrefab);
        go.SetActive(false);
        go.name = anchor.anchorName;
        go.transform.parent = anchorContainer.transform;
        // anchor position
        Vector3 position = new Vector3(anchor.posX, anchor.posY, anchor.posZ);
        go.transform.position = position;
        // anchor description
        NavAnchorController nac = go.GetComponent<NavAnchorController>();
        nac.SetAnchorNameToTMP(anchor.anchorName);
        nac.SetAnchorDescriptionToTMP(anchor.anchorDescription);
        Debug.Log($"GO : {go.name}");
        go.SetActive(true);
        navigateableAnchors.Add(go);
    }

    public void UpdateMapComponent(Mesh mesh)
    {
        string meshname = mesh.name + "-MAP";
        Transform go = mapContainer.gameObject.transform.Find(meshname);
        go.gameObject.SetActive(false);
        MeshFilter filter = go.GetComponent<MeshFilter>();
        if (enableMapMeshRendering)
        {
            MeshRenderer mr = go.GetComponent<MeshRenderer>();
            mr.material.color = Color.cyan;
        }
        filter.mesh = mesh;
        go.gameObject.SetActive(true);
    }

    public void DeleteMapComponent(string name)
    {
        if (meshDict.ContainsKey(name))
        {
            meshDict.Remove(name);
        }
    }

    public void AddAnchor(Anchor anchor)
    {
        anchorList.Add(anchor);
    }

    public void AlignLoadedMap()
    {
        // gps origin
        // gps device

        // distance + bearing
        // XYZ unity origin

        // align Map
        //LocationManager.Instance.GetGPSLocationFromSensors();
        ////Debug.DrawRay(originGO.transform.position, rotationToObject, Color.red);
        ////calculate Distance
        //float distanceToObject = LocationManager.Instance.CalculateVincentyDistance(
        //    latitudeOrigin, 
        //    longitudeOrigin,
        //    LocationManager.Instance.lastGpsCoords[0],
        //    LocationManager.Instance.lastGpsCoords[0]);
        //LocationManager.Instance.CalculateBearing();
        //Vector3 north = GetNorthVector(originGO.transform);
        //DrawBearingAndNorth(originGO.transform.position, rotationToObject, originGO.transform);

        //Vector3 mapPos =  LocationManager.Instance.GetUnityXYZFromGPS(latitudeOrigin,longitudeOrigin);
        //mapContainer.transform.position = mapPos;
    }

    public GameObject FindAnchorGO(string anchorName)
    {
        GameObject foundObject = anchorContainer.transform.Find(anchorName).gameObject;

        if (foundObject != null)
        {
            return foundObject;
        }
        else
        {
            Debug.LogError("GameObject with name '" + name + "' not found!");
            return null;
        }
    }

    public bool TryRenderNewTile(Vector3 position)
    {
        if (!tilesDict.ContainsKey(position.ToString()))
        {
            GameObject newTile = Instantiate(mapTilePrefab, position, Quaternion.identity);
            newTile.SetActive(false);
            newTile.transform.parent = mapTileContainer.transform;
            newTile.SetActive(true);
            return true;
        }
        return false;
    }

    public void RegisterValidatedMapTile(string attachedfMapComponentName, Vector3 pos)
    {
        string position = pos.ToString();
        if (!tilesDict.ContainsKey(position.ToString()))
        {
            tilesDict.Add(position, attachedfMapComponentName);
        }
    }
}
