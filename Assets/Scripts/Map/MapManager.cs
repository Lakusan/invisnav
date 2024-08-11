using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class MapManager : MonoBehaviour
{

    public static MapManager Instance { get; private set; }

    [SerializeField] ARMeshManager m_arMeshManager;
    [SerializeField] MeshFilter m_meshPrefab;
    [SerializeField] XROrigin m_xrOrigin;
    [SerializeField] GameObject mapContainer;
    [SerializeField] Material MapMaterial;
    [SerializeField] GameObject anchorPrefab;
    [SerializeField] GameObject anchorContainer;
    [SerializeField] GameObject anchorNavPrefab;

    public static Dictionary<string, Mesh> meshDict;
    public string currentLocation = string.Empty;

    public float latitudeOrigin = 0.0f;
    public float longitudeOrigin = 0.0f;

    public List<Anchor> anchorList = new List<Anchor>();
    [SerializeField]
    public Vector3 LastTackerPositionOnNavMesh;

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

    }
    void Start()
    {
        meshDict = new Dictionary<string, Mesh> ();
        LastTackerPositionOnNavMesh = Vector3.zero;
    }

    IEnumerator WaitForMeshingEventInit()
    {
        while(m_arMeshManager == null)
        {
            yield return null;
        }
        m_arMeshManager.meshesChanged += map;
    }

    private void OnEnable()
    {
        StartCoroutine(WaitForMeshingEventInit());
    }
    private void OnDisable()
    {
        m_arMeshManager.meshesChanged -= map;
    }
    public void map(ARMeshesChangedEventArgs m)
    {
        //if (m.added != null && m.added.Count > 0)
        //{
        //    foreach (MeshFilter mf in m.added)
        //    {

        //        // for each meshfilter, check if key with mf name exists in mapDict
        //        // if not add it
        //        // if it exists change vals of map dict

        //        if (meshDict.ContainsKey(mf.name))
        //        {
        //            meshDict[mf.name] = mf.sharedMesh;
        //        }
        //        else
        //        {
        //            meshDict.Add(mf.name, mf.sharedMesh);
        //        }
        //        Debug.Log($"count: {meshDict.Count}");
        //    }
        //}
        //if (m.added != null && m.added.Count > 0)
        //{
        //    foreach( MeshFilter mf in m.added)
        //    {
        //        //AddMeshToMap(mf.mesh);
        //    }
        //}

        //foreach (KeyValuePair<string, Mesh> entry in meshDict)
        //{
        //    Debug.Log($"key: {entry.Key} Name: {entry.Value.name}");
        //}
        //if (m.removed != null && m.removed.Count > 0)
        //{
        //    foreach (MeshFilter mf in m.removed)
        //    {
        //        meshDict.Remove(mf.name);
        //        if (gameObject.transform.Find(mf.name)?.gameObject != null)
        //        {
        //            Destroy(mf);
        //            MeshFilter update = Instantiate(mf, gameObject.transform);
        //            update.name = mf.name;
        //            Debug.Log("MESH -> UPDATED: " + mf.name);
        //        }
        //    }
        //}
        //if (m.removed != null && m.removed.Count > 0)
        //{
        //    foreach (MeshFilter mf in m.removed)
        //    {
        //        //AddMeshToMap(mf.mesh);
        //    }
        //}
        //if (m.updated != null && m.updated.Count > 0)
        //{
        //    foreach (MeshFilter mf in m.updated)
        //    {
        //        meshDict[mf.name] = mf.sharedMesh;
        //        UpdateMapComponent(mf.mesh);
        //    }
        //}
        //if (m.updated != null && m.updated.Count > 0)
        //{
        //    foreach (MeshFilter mf in m.updated)
        //    {
        //        if (GameObject.Find(mf.name))
        //        {
        //            Debug.Log("MESH -> UPDATE: " + mf.name);
        //            if (gameObject.transform.Find(mf.name)?.gameObject != null)
        //            {
        //                Destroy(mf);
        //                MeshFilter update = Instantiate(mf, gameObject.transform);
        //                update.name = mf.name;
        //                Debug.Log("MESH -> UPDATED: " + mf.name);
        //            }
        //        }
        //    }
        //}
    }

    public void toggleMeshing()
    {
        m_arMeshManager.enabled = !m_arMeshManager.enabled;
        MyConsole.instance.Log("Meshing state" + m_arMeshManager.enabled);
        Debug.Log($"meshing {m_arMeshManager.enabled}");
    }
    public void DeactivateMeshing()
    {
      m_arMeshManager.enabled = false;
        MyConsole.instance.Log("Meshing state" + m_arMeshManager.enabled);
        Debug.Log($"meshing {m_arMeshManager.enabled}");
    }
    public void ActivateMeshing()
    {
        m_arMeshManager.enabled = true;
        MyConsole.instance.Log("Meshing state" + m_arMeshManager.enabled);
        Debug.Log($"meshing {m_arMeshManager.enabled}");
    }

    public void DBButtonPressed()
    {
        Debug.Log($"try to save meshes");
        StoreMap();
        Debug.Log("Meshes Saved!!");
    }

    public void StoreMap()
    {
        toggleMeshing();
        DBManager.Instance.StoreNewLocation(meshDict, anchorList);
        // TODO: redirect to main menu
    }

    private void MapRenderer(string name, Mesh mesh)
    {
        GameObject gO = new GameObject();
        gO.transform.SetParent(mapContainer.transform);
        gO.SetActive(false);
        //gO.transform.position = new Vector3(gO.transform.position.x, gO.transform.position.y + 1.1176f, gO.transform.position.z);
        gO.name = name;
        gO.AddComponent<MeshFilter>();
        MeshRenderer mr = gO.AddComponent<MeshRenderer>();
        mr.enabled = true;
        MeshCollider meshCollider = gO.AddComponent<MeshCollider>();
        MeshFilter mf = gO.GetComponent<MeshFilter>();
        mf.mesh = mesh;
        mr.material = MapMaterial;
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
            MapRenderer(newMeshName, mesh);
        } else
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
            MapRenderer(newMeshName, mesh);
        }
    }

    public void AddAnchorToLoadedMap(Anchor anchor)
    {
        Debug.Log($"Add Anchor to Loaded Map: {anchor.posX}");

        anchorList.Add(anchor);
        RenderAnchor(anchor);
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
       MeshRenderer mr = go.GetComponent<MeshRenderer>();
        mr.material.color = Color.cyan;
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

    public Vector3 GetLastTrackerPosition()
    {
        return LastTackerPositionOnNavMesh;
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

        Vector3 mapPos =  LocationManager.Instance.GetUnityXYZFromGPS(latitudeOrigin,longitudeOrigin);
        mapContainer.transform.position = mapPos;
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
}
