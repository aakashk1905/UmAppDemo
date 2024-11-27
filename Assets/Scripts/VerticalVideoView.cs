using System.Collections.Generic;
using UnityEngine;

public class VerticalVideoView : MonoBehaviour
{
    [SerializeField] private Transform scrollViewContent;

    // Dictionary to store original parent and position of moved objects
    private Dictionary<Transform, Transform> originalParents = new Dictionary<Transform, Transform>();
    private Dictionary<Transform, Vector3> originalPositions = new Dictionary<Transform, Vector3>();

    private void Update()
    {
        UpdateScrollView();
    }

    public void UpdateScrollView()
    {
        GameObject carouselRoot = GameObject.Find("UICanvas/CarouselBase/CarouselContainer"); // Update this with your carousel root name
        if (carouselRoot == null)
        {
            Debug.LogError("Carousel root object not found!");
            return;
        }

        Transform[] allPanels = carouselRoot.GetComponentsInChildren<Transform>(true); // Include inactive panels

        // Collect and move all children from each panel
        foreach (Transform panel in allPanels)
        {
            // Skip the root itself and only process actual panels
            if (panel == carouselRoot.transform) continue;

            // Iterate over each child (object) inside the current panel
            foreach (Transform child in panel)
            {
                // Store the original parent and position
                if (!originalParents.ContainsKey(child))
                {
                    originalParents[child] = child.parent;
                    originalPositions[child] = child.localPosition;
                }

                // Move the child object to the ScrollView Content
                child.SetParent(scrollViewContent);

                // Optionally reset the object's transform
                child.localPosition = Vector3.zero;
                child.localRotation = Quaternion.identity;
                child.localScale = Vector3.one;
               
                child.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1);
                child.GetComponent<RectTransform>().sizeDelta = new Vector2(180,180);  
            }
        }
    }

    public void OnDestroy()
    {
        // Return all moved objects to their original parents and positions
        foreach (KeyValuePair<Transform, Transform> entry in originalParents)
        {
            Transform child = entry.Key;
            Transform originalParent = entry.Value;

            // Restore the original parent
            child.SetParent(originalParent);

            // Restore the original local position
            if (originalPositions.ContainsKey(child))
            {
                child.localPosition = originalPositions[child];
            }

            // Optionally reset rotation and scale
            child.localRotation = Quaternion.identity;
            child.localScale = Vector3.one;

            child.GetComponent<RectTransform>().sizeDelta = new Vector2(30, 30);
        }

        // Clear the stored data
        originalParents.Clear();
        originalPositions.Clear();
    }
}
