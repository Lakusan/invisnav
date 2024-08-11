using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(LineRenderer))]
public class NavMeshPathDrawer : MonoBehaviour
{
    private LineRenderer lineRenderer;

    [SerializeField] private NavMeshAgent cameraTrackerAgent;
    [SerializeField] private GameObject anchorManager;
    [SerializeField] private GameObject uiContainer;

    public static NavMeshPathDrawer Instance { get; private set; }

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
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        anchorManager.SetActive(false);
    }

    public void SetDestinationAnchor(GameObject destinationAnchor)
    {
        //Vector3 destination = new Vector3(destinationAnchor.posX, destinationAnchor.posY, destinationAnchor.posZ);
        cameraTrackerAgent.SetDestination(destinationAnchor.gameObject.transform.position);
    }

    void DrawPath(NavMeshPath path)
    {
        for (int i = 0; i < path.corners.Length; i++)
        {
            Vector3 corners = new Vector3(path.corners[i].x, path.corners[i].y, path.corners[i].z);
            lineRenderer.SetPosition(i, corners);
        }
    }

    public NavMeshPath CalculatePathToAnchor(Vector3 anchorPos)
    {
        NavMeshPath path = new NavMeshPath();
        Vector3 origin = cameraTrackerAgent.gameObject.transform.position;
        NavMesh.CalculatePath(origin, anchorPos, NavMesh.AllAreas, path);
        Debug.Log($"PaTH: {path.corners.Length}");
        if (path.corners.Length > 2)
        {
            lineRenderer.positionCount = path.corners.Length;
            lineRenderer.SetPositions(path.corners);
            return path;
        }
        else
        {
            Debug.Log("Path is Null");
            lineRenderer.positionCount = 0;
            return null;
        }
    }

    public void NavigateToAnchor(GameObject anchor)
    {
        lineRenderer.enabled = true;
        SetDestinationAnchor(anchor);
        CalculatePathToAnchor(anchor.transform.position);
    }

    public void Navigate()
    {
        CalculatePathToAnchor(cameraTrackerAgent.destination);
    }
}
