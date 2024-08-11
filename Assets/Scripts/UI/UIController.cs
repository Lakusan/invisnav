using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    public UIDocument _mainDoc;
    public static UIController Instance { get; private set; }

    public UISTATE _state;
    public enum UISTATE
    {
        main,
        scan,
        navigate
    }
    // ARMeshing
    GameObject _arMeshing;

    // anchormanager 
    [SerializeField] private GameObject _anchorManager;
    // navigation navbar
    [SerializeField] private GameObject _navManager;
    // mapscanner 
    [SerializeField] private GameObject _mapScanner;
    // runtime navbar reference
    private GameObject _navBar;
    // layout elements
    private VisualElement _mainContainer;
    private VisualElement _mainTopContainer;
    private VisualElement _mainMiddleContainer;
    private VisualElement _mainBottomContainer;

    // Main Menu elements
    private Button _scanButton;
    private Button _navigateButton;
    private Button _backButton;

    [Header("UI Elements")]
    // Scan 
    [SerializeField]
    private VisualTreeAsset _scanTemplate;
    private VisualElement _scanElement;
    private VisualElement _scanTopContainer;
    private VisualElement _scanMiddleContainer;
    private VisualElement _createLocationButton;
    private TextField _locationNameInputField;

    // Navigation 
    [SerializeField]
    private VisualTreeAsset _navigationTemplate;
    private VisualElement _navigationElement;
    private VisualElement _navigationTopContainer;
    private VisualElement _navigationMiddleContainer;
    private RadioButtonGroup _radioButtonGroup;
    private Button _navigationStartButton;

    // Data
    private RegisteredLocations _registeredLocations;
    private string _scanLocationName;
    private string _navigationLocationName;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        _mainDoc = GetComponent<UIDocument>();
        _mainContainer = _mainDoc.rootVisualElement.Q<VisualElement>("MainContainer");
        _mainTopContainer = _mainContainer.Q<VisualElement>("TopContainer");
        _mainMiddleContainer = _mainContainer.Q<VisualElement>("MiddleContainer");
        _mainBottomContainer = _mainContainer.Q<VisualElement>("BottomContainer");

        //scan element
        _scanElement = _scanTemplate.CloneTree();
        _scanTopContainer = _scanElement.Q<VisualElement>("TopContainer");
        _scanMiddleContainer = _scanElement.Q<VisualElement>("MiddleContainer");

        //navigation element
        _navigationElement = _navigationTemplate.CloneTree();
        _navigationTopContainer = _navigationElement.Q<VisualElement>("TopContainer");
        _navigationMiddleContainer = _navigationElement.Q<VisualElement>("MiddleContainer");
        _navigationStartButton = _navigationMiddleContainer.Q<Button>("StartNavigationButton");

        // get buttons + Events
        _backButton = _mainDoc.rootVisualElement.Q<Button>("BackButton");
        _backButton.clicked += BackButtonOnClicked;
        _scanButton = _mainDoc.rootVisualElement.Q<Button>("ScanningButton");
        _navigateButton = _mainDoc.rootVisualElement.Q<Button>("NavigateButton");
        _scanButton.clicked += ScanButtonOnClicked;
        _navigateButton.clicked += NavigateButtonOnClicked;

        // Get Navbar reference
        try
        {
            GameObject myObject = GameObject.Find("NavBar");
            if (myObject != null)
            {
                _navBar = myObject;
            }
            else
            {
                Debug.LogError("NavBar not found!");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error finding object: {e.Message}");
        }
    }

    private void OnDisable()
    {
        _scanButton.clicked -= ScanButtonOnClicked;
        _navigateButton.clicked -= NavigateButtonOnClicked;
        _backButton.clicked -= BackButtonOnClicked;
    }

    void Start()
    {
        this._state = UISTATE.main;
        _backButton.visible = false;
        _scanLocationName = null;
        _navigationLocationName = null;
    }

    private void NavigateButtonOnClicked()
    {
        this._state = UISTATE.navigate;
        ChangeViewState(this._state);
    }

    private void ScanButtonOnClicked()
    {
        this._state = UISTATE.scan;
        ChangeViewState(this._state);

    }

    private void BackButtonOnClicked()
    {
        this._state = UISTATE.main;
        ChangeViewState(this._state);
    }

    void ChangeViewState(UISTATE uiState)
    {
        _mainContainer.Clear();
        switch (uiState)
        {
            case UISTATE.main:
                _backButton.visible = false;
                _navigateButton.visible = true;
                _scanButton.visible = true;
                _mainContainer.Add(_mainTopContainer);
                _mainContainer.Add(_mainMiddleContainer);
                _mainContainer.Add(_mainBottomContainer);
                break;
            case UISTATE.navigate:
                _registeredLocations = DBManager.Instance.GetRegisteredLocations();
                _radioButtonGroup = _navigationElement.Q<RadioButtonGroup>("RadioButtonGroup");
                _radioButtonGroup.choices = _registeredLocations.locations;
                _navigationStartButton.RegisterCallback<ClickEvent>(OnStartNavigationButtonClicked);
                _radioButtonGroup.RegisterValueChangedCallback(v =>
                {
                    _navigationLocationName = _registeredLocations.locations[v.newValue];
                });
                _backButton.visible = true;
                _scanButton.visible = false;
                _navigateButton.visible = false;
                _mainContainer.Add(_navigationTopContainer);
                _mainContainer.Add(_navigationMiddleContainer);
                _mainContainer.Add(_mainBottomContainer);
                break;
            case UISTATE.scan:
                _backButton.visible = true;
                _mainContainer.Add(_scanTopContainer);
                _mainContainer.Add(_scanMiddleContainer);
                _mainContainer.Add(_mainBottomContainer);
                _navigateButton.visible = false;
                _scanButton.visible = false;
                _createLocationButton = _scanMiddleContainer.Q<Button>("CreateLocationButton");
                _createLocationButton.RegisterCallback<ClickEvent>(OnCreateLocationButtonClicked);
     
                break;
            default:
                this._state = UISTATE.main;
                break;
        }
    }

    private void OnStartNavigationButtonClicked(ClickEvent evt)
    {
        if (_navigationLocationName != null) 
        {
            Debug.Log($"Start Navigation with {_navigationLocationName}");
            //load Map
            DBManager.Instance.GetLocationByName(_navigationLocationName);
            InitNavigation(); 
        }
    }

    private void OnCreateLocationButtonClicked(ClickEvent evt)
    {
        _registeredLocations = DBManager.Instance.GetRegisteredLocations();
        _locationNameInputField = _scanMiddleContainer.Q<TextField>("LocationInput");
        Debug.Log($"CREATE LOCATION: {_locationNameInputField.value}");
        // Check if Location exists
        string locationName = _locationNameInputField.value;
        Label textContent = _scanMiddleContainer.Q<Label>("TextContent");

        if (_registeredLocations.locations.Contains(locationName))
        {
            Debug.Log($"Location: {locationName} exists");
            textContent.text = "Location " + locationName + " exists, try another name";
        } else {
            // get created Location
            _scanLocationName = _locationNameInputField.text;
            MapManager.Instance.currentLocation = _scanLocationName;
            Debug.Log($"CREATED LOCATION: {_scanLocationName}");
            InitScanning();
        }
    }

    private void InitScanning()
    {
        if (this._state == UISTATE.scan)
        {
            //set gps coords
            List<float> gpsCoords = LocationManager.Instance.GetGPSCoords();
#if UNITY_EDITOR
            MapManager.Instance.latitudeOrigin = 0.0f;
            MapManager.Instance.longitudeOrigin = 0.0f;
#else
            MapManager.Instance.latitudeOrigin = gpsCoords[0];
            MapManager.Instance.longitudeOrigin = gpsCoords[1];
#endif
            // hide current loaded UI elements
            _mainDoc.rootVisualElement.style.display = DisplayStyle.None; 
        }
        // enable AR Session
        MapManager.Instance.toggleMeshing();
    }

    private void InitNavigation()
    {
#if UNITY_EDITOR
#else
        // Get Map GPS Cords which were saved with the map
        // get User GPS Cords
        // Align Loaded Map with GPS of start Postion
#endif
        // deactivate Modules 
        // anchormanager off
        _anchorManager.SetActive(false);
        // navigation UI on
        _navManager.SetActive(true);
        // deactivate MapScanner
        _mapScanner.SetActive(false);
        // hide current loaded UI Elements
        _mainDoc.rootVisualElement.style.display = DisplayStyle.None;
        NavMeshController.Instance.BakeNavMesh();
    }
}
