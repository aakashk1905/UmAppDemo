using UnityEngine;
using System.Collections.Generic;
using Gpm.WebView;

public class WebViewController : MonoBehaviour
{
    public GameObject prefabToSpawn;
    [SerializeField] public GameObject[] toHideObjects;
    public void ShowUrlFullScreen()
    {
        SetViewOfOtherObjects(false);

#if !UNITY_ANDROID && !UNITY_IOS
        if (prefabToSpawn != null)
        {
            Instantiate(prefabToSpawn, Vector3.zero, Quaternion.identity);
            return;
        }

#endif
        GpmWebViewRequest.Configuration config = new GpmWebViewRequest.Configuration()
        {
            style = GpmWebViewStyle.FULLSCREEN,
            orientation = GpmOrientation.PORTRAIT,
            isClearCookie = false,
            isClearCache = true,
            backgroundColor = "#FFFFFF",
            isNavigationBarVisible = true,
            navigationBarColor = "#4B96E6",
            title = "Task Section",
            isBackButtonVisible = true,
            isForwardButtonVisible = true,
            isCloseButtonVisible = true,
            supportMultipleWindows = true,

#if UNITY_IOS
                contentMode = GpmWebViewContentMode.MOBILE
#endif
        };


        GpmWebView.ShowUrl(
            "https://upskillmafia.com/dashboard/mern?tab=tasks",
            config,
            (callback, data, error) =>
            {
                if (callback == GpmWebViewCallback.CallbackType.PageLoad)
                {
                   
                }
                OnCallback(callback, data, error);
            },
            new List<string>() { "USER_CUSTOM_SCHEME" }
        );
    }

    //    public void ShowUrlFullScreen()
    //    {
    //        // Hide the specified objects
    //        for (int i = 0; i < toHideObjects.Length; i++)
    //        {
    //            toHideObjects[i].SetActive(false);
    //        }

    //#if !UNITY_ANDROID && !UNITY_IOS
    //        if (prefabToSpawn != null)
    //        {
    //            Instantiate(prefabToSpawn, Vector3.zero, Quaternion.identity);
    //            return;
    //        }
    //#endif

    //        // Spawn the scroll view content object dynamically
    //        GameObject spawnedContentParent = Instantiate(prefabToSpawn, Vector3.zero, Quaternion.identity);
    //        Transform scrollViewContent = spawnedContentParent.transform.Find("Content"); // Ensure "Content" is properly referenced in the prefab hierarchy

    //        if (scrollViewContent == null)
    //        {
    //            Debug.LogError("ScrollView Content not found in the spawned prefab!");
    //            return;
    //        }

    //        // Locate the root object that contains the carousel panels
    //        GameObject carouselRoot = GameObject.Find("UICanvas/CarouselBase/CarouselContainer"); // Update this with your carousel root name
    //        if (carouselRoot == null)
    //        {
    //            Debug.LogError("Carousel root object not found!");
    //            return;
    //        }

    //        // Get all panel GameObjects under the carousel root
    //        Transform[] allPanels = carouselRoot.GetComponentsInChildren<Transform>(true); // Include inactive panels

    //        // Collect and move all children from each panel
    //        foreach (Transform panel in allPanels)
    //        {
    //            // Skip the root itself and only process actual panels
    //            if (panel == carouselRoot.transform) continue;

    //            // Iterate over each child (object) inside the current panel
    //            foreach (Transform child in panel)
    //            {
    //                // Move the child object to the ScrollView Content
    //                child.SetParent(scrollViewContent);

    //                // Optionally reset the object's transform
    //                child.localPosition = Vector3.zero;
    //                child.localRotation = Quaternion.identity;
    //                child.localScale = Vector3.one;
    //            }
    //        }

    //        // Optionally, update the layout of the ScrollView Content
    //        // LayoutRebuilder.ForceRebuildLayoutImmediate(scrollViewContent.GetComponent<RectTransform>());
    //    }


    public void Close()
    {
        Destroy(prefabToSpawn);
    }

    public void SetViewOfOtherObjects(bool val)
    {
        for (int i = 0; i < toHideObjects.Length; i++)
        {
            toHideObjects[i].SetActive(val);
        }
    }
     private void OnCallback(GpmWebViewCallback.CallbackType callbackType, string data, GpmWebViewError error)
    {
        Debug.LogError($"WebView Callback: {callbackType}");

        switch (callbackType)
        {
            case GpmWebViewCallback.CallbackType.Open:
                if (error != null)
                {
                    Debug.LogError($"Failed to open WebView. Error: {error}");
                }
                else
                {
                    Debug.LogError("WebView opened successfully.");
                }
                break;
            case GpmWebViewCallback.CallbackType.Close:
                if (error != null)
                {
                    Debug.LogError($"Failed to close WebView. Error: {error}");
                }
                else
                {
                    Debug.LogError("WebView closed successfully.");
                    SetViewOfOtherObjects(true);
                }
                break;
            case GpmWebViewCallback.CallbackType.PageStarted:
                if (!string.IsNullOrEmpty(data))
                {
                    Debug.LogError($"Page started loading: {data}");
                }
                break;
            case GpmWebViewCallback.CallbackType.PageLoad:
                if (!string.IsNullOrEmpty(data))
                {
                    Debug.LogError($"Page loaded: {data}");
                }
                break;
            case GpmWebViewCallback.CallbackType.MultiWindowOpen:
                Debug.Log("Multi-window opened.");
                break;
            case GpmWebViewCallback.CallbackType.MultiWindowClose:
                Debug.Log("Multi-window closed.");
                break;
            case GpmWebViewCallback.CallbackType.Scheme:
                if (error == null)
                {
                    if (data.Equals("USER_CUSTOM_SCHEME") || data.Contains("CUSTOM_SCHEME"))
                    {
                        Debug.LogError($"Custom scheme detected: {data}");
                    }
                }
                else
                {
                    Debug.LogError($"Failed to handle custom scheme. Error: {error}");
                }
                break;
            case GpmWebViewCallback.CallbackType.GoBack:
                Debug.LogError("Navigated back.");
                break;
            case GpmWebViewCallback.CallbackType.GoForward:
                Debug.LogError("Navigated forward.");
                break;
            case GpmWebViewCallback.CallbackType.ExecuteJavascript:
                if (error != null)
                {
                    Debug.LogError($"Failed to execute JavaScript. Error: {error}");
                }
                else
                {
                    Debug.LogError($"JavaScript executed. Result: {data}");
                }
                break;
#if UNITY_ANDROID
            case GpmWebViewCallback.CallbackType.BackButtonClose:
                Debug.Log("WebView closed via back button.");
                SetViewOfOtherObjects(true);
                break;
#endif
            default:
                Debug.LogError($"Unhandled callback type: {callbackType}");
                break;
        }
    }
}