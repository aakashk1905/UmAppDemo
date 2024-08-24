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

    void Start()
    {
        // Add listeners for buttons
        zoomInButton.onClick.AddListener(() => Zoom(0.5f));  // Zoom in
        zoomOutButton.onClick.AddListener(() => Zoom(-0.5f));  // Zoom out
        refocusButton.onClick.AddListener(Refocus);
    }

    void Update()
    {
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            Zoom(difference * 0.01f);
        }

        Zoom(Input.GetAxis("Mouse ScrollWheel"));
    }

    void Zoom(float increment)
    {
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment, zoomOutMin, zoomOutMax);
    }

    void Refocus()
    {
        Camera.main.transform.position = new Vector3(player.position.x, player.position.y, Camera.main.transform.position.z);
    }
}
