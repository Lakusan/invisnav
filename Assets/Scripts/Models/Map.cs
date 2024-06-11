using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public double longitude { get; set; }
    public double latitude { get; set; }

    public string name { get; set; }  
    public List<Anchor> anchors { get; set; }

    public List<Mesh> meshes { get; set; }

}
