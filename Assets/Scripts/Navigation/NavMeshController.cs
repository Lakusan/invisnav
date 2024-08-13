using UnityEngine;
using Unity.AI.Navigation;

public class NavMeshController : MonoBehaviour
{
    public static NavMeshController Instance { get; private set; }

    [SerializeField] public NavMeshSurface navMeshSurface;

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
    private void Start()
    {
        navMeshSurface = gameObject.GetComponent<NavMeshSurface>();
    }
    private void Update()
    {
        BakeNavMesh();

        //if (MapManager.meshDict.Count > 0)
        //{
        //    BakeNavMesh();
        //}
    }

    public void BakeNavMesh()
    {
        navMeshSurface.BuildNavMesh();
    }

}
