using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(LineRenderer))]   
public class TrackerController : MonoBehaviour
{
    MeshRenderer mr;
    LineRenderer lineRenderer;
    public GameObject target = null;
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
            lineRenderer.positionCount = path.corners.Length;
            lineRenderer.SetPositions(path.corners);
        }
    }
}
