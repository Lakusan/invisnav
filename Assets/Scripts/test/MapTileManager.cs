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
            newTile.SetActive(false);
            newTile.transform.parent = mapContainer.transform;
            newTile.SetActive(true);
            tiles.Add(position.ToString(), position.ToString());
        }
    }
}
