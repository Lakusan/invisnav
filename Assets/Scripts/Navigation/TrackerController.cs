using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

[RequireComponent(typeof(LineRenderer))]   
public class TrackerController : MonoBehaviour
{
    MeshRenderer mr;
    LineRenderer lineRenderer;
    public GameObject target = null;
    public float pathOffsetY;
    public static TrackerController Instance { get; private set; }

    private void Awake()
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
        mr = GetComponent<MeshRenderer>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (IsTrackerOnNavMesh())
        {
            mr.material.color = Color.green;
        }
        else
        {
            mr.material.color = Color.red;
        }

        if (target != null)
        {
            CalculatePathToAnchor(target.transform.position);
        }
    }

    public bool IsTrackerOnNavMesh()
    {
        Vector3 agentPosition = gameObject.transform.position;
        NavMeshHit hit;

        if (NavMesh.SamplePosition(agentPosition, out hit, 1, NavMesh.AllAreas))
        {
            return true;
        }
        return false;
    }
    public void CalculatePathToAnchor(Vector3 anchorPos)
    {   if (lineRenderer.enabled == false)
        {
            lineRenderer.enabled = true;
        }
        NavMeshPath path = new NavMeshPath();
        Vector3 origin = gameObject.transform.position;
        NavMesh.CalculatePath(origin, anchorPos, NavMesh.AllAreas, path);
        Debug.Log($"path: {path.corners.Length}");
        if (path.corners.Length > 0)
        {
            Vector3[] elevatedCorners = path.corners;
            for (int i = 0; i < elevatedCorners.Length; i++)
            {
                elevatedCorners[i].y += pathOffsetY;
            }
            lineRenderer.positionCount = elevatedCorners.Length;
            lineRenderer.SetPositions(elevatedCorners);
        }
    }

    public void SetTarget(GameObject anchor)
    {
        target = anchor;
    }
}
