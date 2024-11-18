using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CloseBrowserInWin : MonoBehaviour
{
    [SerializeField] GameObject gameObject;



   public void close()
    {
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
                foreach (GameObject go in webViewController.toHideObjects)
                {
                    go.SetActive(true);
                }
            }
        }
        Destroy(gameObject);
    }
}