using UnityEngine;
using UnityEngine.UI;

public class MyConsole : MonoBehaviour
{
    public static MyConsole instance { get; private set; }

    [SerializeField] RectTransform displayRect;
    [SerializeField] Text MyConsoleText;
    private float initHeight;

    void Awake()
    {
        if (MyConsole.instance != null)
        {
            DestroyImmediate(gameObject);
        } else
        {
            MyConsole.instance = this;
        }

        initHeight = displayRect.anchoredPosition.y;
    }


    public void ChangeDisplayPosition(float newPos)
    {
        displayRect.anchoredPosition = new Vector2(displayRect.anchoredPosition.x, initHeight + newPos);
    }

    public void Log(string msg)
    {
        MyConsoleText.text = msg + "\n" + MyConsoleText.text;
    }
} 