using UnityEngine;
public class PopupController : MonoBehaviour
{
    public GameObject loginPopup;
    public GameObject registerPopup;
    public GameObject SuccesPanel;

    void Start()
    {
        loginPopup.SetActive(true);
        registerPopup.SetActive(false);
        SuccesPanel.SetActive(false);
    }
    public void ShowLoginPopup()
    {
        loginPopup.SetActive(true);
        registerPopup.SetActive(false);
        SuccesPanel.SetActive(false);
    }

    public void ShowRegisterPopup()
    {
        registerPopup.SetActive(true);
        loginPopup.SetActive(false);
        SuccesPanel.SetActive(false);
    }
    public void ShowSuccessPanel()
    {
        SuccesPanel.SetActive(true);
        loginPopup.SetActive(false);
    }
}
