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
        dropdown = transform.GetComponent<TMP_Dropdown>();
        dropdown.ClearOptions();
        StartCoroutine(WaitForDataFromFirebase());
    }

    IEnumerator WaitForDataFromFirebase()
    {
        yield return new WaitUntil(() => frame >= 10);
        populateDropDownWithMapData();
    }
    void Update()
    {
        if (frame <= 10)
        {
            frame++;
        }
    }
    void populateDropDownWithMapData()
    {
        List<TMP_Dropdown.OptionData> optionsDataList = new List<TMP_Dropdown.OptionData>();
    }
}
