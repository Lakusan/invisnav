using System;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class MapManager : MonoBehaviour
{
    [SerializeField] ARMeshManager m_arMeshManager;
    [SerializeField] MeshFilter m_meshPrefab;
    [SerializeField] XROrigin m_xrOrigin;

 
    Dictionary<string, Mesh> meshDict;
    bool meshRendered = false;

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
        if (m.added != null && m.added.Count > 0)
        {
            foreach (MeshFilter mf in m.added)
            {

                // for each meshfilter, check if key with mf name exists in mapDict
                // if not add it
                // if it exists change vals of map dict
                // if its new check if segment exists in map
                //if not add it, if it does do nothing more
                GameObject mgGo = mf.gameObject;
                // raycast - if mesh -> nothing
                // if no mesh -> add
                if (meshDict.ContainsKey(mf.name))
                {
                    meshDict[mf.name] = mf.sharedMesh;
                }
                else
                {
                    meshDict.Add(mf.name, mf.sharedMesh);
                }
                Debug.Log($"count: {meshDict.Count}");

                Vector3 vertex = transform.TransformPoint(mf.mesh.vertices[0]);
                RaycastHit raycastHit;
                Ray vertexRayDown = new Ray(vertex, Vector3.down);
                Debug.DrawRay(vertexRayDown.origin, vertexRayDown.direction * 10, Color.green);
                if (Physics.Raycast(vertexRayDown, out raycastHit))
                {
                    Renderer renderer = raycastHit.collider.gameObject.GetComponent<Renderer>();
                    if (renderer != null) // Make sure the target has a Renderer component
                    {
                        Color randomColor = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
                        renderer.material.color = randomColor; // Change the color to green
                    }
                }

                // render one meshsdrftzu8i9,mn 
                //GameObject go = new GameObject();
                //go.AddComponent<MeshFilter>();
                //go.AddComponent<MeshRenderer>();

                //Mesh mesh = mf.mesh;
                //go.GetComponent<MeshFilter>().mesh = mesh;
                //MeshRenderer mr = go.GetComponent<MeshRenderer>();
                //mr.material.SetColor("_Color", Color.blue);
                //// to geht the mesh below the scanned mesh
                //go.transform.position   = mf.transform.position + Vector3.down;

                //Instantiate(go, gameObject.transform);
                //MyConsole.instance.Log("MESH -> ADD: " + mf.name);
                //if (gameObject.transform.Find(mf.name)?.gameObject == null)
                //{
                //}
            }
        }
        if(m.removed != null && m.removed.Count > 0)
        {
            foreach (MeshFilter mf in m.removed)
            {
                meshDict.Remove(mf.name);
            }
        }
        if (m.updated != null && m.updated.Count > 0)
        {
            foreach (MeshFilter mf in m.removed)
            {
                meshDict[mf.name] = mf.sharedMesh;
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

    public void DBButtonPressed()
    {
        m_arMeshManager.enabled = false;
        MyConsole.instance.Log("Meshing state" + m_arMeshManager.enabled);
        Debug.Log($"try to save meshes");
    }
 
}
