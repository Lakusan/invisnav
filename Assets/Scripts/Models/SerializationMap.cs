using System.Collections.Generic;
using UnityEngine;

public class SerializableRoot : MonoBehaviour
{
    public SerializableLocation location { get; set; }
}
public class SerializableLocation
{
    public float longitude { get; set; }
    public float latitude { get; set; }
    public float trueHeading { get; set;  }
    public List<SerializableMesh> meshes { get; set; }
    public List<string> meshNames { get; set; }

    public List<SerializableAnchor> anchorList { get; set; }
}

public class SerializableMesh
{
    public List<SerializableVertex> vertices { get; set; }
    public List<int> triangles { get; set; }

    public SerializableMesh() 
    {
        vertices = new List<SerializableVertex>();
        triangles = new List<int>();
    }
}

public class SerializableVertex
{
    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }
}

public class SerializableAnchor
{
    public string anchorName { get; set; }
    public string anchorDescription { get; set; }
    public string attachedMapComponentName { get; set; }
    public float posX { get; set; }
    public float posY { get; set; }
    public float posZ { get; set; }
}

public class SerializableMap
{
   public SerializableRoot root;

   public SerializableMap()
    {
        root = new SerializableRoot()
        {
            location = new SerializableLocation()
            {
                meshes = new List<SerializableMesh>(),
                meshNames = new List<string>(),
                anchorList = new List<SerializableAnchor>()
            }
        };
    }
}
