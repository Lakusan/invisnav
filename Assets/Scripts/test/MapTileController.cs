using System.Collections;
using UnityEngine;

public class MaptTileController: MonoBehaviour
{
    public string current_state;

    UnityEngine.Bounds bounds;
    float rayLength;
    Vector3 center;
    float halfWidth;
    float halfHeight;

    private MAPTILE_STATE state;
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
        bounds = GetComponent<Renderer>().bounds;
        center = bounds.center;
        halfWidth = bounds.extents.x;
        halfHeight = bounds.extents.z;
        rayLength = bounds.size.z;
    }

    IEnumerator FindMapComponent()
    {
        if (IsThereMapComponent())
        {
            state = MAPTILE_STATE.findNeighbours;
        } else
        {
            yield return new WaitForSeconds(1.0f);
        }
    }
    private bool IsThereMapComponent()
    {
        // bottom center
        Vector3 bottomCenter = center;
        Debug.DrawRay(bottomCenter, -transform.up * rayLength, Color.green);

        if (Physics.Raycast(bottomCenter, -transform.up, rayLength))
        {
            return true;
        }
        return false;
    }

    void Update()
    {
        current_state = state.ToString();
        switch (state)
        {
            case MAPTILE_STATE.start:
                // checks if its origin. if not gets map height
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
                SendRayCast(leftCenter, -transform.right * rayLength, rayLength, "left");

                //right side center
                Vector3 rightCenter = center + transform.right * halfWidth;
                SendRayCast(rightCenter, transform.right * rayLength, rayLength, "right");

                //front side center
                Vector3 forwardCenter = center + transform.forward * halfWidth;
                SendRayCast(forwardCenter, transform.forward * rayLength, rayLength, "forward");

                //Back side center
                Vector3 backwardsCenter = center - transform.forward * halfWidth;
                SendRayCast(backwardsCenter, -transform.forward * rayLength, rayLength, "backwards");
                state = MAPTILE_STATE.replicating;
                break;
            case MAPTILE_STATE.replicating:
                state = MAPTILE_STATE.idle;
                break;
            case MAPTILE_STATE.idle:

                break;
            default:
                break;
        }
        // Assuming the plane is centered at the origin and has a scale of 1
        // Get plane's dimensions and center


        //top center
        //Vector3 topCenter = center;
        //Debug.DrawRay(topCenter, transform.up * rayLength, Color.red);

        //// bottom center
        //Vector3 bottomCenter = center;
        //Debug.DrawRay(bottomCenter, -transform.up * rayLength, Color.green);

        ////left side center
        //Vector3 leftCenter = center - transform.right * halfWidth;
        //Debug.DrawRay(leftCenter, -transform.right * rayLength, Color.blue);
        ////SendRayCast(leftCenter, -transform.right, rayLength, "left");

        ////right side center
        //Vector3 rightCenter = center + transform.right * halfWidth;
        //Debug.DrawRay(rightCenter, transform.right * rayLength, Color.yellow);
        ////SendRayCast(rightCenter, transform.right, rayLength, "right");

        ////front side center
        //Vector3 forwardCenter = center + transform.forward * halfWidth;
        //Debug.DrawRay(forwardCenter, transform.forward * rayLength, Color.cyan);
        ////SendRayCast(forwardCenter, transform.forward, rayLength, "forward");

        ////Back side center
        //Vector3 backwardsCenter = center - transform.forward * halfWidth;
        //Debug.DrawRay(backwardsCenter, -transform.forward * rayLength, Color.black);
        //SendRayCast(backwardsCenter, -transform.forward, rayLength, "backwards");
    }

    void SendRayCast(Vector3 origin, Vector3 direction, float length, string invokerName)
    {
        if (Physics.Raycast(origin, direction, length))
        {
            Debug.Log($"Hit Something {invokerName}");
        }
        else
        {
            Debug.Log($"REPLICATE in direction: {direction.ToString()} on {invokerName}");
            MapManager.Instance.TryRenderNewTile(gameObject.transform.position + direction);
        }
    }
}
