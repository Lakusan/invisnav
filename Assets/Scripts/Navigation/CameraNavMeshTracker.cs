using System;
using System.Collections;
using System.ComponentModel.Design;
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
    public GameObject mainCamera;

    [SerializeField]
    public float distanceThreshold = 20f;
    [SerializeField]
    public string currentState;
    [SerializeField]
    public bool dropTracker = true;
    [SerializeField]
    private float distanceCamTracker = 0.0f;
    [SerializeField]
    private float distanceCamGround = 0.0f;

    private TRACKER_STATE state;

    private NavMeshAgent trackerNavMeshAgent;

    private float groundLevel = 0.0f;

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
        //if(MapManager.Instance.LastTackerPositionOnNavMesh.y != 0.0f)
        //{
        //    groundLevel = Math.Abs(MapManager.Instance.LastTackerPositionOnNavMesh.y) +1;
        //    distanceThreshold = groundLevel;
        //} 
       
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
                StartCoroutine(FindNavMeshWithTracker());
                break;
            case TRACKER_STATE.isUpdating:
            
                break;
            case TRACKER_STATE.isOnNavMesh:
                trackerNavMeshAgent.enabled = true;
                // if navigating update path
                NavMeshPathDrawer.Instance.Navigate();
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
                //groundLevel = Math.Abs(MapManager.Instance.LastTackerPositionOnNavMesh.y) + 1f;
                //distanceThreshold = groundLevel;
                this.state = TRACKER_STATE.isOnNavMesh;
                //trackerNavMeshAgent.enabled = true;
                yield break;
            }
            if(distanceThreshold < 100)
            {

                distanceThreshold++;
            } else
            {
                distanceThreshold = 0;
            }
            yield return null;
        }
        this.state = TRACKER_STATE.isFalling;
        yield break;
    }
    public void CalculatePathToAnchor(Vector3 anchorPos)
    {
        NavMeshPath path = new NavMeshPath();
        Vector3 origin = mainCamera.transform.position;
        NavMesh.CalculatePath(mainCamera.transform.position, anchorPos, NavMesh.AllAreas, path);
        if (path != null)
        {
            for (int i = 0; i < path.corners.Length; i++)
            {
                Debug.DrawLine(origin, path.corners[i], Color.red);
            }
        }
        else
        {
            Debug.Log("CamNavMeshTracker: Path is Null");
        }
    }
    private void DropTracker()
    {
        trackerRigidbody.velocity = Vector3.zero;
        distanceThreshold = 0;  
        DisableGravityOnTracker();
        trackerNavMeshAgent.enabled = false;
        trackerGO.transform.position = mainCamera.transform.position;
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
            if (Mathf.Abs(mainCamera.transform.position.y - trackerGO.transform.position.y) > distanceThreshold)
            {
                return true;
            }
        }
        else if(this.state == TRACKER_STATE.isOnNavMesh)
        {
            Debug.DrawLine(mainCamera.transform.position, trackerGO.transform.position, Color.black);
            Vector3 groundVector = RayCastToGround();
            if (groundVector != Vector3.zero)
            {
                distanceCamTracker = Math.Abs(Vector3.Distance(mainCamera.transform.position, trackerGO.transform.position));
                distanceCamGround = Math.Abs(Vector3.Distance(mainCamera.transform.position, groundVector));
                if (distanceCamTracker > distanceCamGround)
                {
                    Debug.DrawLine(mainCamera.transform.position, trackerGO.transform.position, Color.red);
                    return true;
                }
            }
        }
        return false;
    }

    private void SetLastTrackerPosition(Vector3 position)
    {
        MapManager.Instance.LastTackerPositionOnNavMesh = position;
    }

    private Vector3 RayCastToGround()
    {
        RaycastHit hit;
        Debug.DrawRay(mainCamera.transform.position, Vector3.down * 100, Color.cyan);
        if (Physics.Raycast(mainCamera.transform.position, Vector3.down * 100,  out hit))
        {
            if(hit.collider.gameObject.layer == 10)
            {
                Debug.DrawRay(mainCamera.transform.position, hit.point, Color.yellow);

                return hit.point;
            }
        }
        return Vector3.zero;
    }
}
