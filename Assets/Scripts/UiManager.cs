using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    [SerializeField] GameObject settingsScreen;
    [SerializeField] GameObject activeUserScreen;
    [SerializeField] GameObject DmPage;

    public void EnableScreen()
    {
        if(settingsScreen.activeInHierarchy == false)
        {
            settingsScreen.SetActive(true);
        }
    }

    public void DisableScreen()
    {
        if (settingsScreen.activeInHierarchy == true)
        {
            settingsScreen.SetActive(false);
        }
    }

    public void EnableUserScreen()
    {
        if (activeUserScreen.activeInHierarchy == false)
        {
            activeUserScreen.SetActive(true);
        }
    }

    public void DisableUserScreen()
    {
        if (activeUserScreen.activeInHierarchy == true)
        {
            activeUserScreen.SetActive(false);
        }
    }

    public void EnableDmPage()
    {
        if (DmPage.activeInHierarchy == false)
        {
            DmPage.SetActive(true);
        }
    }

    public void DisableDmPage()
    {
        if (DmPage.activeInHierarchy == true)
        {
            DmPage.SetActive(false);
        }
    }
}
