using UnityEngine;
using System.Collections.Generic;
using Gpm.WebView;

public class WebViewController : MonoBehaviour
{
    public GameObject prefabToSpawn;
    [SerializeField] public GameObject[] toHideObjects;
    public void ShowUrlFullScreen()
    {
        for(int i = 0; i< toHideObjects.Length; i++)
        {
            toHideObjects[i].SetActive(false);
        }

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
                    // Add cookies after the page has loaded
                    AddCookiesViaJavaScript();
                }
                OnCallback(callback, data, error);
            },
            new List<string>() { "USER_CUSTOM_SCHEME" }
        );
    }

    private void AddCookiesViaJavaScript()
    {
        string userEmail = UserDataManager.Instance.GetUserEmail();
        string userName = UserDataManager.Instance.GetUserName();

        string script = $@"
            function setCookie(name, value, days) {{
                var expires = '';
                if (days) {{
                    var date = new Date();
                    date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
                    expires = '; expires=' + date.toUTCString();
                }}
                document.cookie = name + '=' + (value || '') + expires + '; path=/';
            }}
            setCookie('user_email', '{userEmail}', 7);
            setCookie('user_name', '{userName}', 7);
        ";


        GpmWebView.ExecuteJavaScript(script);
    }

    public void Close()
    {
        //mobileViewPage.SetActive(true);
        //zoom.SetActive(true);
        //relocator.SetActive(true);
        Destroy(prefabToSpawn);
        Debug.Log("Acitve");
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