using TMPro;
using UnityEngine;

public class NavAnchorController : MonoBehaviour
{
    // anchor data 
    [SerializeField]
    public string anchorName;
    [SerializeField]
    public string anchorDescription;

    [SerializeField]
    private TextMeshPro NameTMP; 
    [SerializeField]
    private TextMeshPro DescriptionTMP;
    public void SetAnchorNameToTMP(string name)
    {
        this.anchorName = name;
        this.NameTMP.text = name;
    }
    public void SetAnchorDescriptionToTMP(string description)
    {
        this.anchorDescription = description;
        this.DescriptionTMP.text = description;
    }
}