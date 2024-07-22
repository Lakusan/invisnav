using UnityEngine;
using UnityEngine.UI;

public class AnchorManager : MonoBehaviour
{
    [SerializeField]
    public GameObject anchorPrefab;

    [SerializeField]
    public Button createStartAnchorButton;

    public void CreateStartAnchor()
    {
        Instantiate(anchorPrefab,Camera.main.transform.position, Quaternion.identity);
    }

    public void OnCreateStartAnchorButtonClicked()
    {
        CreateStartAnchor();
        createStartAnchorButton.enabled = false;
    }
}
