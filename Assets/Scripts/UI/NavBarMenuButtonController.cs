using UnityEngine;
using UnityEngine.UIElements;

public class NavBarMenuButtonController : MonoBehaviour
{
    UIDocument _mainDoc;
    GameObject _UI;

    void Start()
    {
        _UI = GameObject.Find("/UI");
        _mainDoc = _UI.GetComponent<UIDocument>();
    }

    public void OnMenuBtnPressed()
    {
        // Disable Meshing
        MapManager.Instance.toggleMeshing();
        // Show last UI state
        _mainDoc.rootVisualElement.style.display = DisplayStyle.Flex;
    }

    /*
     * - create location in DB and upload empty new location data object
     * - - get location data -> method / get location data and update
     * - get gps coodinates and if anchor placed, start meshing
     * - - display information to user 
     * ---> Service Anchor + text 
     * --> Navigation sequence -> Activate Navigation if user is in segment use map to find place
     * ----- > How can I save a navmesh or use AR Navigation in unity engine
     */

}
