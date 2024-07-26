using System.Collections.Generic;
using UnityEngine;

public class SerializableRoot : MonoBehaviour
{
    public SerializableLocation location { get; set; }
}
public class SerializableLocation
{
#nullable disable
    public double longitude { get; set; }
    public double latitude { get; set; }
#nullable disable
    public List<SerializableMesh> meshes { get; set; }
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

public class SerializableMap
{
   public SerializableRoot root;

   public SerializableMap()
    {
        root = new SerializableRoot()
        {
            location = new SerializableLocation() 
            {
                meshes = new List<SerializableMesh>()
            }
        };
    }
}
