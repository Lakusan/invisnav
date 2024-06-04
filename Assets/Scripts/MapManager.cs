using System;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.Management;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]



// make manager static instance
// add events as actions -> If map gets an add -> List -> Observer pattern
// action triggers job
// factory -> Decide which map conponent should me generated -> Map Prefab ( stairs, 
// iterator -> like for loop -> iterate over obejcts -> maybe fopr job

public class MapManager : MonoBehaviour
{
    [SerializeField] ARMeshManager m_arMeshManager;
    [SerializeField] MeshFilter m_meshPrefab;
    [SerializeField] XROrigin m_xrOrigin;
    Mesh m_mapMesh;
    GameObject m_Map;
    GameObject trackables;

    // XRSubsystem Prop to render Meshes
    private XRMeshSubsystem m_Subsystem;
    public XRMeshSubsystem subsystem => m_Subsystem;

    /*
     * - Gets submeshes, if added to map mesh
     * - Changes submeshes if update
     * - remove submeshes if removed
     * - saves mesh as map in firebase
     * - binary converter
     * - loads mesh
     */


    List<MeshInfo> m_MeshInfo;

    void Start()
    {
        m_mapMesh = new Mesh();
        m_Subsystem = null;
        GetComponent<MeshFilter>().mesh = m_mapMesh;
        var xrSettings = XRGeneralSettings.Instance;
        var xrManager = xrSettings.Manager;
        var xrLoader = xrManager.activeLoader;
        m_Subsystem = xrLoader.GetLoadedSubsystem<XRMeshSubsystem>();
        m_MeshInfo = new List<MeshInfo>();
    }

    IEnumerator WaitForMeshingEventInit()
    {
        while(m_arMeshManager == null)
        {
            yield return null;
        }
        m_arMeshManager.meshesChanged += map;
        trackables = m_xrOrigin.transform.Find("Trackables")?.gameObject;
    }

    private void OnEnable()
    {
        StartCoroutine(WaitForMeshingEventInit());
    }
    private void OnDisable()
    {
        m_arMeshManager.meshesChanged -= map;
    }

    void Update()
    {
    }

    public void map(ARMeshesChangedEventArgs m)
    {
        // meshinfo 
        //m_Subsystem.TryGetMeshInfos(m_MeshInfo);
        //Debug.Log("Mesh Infos: " + m_MeshInfo.Count);

        if (m.added != null && m.added.Count > 0)
        {
            foreach (MeshFilter mf in m.added)
            {
                MyConsole.instance.Log("MESH -> ADD: " + mf.name);
                if (gameObject.transform.Find(mf.name)?.gameObject == null)
                {
                    MeshFilter add = Instantiate(mf, gameObject.transform);
                    add.name = mf.name;
                }
            }
        }

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
        //if (m.removed != null && m.removed.Count > 0)
        //{
        //    foreach (MeshFilter mf in m.removed)
        //    {
        //        Debug.Log("MESH -> REMOVE: " + mf.name);
        //        if (gameObject.transform.Find(mf.name)?.gameObject != null)
        //        {
        //            GameObject go = gameObject.transform.Find(mf.name)?.gameObject;
        //            if (go != null)
        //            Destroy(go);
        //            Debug.Log("MESH -> DELETED: " + go.name);
        //        }
        //    }
        //}
        //if (trackables.transform.childCount > 0)
        //{
        //    MeshFilter[] meshFilters = trackables.GetComponentsInChildren<MeshFilter>();
        //    CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        //    int i = 0;
        //    while (i < meshFilters.Length)
        //    {
        //        combine[i].mesh = meshFilters[i].sharedMesh;
        //        combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
        //        //meshFilters[i].gameObject.SetActive(false);
        //        i++;
        //    }
        //    //Instantiate(m_meshPrefab, gameObject.transform);
        //    Mesh mesh = new Mesh();
        //    mesh.CombineMeshes(combine);
        //    mesh.RecalculateNormals();
        //    mesh.RecalculateBounds();
        //    transform.GetComponent<MeshFilter>().sharedMesh = mesh;


        //    //transform.gameObject.SetActive(true);

        //    /*
        //     * Ich brauche nur die adds und die updates, keine Removes
        //     * Dann Mesh optimieren
        //     * Sematic separation testen
        //     * Mesh braucht material
        //     * Dann nav mesh generierung versuchen -> extra in go
        //     * go für navigation
        //     */
        //}
    }

    public void toggleMeshing()
    {
        m_arMeshManager.enabled = !m_arMeshManager.enabled;
        MyConsole.instance.Log("Meshing state" + m_arMeshManager.enabled);
    }
}
