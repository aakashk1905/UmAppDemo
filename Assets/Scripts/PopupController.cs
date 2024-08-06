using UnityEngine;

public class PopupController : MonoBehaviour
{
    public GameObject loginPopup;
    public GameObject signUpPopup;

    void update()
    {
        loginPopup.SetActive(false);
        signUpPopup.SetActive(false);
    }
    public void ShowLoginPopup()
    {
        loginPopup.SetActive(true);
        signUpPopup.SetActive(false);
    }

    public void ShowSignUpPopup()
    {
        signUpPopup.SetActive(true);
        loginPopup.SetActive(false);
    }

    public void ClosePopups()
    {
        loginPopup.SetActive(false);
        signUpPopup.SetActive(false);
    }
}
