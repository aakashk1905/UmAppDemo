using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZoomControl : MonoBehaviour
{
    Vector3 touchStart;
    public float zoomOutMin = 1f;
    public float zoomOutMax = 10f;
    public Button zoomInButton;
    public Button zoomOutButton;
    public Button refocusButton;
    public Text zoomPercentageText;
    public Transform player;

    [SerializeField] GameObject dmScreen, ActiveUserPanel;

    void Start()
    {
        zoomInButton.onClick.AddListener(() => Zoom(0.5f)); 
        zoomOutButton.onClick.AddListener(() => Zoom(-0.5f));  
        refocusButton.onClick.AddListener(Refocus);
    }

    void Update()
    {
        if (!dmScreen.activeSelf && !ActiveUserPanel.activeSelf)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0f)
            {
                Zoom(scroll);
            }
        }
    }

    void Zoom(float increment)
    {
        float newSize = Mathf.Clamp(Camera.main.orthographicSize - increment, zoomOutMin, zoomOutMax);

        float zoomFactor = Camera.main.orthographicSize / newSize;

        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector3 direction = mouseWorldPosition - Camera.main.transform.position;
        Vector3 targetPosition = Camera.main.transform.position + direction * (1 - 1 / zoomFactor);

        Camera.main.transform.position = targetPosition;
        Camera.main.orthographicSize = newSize;
    }

    void Refocus()
    {
        Camera.main.transform.position = new Vector3(player.position.x, player.position.y, Camera.main.transform.position.z);
    }
}
