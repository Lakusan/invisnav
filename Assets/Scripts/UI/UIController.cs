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
    [SerializeField] private GameObject _navBar;
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
    private float initTrueHeading;
    private float initLat;
    private float initLon;

    // get initHeading Scan hint elements
    [SerializeField] public GameObject alignmentManager;
    [SerializeField] public GameObject panelScan;
    [SerializeField] public GameObject panelNav;


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
            //load Map
            DBManager.Instance.GetLocationByName(_navigationLocationName);
            InitNavigation(); 
        }
    }

    private void OnCreateLocationButtonClicked(ClickEvent evt)
    {
        _registeredLocations = DBManager.Instance.GetRegisteredLocations();
        _locationNameInputField = _scanMiddleContainer.Q<TextField>("LocationInput");
        // Check if Location exists
        string locationName = _locationNameInputField.value;
        Label textContent = _scanMiddleContainer.Q<Label>("TextContent");

        if (_registeredLocations.locations.Contains(locationName))
        {
            textContent.text = "Location " + locationName + " exists, try another name";
        } else {
            // get created Location
            _scanLocationName = _locationNameInputField.text;
            MapManager.Instance.currentLocation = _scanLocationName;
            InitScanning();
        }
    }

    private void InitScanning()
    {
        if (this._state == UISTATE.scan)
        {
            alignmentManager.SetActive(true);
            // hide current loaded UI elements
            _mainDoc.rootVisualElement.style.display = DisplayStyle.None; 
        }
    }

    private void InitNavigation()
    {
        // deactivate MapScanner
        _mapScanner.SetActive(false);
        // alignment Manager active
        alignmentManager.SetActive(true);

        _mainDoc.rootVisualElement.style.display = DisplayStyle.None;
    }
}
