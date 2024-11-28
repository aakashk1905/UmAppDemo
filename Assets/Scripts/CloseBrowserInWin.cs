using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CloseBrowserInWin : MonoBehaviour
{
    [SerializeField] GameObject gameObject;
    [SerializeField] VerticalVideoView verticalVideoView;

   public void close()
    {
        verticalVideoView.OnDestroy();
        GameObject webTestObject = GameObject.Find("WebTest");

        if (webTestObject == null)
        {
            Debug.LogError("WebTest GameObject not found.");
        }
        else
        {
            WebViewController webViewController = webTestObject.GetComponent<WebViewController>();
            if (webViewController == null)
            {
                Debug.LogError("WebViewController not found on WebTest.");
            }
            else
            {
                Debug.Log("WebViewController successfully found.");
                for (int i = 0; i < webViewController.toHideObjects.Length; i++)
                {
                    webViewController.toHideObjects[i].gameObject.SetActive(true);
                }
            }
        }
        Destroy(gameObject);
    }
}
