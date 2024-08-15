using UnityEngine;
using Unity.AI.Navigation;
using System.Collections;

public class NavMeshController : MonoBehaviour
{
    public static NavMeshController Instance { get; private set; }
    
    [SerializeField] public NavMeshSurface navMeshSurface;
    [SerializeField] NavMeshController navMeshControllerScript;
    [SerializeField] NavMeshVisualizer navMeshVisualizerScript;

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
    IEnumerator BakeNavMeshAndWait()
    {
        while (true)
        {
            BakeNavMesh();
            yield return new WaitForSeconds(5.0f);
        }
    }

    IEnumerator WaitForMapTiles()
    {
        while (MapManager.Instance.tilesDict.Count < 10 || MapManager.Instance.tilesDict == null)
        {
            yield return new WaitForSeconds(.5f);
        }
        StartBaking();
    }

    void Start()
    {
        navMeshSurface = gameObject.GetComponent<NavMeshSurface>();
        navMeshControllerScript = gameObject.GetComponent<NavMeshController>();
        navMeshVisualizerScript = gameObject.GetComponent<NavMeshVisualizer>();
        StartCoroutine(WaitForMapTiles());
    }
    private void BakeNavMesh()
    {
        navMeshSurface.BuildNavMesh();
    }
    
    public void StartBaking()
    {
        navMeshSurface.enabled = true;
        navMeshVisualizerScript.enabled = true;
        StartCoroutine(BakeNavMeshAndWait());
    }

    public void StopBaking()
    {
        navMeshSurface.enabled = false;
        navMeshVisualizerScript.enabled = false;
        StopAllCoroutines();
    }
}
