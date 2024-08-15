using UnityEngine;

public class MapScanner : MonoBehaviour
{

    public float moveDistance = 5f;
    private bool movingRight = true;

    void Update()
    {
        if (movingRight)
        {
            transform.Translate(Vector3.right * moveDistance * Time.deltaTime, Space.Self);
            if (transform.position.x >= moveDistance)
            {
                movingRight = false;
            }
        }
        else
        {
            transform.Translate(Vector3.left * moveDistance * Time.deltaTime, Space.Self);
            if (transform.position.x <= -moveDistance)
            {
                movingRight = true;
            }
        }
    }

}

