using UnityEngine;

public class MapScanner : MonoBehaviour
{

    public float moveDistance;
    private bool movingRight = true;

    void Update()
    {
        if (movingRight)
        {
            transform.Translate(Vector3.right * moveDistance * Time.deltaTime, Space.Self);
            if (transform.localPosition.x >= moveDistance)
            {
                movingRight = false;
            }
        }
        else
        {
            transform.Translate(Vector3.left * moveDistance * Time.deltaTime, Space.Self);
            if (transform.localPosition.x <= -moveDistance)
            {
                movingRight = true;
            }
        }
    }
}

