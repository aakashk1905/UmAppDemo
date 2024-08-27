using UnityEngine;
using System.Collections.Generic;
using Gpm.WebView;

public class WebViewController : MonoBehaviour
{
    // FullScreen
    public void ShowUrlFullScreen()
    {
        Debug.LogError("button CLicked");
        GpmWebView.ShowUrl(
            "https://upskillmafia.com/dashboard/mern?tab=tasks",
            new GpmWebViewRequest.Configuration()
            {
                style = GpmWebViewStyle.FULLSCREEN,
                orientation = GpmOrientation.UNSPECIFIED,
                isClearCookie = true,
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
            },
            OnCallback,
            new List<string>() { "USER_CUSTOM_SCHEME" }
        );
    }

    // Popup default
    public void ShowUrlPopupDefault()
    {
        GpmWebView.ShowUrl(
            "https://upskillmafia.com/mern/tasks",
            new GpmWebViewRequest.Configuration()
            {
                style = GpmWebViewStyle.POPUP,
                orientation = GpmOrientation.UNSPECIFIED,
                isClearCookie = true,
                isClearCache = true,
                isNavigationBarVisible = true,
                isCloseButtonVisible = true,
                supportMultipleWindows = true,
#if UNITY_IOS
                contentMode = GpmWebViewContentMode.MOBILE,
                isMaskViewVisible = true,
#endif
            },
            OnCallback,
            new List<string>() { "USER_CUSTOM_SCHEME" }
        );
    }

    // Popup custom position and size
    public void ShowUrlPopupPositionSize()
    {
        GpmWebView.ShowUrl(
            "https://upskillmafia.com/mern/tasks",
            new GpmWebViewRequest.Configuration()
            {
                style = GpmWebViewStyle.POPUP,
                orientation = GpmOrientation.UNSPECIFIED,
                isClearCookie = true,
                isClearCache = true,
                isNavigationBarVisible = true,
                isCloseButtonVisible = true,
                position = new GpmWebViewRequest.Position
                {
                    hasValue = true,
                    x = (int)(Screen.width * 0.1f),
                    y = (int)(Screen.height * 0.1f)
                },
                size = new GpmWebViewRequest.Size
                {
                    hasValue = true,
                    width = (int)(Screen.width * 0.8f),
                    height = (int)(Screen.height * 0.8f)
                },
                supportMultipleWindows = true,
#if UNITY_IOS
                contentMode = GpmWebViewContentMode.MOBILE,
                isMaskViewVisible = true,
#endif
            },
            OnCallback,
            new List<string>() { "USER_CUSTOM_SCHEME" }
        );
    }

    // Popup custom margins
    public void ShowUrlPopupMargins()
    {
        GpmWebView.ShowUrl(
            "https://upskillmafia.com/mern/tasks",
            new GpmWebViewRequest.Configuration()
            {
                style = GpmWebViewStyle.POPUP,
                orientation = GpmOrientation.UNSPECIFIED,
                isClearCookie = true,
                isClearCache = true,
                isNavigationBarVisible = true,
                isCloseButtonVisible = true,
                margins = new GpmWebViewRequest.Margins
                {
                    hasValue = true,
                    left = (int)(Screen.width * 0.1f),
                    top = (int)(Screen.height * 0.1f),
                    right = (int)(Screen.width * 0.1f),
                    bottom = (int)(Screen.height * 0.1f)
                },
                supportMultipleWindows = true,
#if UNITY_IOS
                contentMode = GpmWebViewContentMode.MOBILE,
                isMaskViewVisible = true,
#endif
            },
            OnCallback,
            new List<string>() { "USER_CUSTOM_SCHEME" }
        );
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
                break;
#endif
            default:
                Debug.LogError($"Unhandled callback type: {callbackType}");
                break;
        }
    }
}