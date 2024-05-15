using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARRaycastManager))]

public class PlacementController : MonoBehaviour
{
    [SerializeField]
    private GameObject virtualAnchor;

    public GameObject VirtualAnchorPrefab
    {
        get { 
        return virtualAnchor;
        }
        set
        {
            virtualAnchor = value;
        }
    }

    private ARRaycastManager m_RaycastManager;

    private void Awake()
    {
        m_RaycastManager = GetComponent<ARRaycastManager>();
    }

    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if(Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }
        touchPosition = default;
        return false;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
