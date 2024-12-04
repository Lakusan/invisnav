using Niantic.Lightship.AR.Protobuf;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Hints : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    public static Hints Instance { get; private set; }
    private string hintText;
    private Dictionary<string, string> textTemplates;
    [SerializeField] private TMP_Text tmp_textField;
    public enum HINT_STATE
    {
        DISABLED,
        SCAN_StartHint,
        SCAN_SaveMapHint,
        SCAN_MapSaved,
        NAV_StartHint,
        Nav_MapLoadHint
    }
    private HINT_STATE state;
    public string currentHintState;
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
    }
    private void Start()
    {
        textTemplates = new Dictionary<string, string>()
        {
            { "DISABLED", "State: DISABLED" },
            { "SCAN_StartHint" , "The Scanning process runs automatically. \nThe green surface marks the navigateable area which was already registered by the System. \nTo mark POIs create Anchors on the greena area and save the scan to allow other to access the data." },
            { "SCAN_SaveMapHint" , "Now your scanned data gets validated and uploaded to the server. \nPlease wait until this is done, to make sure the map gets uploaded correcly."},
            { "SCAN_MapSaved", "Map has been saved. \n\nPlease Restart the App." },
            { "NAV_StartHint" , "Currently the map get loaded. \n Please be patient, this could take a minute. \n"},
            { "Nav_MapLoadHint", "Map laaded successfully. \n Select a POI to get directions." }
        };
        state = HINT_STATE.DISABLED;
        hintText = textTemplates["DISABLED"];
    }


    private void Update()
    {
        currentHintState = state.ToString();
        switch (state)
        {
            case HINT_STATE.DISABLED:
                if (panel.activeSelf)
                {
                    HideHint();
                }
                break;
            case HINT_STATE.SCAN_StartHint:
                SetHintText();
                ShowHint();
                break;
            case HINT_STATE.SCAN_SaveMapHint:
                break;
            case HINT_STATE.SCAN_MapSaved:
                break;
            case HINT_STATE.NAV_StartHint:
                break;
            case HINT_STATE.Nav_MapLoadHint:
                break;
        }
    }
    private void SetHintText()
    {
        hintText = textTemplates[currentHintState];
        tmp_textField.text = hintText;
    }

    public void ShowHint()
    {
        panel.SetActive(true);
    }

    public void HideHint()
    {
        state = HINT_STATE.DISABLED;
        panel.SetActive(false); 
    }

    public void TransitionToState(HINT_STATE newState)
    {
        state = newState;
        currentHintState = newState.ToString();
    }
}
