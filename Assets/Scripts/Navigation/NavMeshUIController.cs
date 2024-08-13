using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NavMeshUIController : MonoBehaviour
{
    [SerializeField] private GameObject DropDown;
    [SerializeField] private GameObject ConfirmationButton;
    [SerializeField] private GameObject Panel;
    [SerializeField] private GameObject navManager;

    private string currentAnchorName = null;
    private GameObject currentAnchor = null;

    private TMP_Dropdown dropdown;

    private bool populate = true;
    void Start()
    {
        dropdown = DropDown.gameObject.GetComponent<TMP_Dropdown>();
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    void Update()
    {
        if (populate)
        {
            if (PopulateDropDown())
            {
                populate = false;
            }
        }
    }

    void Awake()
    {
    }

    private bool PopulateDropDown()
    {
        dropdown.ClearOptions();
        List<string> data = new List<string>();
        if (MapManager.Instance.anchorList.Count > 0)
        {
            foreach (Anchor anchor in MapManager.Instance.anchorList)
            {
                Debug.Log($"anchorlist : {anchor}");
                data.Add(anchor.anchorName);
            }
            if (data.Count >=1)
            {
                dropdown.AddOptions(data);
                ConfirmationButton.SetActive(true);
                currentAnchorName = dropdown.options[0].text;
                return true;
            }
        }

        return false;
    }

    void OnDropdownValueChanged(int index)
    {
        currentAnchorName = dropdown.options[index].text;
    }

    public void OnConfirmationButtonPressed()
    {
        // hide ui
        Panel.SetActive(false);
        // find anchor GO
        currentAnchor = MapManager.Instance.FindAnchorGO(currentAnchorName);
        // TODO if not found -> Error Message in text field

        // navigate to foudn Acnho
        NavMeshPathDrawer.Instance.NavigateToAnchor(currentAnchor);
        // if treckerisonnavmesh true -> draw path

    }
}
