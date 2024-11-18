using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TabNavigation : MonoBehaviour
{
    [SerializeField] TMP_InputField[] inputFields; 
    private int currentFieldIndex = 0;
    [SerializeField] Button submitButton;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            currentFieldIndex++;
            if (currentFieldIndex >= inputFields.Length)
            {
                currentFieldIndex = 0;
            }
            inputFields[currentFieldIndex].Select();
        }
        else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            // If Enter is pressed and we're on the last input field, click the button
            if (currentFieldIndex == inputFields.Length - 1)
            {
                submitButton.onClick.Invoke();
            }
        }
    }

}
