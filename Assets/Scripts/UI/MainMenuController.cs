using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public static MainMenuController instance { get; private set; }
    [SerializeField]
    public GameObject mainMenu;

    private void Awake()
    {
        if (MainMenuController.instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            MainMenuController.instance = this;
        }
    }

    private void Start()
    {

    }

    public void HandleAddLocationBtnPressed()
    {
        // disable Main Menu

    }

}
