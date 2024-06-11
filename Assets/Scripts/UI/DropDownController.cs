using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DropDownController : MonoBehaviour
{
    TMP_Dropdown dropdown;
    public int frame;
    void Start()
    {
        // clear Drop down
        dropdown = transform.GetComponent<TMP_Dropdown>();
        dropdown.ClearOptions();
        StartCoroutine(WaitForDataFromFirebase());
    }

    IEnumerator WaitForDataFromFirebase()
    {
        //Debug.Log("Waiting for Firebase To Connect");
        yield return new WaitUntil(() => frame >= 10);
        //Debug.Log("Firebase is now ready!");
        populateDropDownWithMapData();
    }
    void Update()
    {
        if (frame <= 10)
        {
            //Debug.Log("Frame: " + frame);
            frame++;
        }
    }
    void populateDropDownWithMapData()
    {
        List<TMP_Dropdown.OptionData> optionsDataList = new List<TMP_Dropdown.OptionData>();
        //List<MapData> mapDataList = DBManager.Instance.GetMapDataList();
        //foreach (var mapDataItem in mapDataList)
        //{
        //    Debug.Log($"add {mapDataItem.id}");
        //    optionsDataList.Add(new TMP_Dropdown.OptionData(mapDataItem.locationName, null));
        //}
        //dropdown.options = optionsDataList;
        //dropdown.RefreshShownValue();
    }
}
