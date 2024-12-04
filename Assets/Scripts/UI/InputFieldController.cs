using TMPro;
using UnityEngine;

public class InputFieldController : MonoBehaviour
{
    // Wenn was eingetragen wird, suche in der MapDataList
    [SerializeField]
    public TMP_InputField inputField;

    [SerializeField]
    public TMP_Text text;

    [SerializeField]
    public GameObject textGameObject;
            
    private void Start()
    {
        //inputField.onValueChanged.AddListener(FindNameInMapData);
    }
    //public void FindNameInMapData(string name)
    //{
    //    bool foundLocation = false;
    //    textGameObject.gameObject.SetActive(false);
    //    Debug.Log("On value changed");
    //    foreach (var item in DBManager.Instance.mapDataList)
    //    {
    //        if(name == item.locationName)
    //        {
    //            // display error set can save to falsebm.
    //            Debug.Log($"inpufield: {name}, mapdatalist: {item.locationName}");
    //            text.text = "Location with Name " + item.locationName + " exists";
    //            textGameObject.gameObject.SetActive(true);
    //            foundLocation = true;
    //            break;
    //        }
    //    }
    //    if (!foundLocation)
    //    {
    //        textGameObject.gameObject.SetActive(false);
    //    }
    //}
}
