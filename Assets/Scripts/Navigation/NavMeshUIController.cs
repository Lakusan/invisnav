using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NavMeshUIController : MonoBehaviour
{
    [SerializeField] private GameObject DropDown;
    [SerializeField] private GameObject ConfirmationButton;
    [SerializeField] public GameObject Panel;
    [SerializeField] private GameObject navManager;
    [SerializeField] private GameObject anchorSelectionButton;
    public static NavMeshUIController Instance { get; private set; }


    private string currentAnchorName = null;
    private GameObject currentAnchor = null;

    private TMP_Dropdown dropdown;

    [SerializeField] private bool populate = true;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
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

    private bool PopulateDropDown()
    {
        dropdown.ClearOptions();
        List<string> data = new List<string>();
        if (MapManager.anchorList.Count > 0)
        {
            foreach (Anchor anchor in MapManager.anchorList)
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
        Debug.Log("Dropdown Populate false");
        return false;
    }

    void OnDropdownValueChanged(int index)
    {
        currentAnchorName = dropdown.options[index].text;
    }

    public void OnConfirmationButtonPressed()
    {
        Panel.SetActive(false);
        anchorSelectionButton.SetActive(true);
        currentAnchor = MapManager.Instance.FindAnchorGO(currentAnchorName);
        TrackerController.Instance.SetTarget(currentAnchor);
    }

    public void OnAnchorSelectionButtonPressed()
    {
        anchorSelectionButton.SetActive(false);
        Panel.SetActive(true);
    }
}
