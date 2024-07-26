using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEditor;
using UnityEditor.Build;
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


    public static Dictionary<string, Mesh> meshDict;
    public string currentLocation = string.Empty;

    public double latitude = 0.0;
    public double longitude = 0.0;

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
        //    foreach (MeshFilter mf in m.removed)
        //    {
        //        meshDict[mf.name] = mf.sharedMesh;
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
        // deaktivate meshing
        toggleMeshing();
        DBManager.Instance.StoreNewLocation(meshDict);
        // redirect to main menu
    }

    private void MapRenderer(string name, Mesh mesh)
    {
        //foreach (KeyValuePair<string, Mesh> entry in meshDict)
        //{
        //    GameObject gO = new GameObject();
        //    gO.name = entry.Key;
        //    gO.AddComponent<MeshFilter>();
        //    gO.AddComponent<MeshRenderer>();
        //    MeshFilter mf = gO.GetComponent<MeshFilter>();
        //    mf.mesh = entry.Value;
        //    MeshRenderer mr = gO.GetComponent<MeshRenderer>();
        //    mr.material.SetColor("_Color", Color.blue);
        //    Instantiate(gO);
        //}
        GameObject gO = new GameObject();
        gO.SetActive(false);
        gO.transform.position = new Vector3(gO.transform.position.x, gO.transform.position.y + 1.1176f, gO.transform.position.z);
        gO.name = name;
        gO.AddComponent<MeshFilter>();
        gO.AddComponent<MeshRenderer>();
        MeshCollider meshCollider = gO.AddComponent<MeshCollider>();
        MeshFilter mf = gO.GetComponent<MeshFilter>();
        mf.mesh = mesh;
        MeshRenderer mr = gO.GetComponent<MeshRenderer>();
        mr.material.SetColor("_Color", Color.blue);
        meshCollider.sharedMesh = mesh;
        gO.layer = 8;
        gO.SetActive(true);
        Debug.Log("Render Meshes");
    }

    public void AddMeshToMap(Mesh mesh) 
    {
        string newMeshName = mesh.name + "-MAP";
        if (!meshDict.ContainsKey(newMeshName))
        {
            meshDict.Add(newMeshName, mesh);
            MapRenderer(newMeshName, mesh);
        }
    }
}
