using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class CarouselVideo : MonoBehaviour
{
    [SerializeField] private GameObject panelPrefab, carouselPanelContainer;
    public GameObject carousel;
    private List<GameObject> panels = new List<GameObject>();

    private int videoScreensPerPanel = 5;

    internal GameObject CurrentPanel;
    int currentPanelIndex = 0;

    [SerializeField] Button nextButton, previousButton;
    [SerializeField] TextMeshProUGUI curentPanelCountText;

    //FullScreen
    [SerializeField] GameObject FullScreenPanel;
    private bool isFullscreen = false;
    [SerializeField] Button exitFullScreen;
    public GameObject fullscreenVideoSurfacePrefab; // Assign a prefab for the fullscreen surface
    private GameObject fullscreenVideoSurface; // Reference to the fullscreen surface

    public void ActivateCarousel()
    {
        if (carousel != null) // Check if carousel is assigned
        {
            carousel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Carousel is not assigned!");
        }
    }

    public void currentPanel()
    {
        // Get the last panel.
        CurrentPanel = panels[panels.Count - 1];

        // Check if the current panel has space for more images.
        if (CurrentPanel.transform.childCount >= videoScreensPerPanel)
        {
            // If the current panel is full, create a new panel.
            CreateNewPanel();
            CurrentPanel = panels[panels.Count - 1];
        }

        UpdateNavigationButtons();

    }

    public void CreateNewPanel()
    {
        // Instantiate a new panel and add it to the carouselPanelContainer.
        GameObject newPanel = Instantiate(panelPrefab, carouselPanelContainer.transform);
        newPanel.GetComponent<HorizontalLayoutGroup>().childAlignment = TextAnchor.MiddleCenter; // Center alignment for images.
        newPanel.SetActive(false); // Disable the new panel by default.

        // Add the new panel to the list of panels.
        panels.Add(newPanel);

        // If this is the first panel, set it active.
        if (panels.Count == 1)
        {
            newPanel.SetActive(true);
        }
    }

    public void ToggleFullscreen(GameObject originalVideoSurface)
    {
        exitFullScreen.onClick.AddListener(ExitFullscreen);

        if (isFullscreen)
        {
            ExitFullscreen();
        }
        else
        {
            RawImage originalRawImage = originalVideoSurface.GetComponent<RawImage>();

            // Create a new fullscreen video surface
            fullscreenVideoSurface = Instantiate(fullscreenVideoSurfacePrefab, FullScreenPanel.transform);

            // Copy the video feed from the original to the fullscreen surface
            RawImage fullscreenRawImage = fullscreenVideoSurface.GetComponent<RawImage>();
            fullscreenRawImage.texture = originalRawImage.texture; // Assuming video texture is in RawImage

            // Make the fullscreen panel active
            FullScreenPanel.SetActive(true);

            isFullscreen = true;
        }
    }

    public void ExitFullscreen()
    {
        if (!isFullscreen) return;

        // Destroy the fullscreen surface
        Destroy(fullscreenVideoSurface);

        // Deactivate the fullscreen panel
        FullScreenPanel.SetActive(false);

        fullscreenVideoSurface = null;
        isFullscreen = false;
    }

    public void NextPanel()
    {
        if (currentPanelIndex < panels.Count - 1)
        {
            panels[currentPanelIndex].SetActive(false);
            currentPanelIndex++;
            panels[currentPanelIndex].SetActive(true);
            UpdateNavigationButtons();
        }
    }

    // Function to navigate to the previous panel.
    public void PreviousPanel()
    {
        if (currentPanelIndex > 0)
        {
            panels[currentPanelIndex].SetActive(false);
            currentPanelIndex--;
            panels[currentPanelIndex].SetActive(true);
            UpdateNavigationButtons();
        }
    }

    // Enable/Disable navigation buttons based on the current panel index.
    private void UpdateNavigationButtons()
    {
        nextButton.interactable = (currentPanelIndex < panels.Count - 1);
        previousButton.interactable = (currentPanelIndex > 0);
        currentPanelCount();
    }

    void currentPanelCount()
    {
        curentPanelCountText.text = currentPanelIndex + 1 + " / " + panels.Count;
    }

}
