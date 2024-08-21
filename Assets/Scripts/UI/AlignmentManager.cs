using TMPro;
using UnityEngine;

public class AlignmentManager : MonoBehaviour
{
    public static AlignmentManager Instance { get; private set; }

    public float confirmedHeading { get; private set; }
    public float confirmedLatitude { get; private set; }
    public float confirmedLongitude { get; private set; }
    [SerializeField] private GameObject UI;


    [SerializeField] private GameObject scanPanel;
    [SerializeField] private GameObject scanPanelLat;
    [SerializeField] private GameObject scanPanelLon;
    [SerializeField] private GameObject scanPanelActualHeading;
    [SerializeField] private GameObject Confirm_ScanHeading_Button;
    [SerializeField] private GameObject uICamera;
    [SerializeField] private GameObject anchorManager;
    [SerializeField] private GameObject xROrigin;
    [SerializeField] private GameObject aRSession;
    [SerializeField] private GameObject navBar;
    private TMP_Text scanPanelLatText;
    private TMP_Text scanPanelLonText;
    private TMP_Text scanPanelActualHeadingText;
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

    [SerializeField] private GameObject navigationPanelLat;
    [SerializeField] private GameObject navigationPanelLon;
    [SerializeField] private GameObject navigationPanelActualHeading;
    [SerializeField] private GameObject navigationPanelSavedHeading;
    [SerializeField] private GameObject navigationPanel;
    [SerializeField] private GameObject Confirm_NAVHeading_Button;
    private TMP_Text navigationPanelLatText;
    private TMP_Text navigationPanelLonText;
    private TMP_Text navigationPanelSavedHeadingText;
    private TMP_Text navigationPanelActualHeadingText;

    private bool done = false;

    void Start()
    {
        scanPanelLatText = scanPanelLat.GetComponent<TMP_Text>();
        scanPanelLonText = scanPanelLon.GetComponent<TMP_Text>();
        scanPanelActualHeadingText = scanPanelActualHeading.GetComponent<TMP_Text>();

        navigationPanelLatText = navigationPanelLat.GetComponent<TMP_Text>();
        navigationPanelLonText = navigationPanelLon.GetComponent<TMP_Text>();
        navigationPanelSavedHeadingText = navigationPanelSavedHeading.GetComponent<TMP_Text>();
        navigationPanelActualHeadingText = navigationPanelActualHeading.GetComponent<TMP_Text>();
    }

    void Update()
    {
        if (!done)
        {
            if (LocationManager.Instance.isLocationServicesRunning)
            {
                switch (UIController.Instance._state)
                {
                    case UIController.UISTATE.scan:
                        if (!scanPanel.activeSelf && !done)
                        {
                            scanPanel.SetActive(true);
                        }
                        scanPanelLatText.text = Input.location.lastData.latitude.ToString();
                        scanPanelLonText.text = Input.location.lastData.longitude.ToString();
                        scanPanelActualHeadingText.text = Input.compass.trueHeading.ToString();

                        break;
                    case UIController.UISTATE.navigate:
                        if (!navigationPanel.activeSelf && !done)
                        {
                            navigationPanel.SetActive(true);
                        }
                        navigationPanelLatText.text = Input.location.lastData.latitude.ToString();
                        navigationPanelLonText.text = Input.location.lastData.longitude.ToString();
                        navigationPanelActualHeadingText.text = Input.compass.trueHeading.ToString();
                        //navigationPanelSavedHeadingText.text = MapManager.Instance.;
                        break;
                }
            }
        }

    }

    public void OnConfirmScanHeadingButtonPressed()
    {
        done = true;
        confirmedHeading = Input.compass.trueHeading;
        confirmedLatitude = Input.location.lastData.latitude;
        confirmedLongitude = Input.location.lastData.longitude;
        // Data to MapManger
        MapManager.trueHeading = confirmedHeading;
        MapManager.latitudeOrigin = confirmedLatitude;
        MapManager.longitudeOrigin = confirmedLongitude;
        // activate AnchorManager'
        anchorManager.SetActive(true);
        // deactivate UI-Camera
        uICamera.SetActive(false);
        // activate Navbar and debug
        navBar.SetActive(true);
        // deactivate scan panel 
        scanPanel.SetActive(false);
        // Activate XROrigin
        xROrigin.SetActive(true);
        // AR Session
        aRSession.SetActive(true);
        // activate Meshing
        MapManager.Instance.ActivateMeshing();
        LocationManager.Instance.StopLocationServices();
    }
    public void OnConfirmNAVHeadingButtonPressed()
    {
        done = true;

        // gyroskopkamera von youtube. 
        // dann vincenty distanz und azimuth 
        // wenn user dann am Punkt -> rotation true HEading
        // dann erst map laden wenn XR Orgigin  + session geladen

    }
}
