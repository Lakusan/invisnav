using System.Collections;
using UnityEngine;

public class CamTrackerRayCast : MonoBehaviour
{
    public GameObject mainCamera;
    private enum TRACKER_STATE
    {
        lookingForGround,
        lookingForNavMesh
    };
    private TRACKER_STATE state;

    public bool gotGoundLevel = false;
    public Vector3 groundLevel = Vector3.zero;
    private Coroutine lookingForGroundReference = null;
    private Coroutine lookingForNavMeshReference = null;

    [SerializeField]
    public string currentState;
    private Vector3[] origins;
    private Vector3 direction;
    private float rayLength = 100f;
    private float rayOffset = 0.1f;
    Vector3 mapMeshHitPosition = Vector3.negativeInfinity;
    [SerializeField] private Vector3 originMapTileOffset = Vector3.up / 5;

    public GameObject tracker;

    IEnumerator LookingForGround()
    {
        while (true)
        {
            if (IsThereGround())
            {
                state = TRACKER_STATE.lookingForNavMesh;
                // get GPS cords + device Rotation
                yield break;
            }
            yield return new WaitForSeconds(.1f);
        }
    }

    IEnumerator LookingForNavMesh()
    {
        while (true)
        {
            IsThereMapMesh();
            yield return new WaitForSeconds(.5f);
        }
    }

    private bool IsThereGround()
    {
        for (int i = 0; i < origins.Length; i++)
        {
            Debug.DrawRay(origins[i], direction * rayLength, Color.green);
            RaycastHit[] hits = Physics.RaycastAll(origins[i], direction, rayLength);
            if (hits.Length > 0)
            {
                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider.gameObject.layer == 8)
                    {
                        SetGroundLevelOnVector3Pos(hit.point);
                        return true;
                    }
                }
            }
        }
        return false;
    }
    private bool IsThereMapMesh()
    {
        for (int i = 0; i < origins.Length; i++)
        {
            Debug.DrawRay(origins[i], direction * rayLength, Color.blue);
            RaycastHit[] hits = Physics.RaycastAll(origins[i], direction, rayLength);
            if (hits.Length > 0)
            {
                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider.gameObject.layer == 8)
                    {
                        SetMapMeshHitPosition(hit.point);
                        return true;
                    }
                }
            }
        }
        return false;
    }


    void Start()
    {
        mainCamera = this.gameObject;
        this.state = TRACKER_STATE.lookingForGround;
        direction = Vector3.down;

    }
    void Update()
    {
        origins = new Vector3[]{
            transform.position,
            new Vector3 (transform.position.x + rayOffset ,transform.position.y,transform.position.z),
            new Vector3 (transform.position.x - rayOffset ,transform.position.y,transform.position.z),
            new Vector3 (transform.position.x ,transform.position.y,transform.position.z + rayOffset),
            new Vector3 (transform.position.x ,transform.position.y,transform.position.z - rayOffset),
            };
       

        currentState = this.state.ToString();

        switch (this.state)
        {
            case TRACKER_STATE.lookingForGround:
                if (!gotGoundLevel && lookingForGroundReference == null)
                {
                    lookingForGroundReference = StartCoroutine(LookingForGround());
                }
                break;
            case TRACKER_STATE.lookingForNavMesh:
                if (lookingForGroundReference != null)
                {
                    lookingForGroundReference = null;
                }
                if (lookingForNavMeshReference == null)
                {
                    lookingForNavMeshReference = StartCoroutine(LookingForNavMesh());
                }
                break;
            default:
                break;
        }
    }
    private void SetGroundLevelOnVector3Pos(Vector3 point)
    {
        groundLevel = point;
        gotGoundLevel = true;
        MapManager.Instance.goundLevelIsKnown = true;
    }
    private void SetMapMeshHitPosition(Vector3 position)
    {
        mapMeshHitPosition = position;
        tracker.transform.position = position;
        MapManager.Instance.lastTackerPositionOnNavMesh = mapMeshHitPosition;
        CreateFirstMapTile(position);
    }

    void CreateFirstMapTile(Vector3 position)
    {
        Vector3 offestPos = position + originMapTileOffset;
        if (MapManager.Instance.goundLevelIsKnown && MapManager.Instance.originTilePosition == Vector3.zero)
        {
            MapManager.Instance.originTilePosition = offestPos;
            MapManager.Instance.TryRenderNewTile(offestPos);
        }
    }
}

