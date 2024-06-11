using System.Collections.Generic;
using UnityEngine;


// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class MapRoot
{
    public LocationNameConversion locationName { get; set; }
}

public class LocationNameConversion
{
    public double longitude { get; set; }
    public double latitude { get; set; }
    public StartAnchorConversion startAnchor { get; set; }
    public List<Mesh> meshes { get; set; }
}
public class StartAnchorConversion
{
    public double longitude { get; set; }
    public double latitude { get; set; }

    public string text { get; set; }
}

public class MeshConversion
{
    public List<Vertex> vertices { get; set; }
    public List<int> triangles { get; set; }
}

public class VertexConversion
{
    public int x { get; set; }
    public int y { get; set; }
    public int z { get; set; }
}


public class Map : MonoBehaviour
{
    public string locationName { get; set; }
    public double longitude { get; set; }
    public double latitude { get; set; }
    List<Mesh> newMeshes { get; set; }
    List<Mesh> mapMeshes { get; set; }

    

}
