using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;

public class AnchorManager : MonoBehaviour
{
    private int layerNumber = 7;
    private int layerMask;
    [SerializeField]
    public GameObject anchorPrefab;

    [SerializeField] public ARRaycastManager raycastManager;

    [SerializeField]
    public Button createStartAnchorButton;

    private List<ARRaycastHit> hitList = new List<ARRaycastHit>();
    private RaycastHit hit;

    private bool startAnchorIsPlaced = false;

    public LineRenderer lineRenderer;

    private void Start()
    {
        layerMask = 1 << layerNumber;
    }

    public void OnCreateStartAnchorButtonClicked()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.green);
        if (Physics.Raycast(ray, out hit))
        {
            Renderer renderer = hit.collider.gameObject.GetComponent<Renderer>();
            if (renderer != null) // Make sure the target has a Renderer component
            {
                Color randomColor = new Color(Random.value, Random.value, Random.value);
                renderer.material.color = randomColor; // Change the color to green
            }
            Instantiate(anchorPrefab, hit.point, hit.transform.rotation);
            createStartAnchorButton.gameObject.SetActive(false);
            startAnchorIsPlaced=true;
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


}
