using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextVisibilityToggle : MonoBehaviour
{
    [SerializeField] TMP_InputField passwordIP;
    [SerializeField] Button eyebtn;
    [SerializeField] Image disableImage;

    public void ToggleVisibility()
    {
        if(passwordIP.contentType == TMP_InputField.ContentType.Standard)
        {
            passwordIP.contentType = TMP_InputField.ContentType.Password;
            disableImage.gameObject.SetActive(true);
        }
        else if(passwordIP.contentType == TMP_InputField.ContentType.Password)
        {
            passwordIP.contentType = TMP_InputField.ContentType.Standard;
            disableImage.gameObject.SetActive(false);
        }

        passwordIP.ForceLabelUpdate();
    }
}
