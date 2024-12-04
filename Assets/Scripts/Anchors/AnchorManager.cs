using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;
using TMPro;

public class AnchorManager : MonoBehaviour
{
    private int layerNumber = 10;
    private int layerMask;
    [SerializeField]
    public GameObject anchorPrefab;

    [SerializeField]
    public Button createAnchorButton;

    [SerializeField]
    public Button ConfirmAnchorData;
    [SerializeField]
    public GameObject panel;
    public GameObject inputFieldAnchorNameGO;
    public GameObject inputFieldAnchroDescriptionGO;

    private TMP_InputField inputFieldAnchorName;
    private TMP_InputField inputFieldAnchorDescription;

    private List<ARRaycastHit> hitList = new List<ARRaycastHit>();
    private RaycastHit hit;

    private LineRenderer lineRenderer;

    private GameObject currentAnchor;
    public static AnchorManager Instance { get; private set; }


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

    private void Start()
    {
        layerMask = 1 << layerNumber;
        inputFieldAnchorName = inputFieldAnchorNameGO.GetComponent<TMP_InputField>();
        inputFieldAnchorDescription = inputFieldAnchroDescriptionGO.GetComponent<TMP_InputField>();
    }

    public void OnCreateAnchorButtonClicked()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.green);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.layer == 10)
            {
                currentAnchor = Instantiate(anchorPrefab, hit.point, hit.transform.rotation);
                createAnchorButton.gameObject.SetActive(false);
                panel.SetActive(true);
            }
        }
    }

    private void DrawScopingLine()
    {
        lineRenderer = GetComponent<LineRenderer>();
        Vector3 middleScreenPoint = new Vector3(0.5f, 0.5f, 0f);
        Vector3 middleWorldPoint = Camera.main.ScreenToWorldPoint(middleScreenPoint);
        lineRenderer.SetPosition(0, middleWorldPoint);
        lineRenderer.SetPosition(1, middleWorldPoint + Camera.main.transform.forward * 5f); // Extend the line forward
    }

    public void OnConfirmAnchorDataButtonClicked()
    {
        // get Data from input fields.
        if (inputFieldAnchorName.text != "" && inputFieldAnchorDescription.text != "")
        {
            // save anchor data
            currentAnchor.name = inputFieldAnchorName.text;
            AnchorComponentController anchorComponentController = currentAnchor.GetComponent<AnchorComponentController>();
            anchorComponentController.SetAnchorDescription(inputFieldAnchorDescription.text);
            anchorComponentController.SetAnchorName(inputFieldAnchorName.text);
            inputFieldAnchorName.text = "";
            inputFieldAnchorDescription.text = "";
            panel.SetActive(false);
            createAnchorButton.gameObject.SetActive(true);
        }
        else { Debug.Log("NULL"); }
    }
}
