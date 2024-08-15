using System.Collections;
using UnityEngine;

public class MaptTileController : MonoBehaviour
{
    public string current_state;

    UnityEngine.Bounds bounds;
    float rayLength;
    Vector3 center;
    float halfWidth;
    float halfHeight;

    private MAPTILE_STATE state;

    public string attachedMapComponentName = "";
    Vector3 registeredPosition = Vector3.zero;

    private enum MAPTILE_STATE
    {
        start,
        findMap,
        findNeighbours,
        replicating,
        idle,
    };
    private bool corountineRunning = false;

    private void Start()
    {
        state = MAPTILE_STATE.start;
    }

    IEnumerator FindMapComponent()
    {
        if (IsThereMapComponent())
        {
            state = MAPTILE_STATE.findNeighbours;
        }
        else
        {
            yield return new WaitForSeconds(.5f);
        }
    }
    private bool IsThereMapComponent()
    {
        // bottom center
        Vector3 bottomCenter = center;
        //Debug.DrawRay(bottomCenter, -transform.up * rayLength, Color.green);

        Vector3[] origins = {
            bottomCenter,
            bottomCenter + new Vector3 (.05f,0f,0f),
            bottomCenter + new Vector3 (-.05f,0f,0f),
            bottomCenter + new Vector3 (0f,0f,.05f),
            bottomCenter + new Vector3 (0f,0f,-.05f),

            bottomCenter + new Vector3 (bounds.extents.x,0f,0f),
            bottomCenter + new Vector3 (-bounds.extents.x,0f,0f),
            bottomCenter + new Vector3 (0f,0f,bounds.extents.x),
            bottomCenter + new Vector3 (0f,0f,-bounds.extents.x),
            };

        Vector3 direction = -transform.up * rayLength * 5;

        for (int i = 0; i < origins.Length; i++)
        {
            Debug.DrawRay(origins[i], direction, Color.green);
            RaycastHit[] hits = Physics.RaycastAll(origins[i], direction, rayLength);
            if (hits.Length > 0)
            {
                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider.gameObject.layer == 8)
                    {
                        Debug.Log("Hit object: " + hit.collider.gameObject.name);
                        attachedMapComponentName = hits[0].collider.gameObject.name;
                        return true;
                    }
                }
            }
        }
        //RaycastHit hit;
        //if (Physics.Raycast(bottomCenter, -transform.up, out hit, rayLength))
        //{
        //    Debug.Log($"HIT {hit.collider.gameObject.name}");
        //    if (hit.collider.gameObject.layer == 8)
        //    {
        //        attachedMapComponentName = hit.collider.gameObject.name;
        //        return true;
        //    }
        //}
        return false;
    }

    void Update()
    {

        bounds = GetComponent<Renderer>().bounds;
        center = bounds.center;
        halfWidth = bounds.extents.x;
        halfHeight = bounds.extents.z;
        rayLength = bounds.size.z;

        current_state = state.ToString();
        switch (state)
        {
            case MAPTILE_STATE.start:
                state = MAPTILE_STATE.findMap;
                break;
            case MAPTILE_STATE.findMap:
                if (!corountineRunning)
                {
                    StartCoroutine(FindMapComponent());
                }
                break;
            case MAPTILE_STATE.findNeighbours:
                // check directions for other tiles; if no tile -> replicate
                Vector3 leftCenter = center - transform.right * halfWidth;
                Debug.DrawRay(leftCenter, -transform.right * rayLength, Color.red);
                SendRayCast(leftCenter, -transform.right, rayLength, "left");

                //right side center
                Vector3 rightCenter = center + transform.right * halfWidth;
                Debug.DrawRay(rightCenter, transform.right * rayLength, Color.green);
                SendRayCast(rightCenter, transform.right, rayLength, "right");

                //front side center
                Vector3 forwardCenter = center + transform.forward * halfWidth;
                Debug.DrawRay(forwardCenter, transform.forward * rayLength, Color.blue);
                SendRayCast(forwardCenter, transform.forward, rayLength, "forward");

                //Back side center
                Vector3 backwardsCenter = center - transform.forward * halfWidth;
                Debug.DrawRay(backwardsCenter, -transform.forward * rayLength, Color.magenta);
                SendRayCast(backwardsCenter, -transform.forward, rayLength, "backwards");
                state = MAPTILE_STATE.replicating;
                break;
            case MAPTILE_STATE.replicating:
                    AddSelfToMap();
                    state = MAPTILE_STATE.idle;
                break;
            case MAPTILE_STATE.idle:
                break;
            default:
                break;
        }
    }

    void SendRayCast(Vector3 origin, Vector3 direction, float length, string invokerName)
    {
        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, length))
        {
            if (hit.collider.gameObject.layer != 11)
            {
                registeredPosition = gameObject.transform.position;
                gameObject.name = "MapTile_on_" + attachedMapComponentName + "_at_" + registeredPosition.ToString();
                Debug.Log($"REPLICATE in direction: {direction.ToString()} on {invokerName}");
                Vector3 posNewTile = registeredPosition + (direction * length);
                MapManager.Instance.TryRenderNewTile(posNewTile);
            }

        }
        else
        {
            registeredPosition = gameObject.transform.position;
            gameObject.name = "MapTile_on_" + attachedMapComponentName + "_at_" + registeredPosition.ToString();
            Debug.Log($"REPLICATE in direction: {direction.ToString()} on {invokerName}");
            Vector3 posNewTile = registeredPosition + (direction * length);
            MapManager.Instance.TryRenderNewTile(posNewTile);
        }
    }

    void AddSelfToMap()
    {
        MapManager.Instance.RegisterValidatedMapTile(attachedMapComponentName, registeredPosition);
    }
}
