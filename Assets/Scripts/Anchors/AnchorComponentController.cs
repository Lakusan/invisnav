using UnityEngine;
using UnityEngine.AI;

public class AnchorComponentController : MonoBehaviour
{
    private bool isValidatedAnchor = false;
    [SerializeField]
    public Material validAnchorMaterial;
    [SerializeField]
    public bool done = false;
    private MeshRenderer renderer;
    private bool isOnMapComponent = false;
    private bool isOnNavMesh = false;
    private ANCHOR_STATE state;
    public string currentState;


    // anchor data 
    [SerializeField]
    public string anchorName;
    [SerializeField]
    public string anchorDescription;
    [SerializeField]
    public string attachedMapComponentName;
    [SerializeField]
    public GameObject attachedMapComponent;
    private enum ANCHOR_STATE
    {
    analyzing,
    isOnMap,
    isOnNavMesh,
    isReachable,
    validation,
    isValidated
    };

    private Anchor anchorData;

    void Start()
    {
        this.anchorName = null;
        this.anchorDescription = null;
        this.attachedMapComponentName = null;
        this.renderer = gameObject.GetComponent<MeshRenderer>();
        this.state = ANCHOR_STATE.analyzing;
        this.currentState = this.state.ToString();
        this.anchorData = new Anchor()
        {
            anchorName = "",
            anchorDescription = "",
            attachedMapComponentName = "",
            posX = 0f,
            posY = 0f,
            posZ = 0f,
        };
    }

    void Update()
    {
        currentState = this.state.ToString();
        if (!done)
        {
            switch (this.state)
            {
                case ANCHOR_STATE.analyzing:
                    TryToFindMapComponent();
                    break;
                case ANCHOR_STATE.isOnMap:
                    IsAnchorOnNavMesh();
                    break;
                case ANCHOR_STATE.isOnNavMesh:
                    IsReachable();
                    break;
                case ANCHOR_STATE.isReachable:
                    // add anchor to anchorlist
                    //if (this.anchorDescription != null && this.anchorName != null && this.attachedMapComponentName != null)
                    //{
                    //    Debug.Log($"anchorName: {this.anchorName}");
                    //    Debug.Log($"anchorDescription: {this.anchorDescription}");
                    //    Debug.Log($"attachedMapComponent: {this.attachedMapComponentName}");

                    //} else { Debug.Log("NULL"); }
                    this.state = ANCHOR_STATE.validation;
                    break;
                case ANCHOR_STATE.validation:
                    ValidateAnchor();
                    break;
                case ANCHOR_STATE.isValidated:
                    AddAnchorToLocation(this.anchorData);
                    this.done = true;
                    break;
                default:
                    break;
            }
        }
    }

    private void TryToFindMapComponent()
    {
        Vector3 bottomCenter = transform.position - Vector3.up * (transform.localScale.y / 2f);
        Ray ray = new Ray(bottomCenter, Vector3.down);
        Debug.DrawRay(ray.origin, ray.direction * 10, Color.green);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.layer == 8)
            {
                attachedMapComponentName = hit.collider.gameObject.name;
                attachedMapComponent = hit.collider.gameObject;
                SetAnchorPosition();
                this.state = ANCHOR_STATE.isOnMap;
            }
        }
        else
        {
            MoveAnchorUp();
        }
    }

    private void SetAnchorPosition()
    {
        Vector3 posVector = gameObject.transform.position;
        this.anchorData.posX = posVector.x;
        this.anchorData.posY = posVector.y;
        this.anchorData.posZ = posVector.z;
    }

    private void MoveAnchorUp()
    {
        gameObject.transform.position += Vector3.up/100;
    }

    private void ValidateAnchor()
    {
        if (this.anchorDescription != null && this.anchorName != null && this.attachedMapComponentName != null)
        {
            this.anchorData.anchorDescription = this.anchorDescription;
            this.anchorData.anchorName = this.anchorName;
            this.anchorData.attachedMapComponentName = this.attachedMapComponentName;
            MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
            renderer.material = validAnchorMaterial;
            isValidatedAnchor = true;
            this.state = ANCHOR_STATE.isValidated;
        }
    }

    public void IsAnchorOnNavMesh()
    {
        Vector3 agentPosition = gameObject.transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(agentPosition, out hit, 1, NavMesh.AllAreas))
        {
            this.state = ANCHOR_STATE.isOnNavMesh;
        }
    }

    private void IsReachable()
    {
        // check Calculate Path from last tracker position on navmesh to service anchor
        if (CalculatePath())
        {
            this.state = ANCHOR_STATE.isReachable;
        };
    }

    public bool CalculatePath()
    {
        NavMeshPath path = new NavMeshPath();
        Vector3 origin = MapManager.Instance.LastTackerPositionOnNavMesh;
        NavMesh.CalculatePath(origin, this.gameObject.transform.position, NavMesh.AllAreas, path);
        if (path != null)
        {
            for (int i = 0; i < path.corners.Length; i++)
            {
                Debug.DrawLine(origin, path.corners[i], Color.red);
            }
            return true;
        }
         return false;
    }

    private void AddAnchorToLocation(Anchor anchor)
    {
        if (isValidatedAnchor)
        {
            MapManager.Instance.AddAnchor(anchor);
        }
    }

    public void SetAnchorName(string newName)
    {
        this.gameObject.name = newName;
        this.anchorName = newName;
    }

    public void SetAnchorDescription(string description)
    {
        this.anchorDescription = description;
    }

}
