using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    private UIDocument _mainDoc;
    private UISTATE _state;
    public enum UISTATE
    {
        main,
        scan,
        navigate
    }
    // layout elements
    private VisualElement  _mainContainer;
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

    // Navigation 
    [SerializeField]
    private VisualTreeAsset _navigationTemplate;
    private VisualElement _navigationElement;
    private VisualElement _navigationTopContainer;
    private VisualElement _navigationMiddleContainer;
    
    private void Awake()
    {
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
                Debug.Log($"back clicked");
                Debug.Log($"uiState: {this._state}");
                _backButton.visible = false;
                _navigateButton.visible = true;
                _scanButton.visible = true;
                _mainContainer.Add(_mainTopContainer);
                _mainContainer.Add(_mainMiddleContainer);
                _mainContainer.Add(_mainBottomContainer);
                break;
            case UISTATE.navigate:
                Debug.Log($"navigate clicked");
                Debug.Log($"uiState: {this._state}");
                _backButton.visible = true;
                _mainContainer.Add(_navigationTopContainer);
                _mainContainer.Add(_navigationMiddleContainer);
                _mainContainer.Add(_mainBottomContainer);
                _scanButton.visible = false;
                break;
            case UISTATE.scan:
                Debug.Log($"scan clicked");
                Debug.Log($"uiState: {this._state}");
                _backButton.visible = true;
                _mainContainer.Add(_scanTopContainer);
                _mainContainer.Add(_scanMiddleContainer);
                _mainContainer.Add(_mainBottomContainer);
                _navigateButton.visible = false;
                break;
            default:
                this._state = UISTATE.main;
                Debug.Log($"default");
                Debug.Log($"uiState: {this._state}");
                break;
        }
    }
}
