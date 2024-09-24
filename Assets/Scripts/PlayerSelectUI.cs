using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelectUI : MonoBehaviour
{
    [SerializeField] private GameObject characterSelectionCanvas;
    [SerializeField] private Button toggleButton;

    private bool isCanvasActive = false;

    private void Start()
    {
        if (characterSelectionCanvas == null)
        {
            Debug.LogError("Character Selection Canvas is not assigned!");
            return;
        }

        if (toggleButton == null)
        {
            Debug.LogError("Toggle Button is not assigned!");
            return;
        }

        // Ensure the canvas is initially hidden
        characterSelectionCanvas.gameObject.SetActive(false);

        // Add listener to the toggle button
        toggleButton.onClick.AddListener(ToggleCharacterSelectionCanvas);
    }

    private void ToggleCharacterSelectionCanvas()
    {
        isCanvasActive = !isCanvasActive;
        characterSelectionCanvas.gameObject.SetActive(isCanvasActive);
        Debug.Log("Character selection canvas toggled: " + (isCanvasActive ? "shown" : "hidden"));
    }

    // Public method to hide the canvas (can be called from other scripts if needed)
    public void HideCanvas()
    {
        isCanvasActive = false;
        characterSelectionCanvas.gameObject.SetActive(false);
    }

    // Public method to show the canvas (can be called from other scripts if needed)
    public void ShowCanvas()
    {
        isCanvasActive = true;
        characterSelectionCanvas.gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        // Remove the listener when the script is destroyed
        if (toggleButton != null)
        {
            toggleButton.onClick.RemoveListener(ToggleCharacterSelectionCanvas);
        }
    }
}
