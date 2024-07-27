using UnityEngine;
using UnityEngine.UI;


public class DynamicVideoList : MonoBehaviour
{
    public GameObject content; // Reference to the Content GameObject
    public GameObject imagePrefab; // Prefab of the Image to be added
    private int imageCount = 1;
    private const int maxImages = 5;
    private const int maxImagesPerPage = 5;
    private int currentPage = 0;


    public Button nextButton; // Reference to the Next Button in the UI
    public Button prevButton; // Reference to the Previous Button in the UI
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddImage();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            PreviousPage();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            NextPage();
        }
    }

    void AddImage()
    {
        
            GameObject newImage = Instantiate(imagePrefab, content.transform);
            newImage.name = "Image " + imageCount;
            imageCount++;
            AdjustScrollViewSize();
        
    }

    void AdjustScrollViewSize()
    {
        // Adjust the width of the content based on the number of images
        RectTransform contentRect = content.GetComponent<RectTransform>();
        contentRect.sizeDelta = new Vector2(imageCount * 150, contentRect.sizeDelta.y); // Assuming each image has a width of 150
        UpdatePage(currentPage);
    }

    void UpdatePage(int page)
    {
        int startIndex = page * maxImagesPerPage;
        int endIndex = startIndex+maxImages;

        for (int i = 0; i < content.transform.childCount; i++)
        {
            GameObject imageObj = content.transform.GetChild(i).gameObject;
            bool shouldBeActive = (i >= startIndex && i < endIndex);
            imageObj.SetActive(shouldBeActive);
        }
        UpdateNavigationButtons();
    }


    public void NextPage()
    {
        if (currentPage < Mathf.CeilToInt((float)imageCount / maxImagesPerPage) - 1)
        {
            currentPage++;
            UpdatePage(currentPage);
        }
    }

    public void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            UpdatePage(currentPage);
        }
    }

    void UpdateNavigationButtons()
    {
        int numPages = Mathf.CeilToInt((float)imageCount / maxImagesPerPage);

        if (numPages > 1)
        {
            nextButton.gameObject.SetActive(currentPage < numPages - 1);
            prevButton.gameObject.SetActive(currentPage > 0);
        }
        else
        {
            nextButton.gameObject.SetActive(false);
            prevButton.gameObject.SetActive(false);
        }
    }
}

