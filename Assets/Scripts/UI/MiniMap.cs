using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;


public class MiniMap : MonoBehaviour
{

    void Start()
    {
        
    }

    void Update()
    {
            transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y + 30, Camera.main.transform.position.z);
            transform.rotation = Quaternion.Euler(90f, Camera.main.transform.rotation.eulerAngles.y, 0);
    }
}
