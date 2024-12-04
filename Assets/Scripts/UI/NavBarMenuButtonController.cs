using UnityEngine;
using UnityEngine.UIElements;

public class NavBarMenuButtonController : MonoBehaviour
{
    UIDocument _mainDoc;
    GameObject _UI;

    [SerializeField]
    private GameObject _DebugCanvas;
    private Canvas _Canvas;

    void Start()
    {
        _UI = GameObject.Find("/UI");
        _mainDoc = _UI.GetComponent<UIDocument>();
        _Canvas = _DebugCanvas.GetComponent<Canvas>();
    }

    public void OnMenuBtnPressed()
    {
        // Disable Meshing
        MapManager.Instance.DeactivateMeshing();
        // Show last UI state
        _mainDoc.rootVisualElement.style.display = DisplayStyle.Flex;
    }

    public void OnDebugBtnPressed()
    {
        ToggleDebug();
    }

    private void ToggleDebug()
    {
        _Canvas.enabled = !_Canvas.enabled;
    }
}
