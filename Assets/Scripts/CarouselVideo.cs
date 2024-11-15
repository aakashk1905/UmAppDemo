using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using System.Security.Cryptography;
//using UnityEngine.UIElements;

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
    public GameObject fullscreenVideoSurfacePrefab;
    private GameObject fullscreenVideoSurface; 

    public RectTransform content;
    public float buttonOffset = 10f;

    //private void Update()
    //{
    //    UpdateButtonPositions();
    //}

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
        CurrentPanel = panels[panels.Count - 1];

        if (CurrentPanel.transform.childCount >= videoScreensPerPanel)
        {
            CreateNewPanel();
            CurrentPanel = panels[panels.Count - 1];
        }

        UpdateNavigationButtons();

    }

    public void CreateNewPanel()
    {
        // Instantiate a new panel and add it to the carouselPanelContainer.
        GameObject newPanel = Instantiate(panelPrefab, carouselPanelContainer.transform);
        newPanel.GetComponent<HorizontalLayoutGroup>().childAlignment = TextAnchor.MiddleCenter;
        //newPanel.GetComponent<HorizontalLayoutGroup>().padding = new RectOffset(0, 0, 1, 0);
        newPanel.GetComponent<HorizontalLayoutGroup>().spacing = 0f;
        newPanel.SetActive(false); 

        panels.Add(newPanel);

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

            fullscreenVideoSurface = Instantiate(fullscreenVideoSurfacePrefab, FullScreenPanel.transform);

            RawImage fullscreenRawImage = fullscreenVideoSurface.GetComponent<RawImage>();
            fullscreenRawImage.texture = originalRawImage.texture; 

            FullScreenPanel.SetActive(true);

            isFullscreen = true;
        }
    }

    public void ExitFullscreen()
    {
        if (!isFullscreen) return;

        Destroy(fullscreenVideoSurface);

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
        if (panels.Count > 1)
        {
            nextButton.gameObject.SetActive(true);
            previousButton.gameObject.SetActive(true);

            nextButton.interactable = (currentPanelIndex < panels.Count - 1);
            previousButton.interactable = (currentPanelIndex > 0);
        }
        else
        {
            // Hide the buttons if there's only one panel or none
            nextButton.gameObject.SetActive(false);
            previousButton.gameObject.SetActive(false);
        }

        currentPanelCount();
    }

    void currentPanelCount()
    {
        curentPanelCountText.text = currentPanelIndex + 1 + " / " + panels.Count;
    }

    public void UpdateButtonPositions()
    {
        // Get the width of the content
        float contentWidth = content.rect.width;

        float carouselWidth = content.parent.GetComponent<RectTransform>().rect.width;

        nextButton.transform.position = new Vector3(nextButton.transform.position.x + 30 + contentWidth + buttonOffset, nextButton.transform.position.y);

        previousButton.transform.position = new Vector3(previousButton.transform.position.x - 30 - contentWidth - buttonOffset, previousButton.transform.position.y);
    }
}
