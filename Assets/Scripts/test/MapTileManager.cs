using System.Collections.Generic;
using UnityEngine;

public class MapTileManager : MonoBehaviour
{
    public static Dictionary<string, string> tiles;
    [SerializeField] public GameObject prefab;
    [SerializeField] public GameObject mapContainer;
    public static MapTileManager Instance { get; private set; }

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
    public void TryRenderNewTile(Vector3 position)
    {
        if (!tiles.ContainsKey(position.ToString()))
        {
            GameObject newTile = Instantiate(prefab, position,Quaternion.identity);
            //GameObject newTile = new ;
            //MeshFilter mf = newTile.AddComponent<MeshFilter>();
            //mf.mesh = gameObject.GetComponent<MeshFilter>().mesh;
            //newTile.transform.position = gameObject.transform.position + direction;
            //newTile.transform.rotation = gameObject.transform.rotation;
            //newTile.transform.position = direction;
            newTile.SetActive(false);
            newTile.transform.parent = mapContainer.transform;
            newTile.SetActive(true);
            tiles.Add(position.ToString(), position.ToString());
        }
    }
}
