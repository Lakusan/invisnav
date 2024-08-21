using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public float longitude { get; set; }
    public float latitude { get; set; }

    public string name { get; set; }  
    public List<Anchor> anchors { get; set; }

    public List<Mesh> meshes { get; set; }

}
