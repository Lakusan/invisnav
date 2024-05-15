using System;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.ARFoundation;


public class MeshConverter : MonoBehaviour
{
    Mesh mesh;
    GameObject xrOrigin;
    GameObject arMeshRenderer;
    Component arMeshManager;
    bool done = false;

    void Start()
    {
        xrOrigin = GameObject.Find("XR Origin");
        arMeshRenderer = xrOrigin.GetNamedChild("AR Meshing");
        arMeshManager = arMeshRenderer.GetComponent<ARMeshManager>();
        // XR Origin -> AR Meshing -> ARMeshManager -> AR Mesh
        

    }

    void Update()
    {
        //Debug.Log(arMeshRenderer.GetComponent<ARMeshManager>().meshes);
        if (arMeshRenderer.GetComponent<ARMeshManager>().meshes.Count > 0)
        {
            //Debug.Log(arMeshRenderer.GetComponent<ARMeshManager>().meshes[0].name);
        }
    }
}
