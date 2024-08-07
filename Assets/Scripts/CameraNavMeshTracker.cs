using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CameraNavMeshTracker : MonoBehaviour
{
    private bool trackerIsOnNavMesh = false;
    private Vector3 trackerPosVec = Vector3.zero;

    [SerializeField]
    public GameObject trackerGO;
    private Rigidbody trackerRigidbody;

    [SerializeField]
    public float distanceThreshold = 20f;
    [SerializeField]
    public string currentState;
    [SerializeField]
    public bool dropTracker = true;

    private TRACKER_STATE state;

    private NavMeshAgent trackerNavMeshAgent;

    private enum TRACKER_STATE
    {
        idle,
        isFalling,
        shouldUpdate,
        isUpdating,
        isOnNavMesh
    };

    void Start()
    {
        this.state = TRACKER_STATE.isFalling;
        trackerRigidbody = trackerGO.GetComponent<Rigidbody>();
        trackerPosVec = transform.position;
        trackerNavMeshAgent = trackerGO.GetComponent<NavMeshAgent>();
    }

    void Update()
    {
      currentState = this.state.ToString();
        if (dropTracker)
        {
            trackerRigidbody.useGravity = true;
            dropTracker = false;
            this.state = TRACKER_STATE.isFalling;
        }
        switch (this.state)
        {
            case TRACKER_STATE.idle:

                break;
            case TRACKER_STATE.isFalling:
                // check if tracker is too far away -> redrop
                if (IsTrackerOutOfRange())
                {
                    this.state = TRACKER_STATE.shouldUpdate;
                }
                break;
            case TRACKER_STATE.shouldUpdate:
                // coroutinge to find navmesh
                StartCoroutine(FindNavMeshWithTracker());
                break;
            case TRACKER_STATE.isUpdating:
            
                break;
            case TRACKER_STATE.isOnNavMesh:
                if (IsTrackerOutOfRange())
                {
                    trackerNavMeshAgent.enabled = false;
                    this.state = TRACKER_STATE.shouldUpdate;
                }
                break;
            default:
                break;
        }
    }
    IEnumerator FindNavMeshWithTracker()
    {
        DropTracker();
        this.state = TRACKER_STATE.isUpdating;
        while (!IsTrackerOutOfRange())
        {
            if (IsTrackerOnNavMesh())
            {
                this.state = TRACKER_STATE.isOnNavMesh;
                trackerNavMeshAgent.enabled = true;
                yield break;
            }
            yield return null;
        }
        this.state = TRACKER_STATE.isFalling;
        yield break;
    }
    public void CalculatePathToAnchor(Vector3 anchorPos)
    {
        NavMeshPath path = new NavMeshPath();
        Vector3 origin = Camera.main.transform.position;
        NavMesh.CalculatePath(Camera.main.transform.position, anchorPos, NavMesh.AllAreas, path);
        if (path != null)
        {
            for (int i = 0; i < path.corners.Length; i++)
            {
                Debug.DrawLine(origin, path.corners[i], Color.red);
            }
        }
        else
        {
            Debug.Log("Path is Null");
        }
    }
    private void DropTracker()
    {
        trackerRigidbody.velocity = Vector3.zero;
        DisableGravityOnTracker();
        trackerGO.transform.position = Camera.main.transform.position;
        EnableGravitiyOnTracker();
        this.state = TRACKER_STATE.isFalling;
    }

    private void EnableGravitiyOnTracker()
    {
        trackerRigidbody.useGravity = true;
    }
    private void DisableGravityOnTracker()
    {
        trackerRigidbody.useGravity = false;
    }

    public bool IsTrackerOnNavMesh()
    {
        Vector3 agentPosition = trackerGO.transform.position;
        NavMeshHit hit;

        if (NavMesh.SamplePosition(agentPosition, out hit, 1, NavMesh.AllAreas))
        {
            SetLastTrackerPosition(agentPosition);
            return true;
        }

        return false;
    }
    private bool IsTrackerOutOfRange()
    {
        if (this.state == TRACKER_STATE.isFalling || this.state == TRACKER_STATE.isUpdating)
        {
            if (Mathf.Abs(Camera.main.transform.position.y - trackerGO.transform.position.y) > distanceThreshold)
            {
                return true;
            }
        }
        else if(this.state == TRACKER_STATE.isOnNavMesh)
        {
            if(Vector3.Distance(Camera.main.transform.position, trackerGO.transform.position) > (distanceThreshold/10))
            {
                return true;
            }
        }
        return false;
    }

    private void SetLastTrackerPosition(Vector3 position)
    {
        MapManager.Instance.LastTackerPositionOnNavMesh = position;
    }
}
