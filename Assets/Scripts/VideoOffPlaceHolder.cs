using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoOffPlaceHolder : MonoBehaviour
{
    private GameObject placeholder;

    public void SetPlaceholder(GameObject placeholderObject)
    {
        placeholder = placeholderObject;
    }

    public void ShowPlaceholder(bool show)
    {
        if (placeholder != null)
        {
            placeholder.SetActive(show);
        }
    }
}
