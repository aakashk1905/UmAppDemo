using System.Collections;
using TMPro;
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
    public TextMeshProUGUI zoomPercentageText;
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
            if (Input.touchCount == 2) // Handle pinch-to-zoom for phones
            {
                Touch touch0 = Input.GetTouch(0);
                Touch touch1 = Input.GetTouch(1);

                // Calculate the distance between two touches in the current and previous frame
                float prevTouchDeltaMag = (touch0.position - touch0.deltaPosition - (touch1.position - touch1.deltaPosition)).magnitude;
                float touchDeltaMag = (touch0.position - touch1.position).magnitude;

                // Find the difference in distances
                float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

                // Adjust zoom based on the difference
                Zoom(deltaMagnitudeDiff * 0.01f); // Scale factor to make zoom smoother
            }
            else if (Input.GetAxis("Mouse ScrollWheel") != 0f) // Handle mouse zoom for desktops
            {
                Zoom(Input.GetAxis("Mouse ScrollWheel"));
            }
        }

        UpdateZoomPercentageText();
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

    void UpdateZoomPercentageText()
    {
        float zoomPercentage = (zoomOutMax - Camera.main.orthographicSize) / (zoomOutMax - zoomOutMin) * 100f;
        zoomPercentageText.text = Mathf.RoundToInt(zoomPercentage) + "%";
    }

    void Refocus()
    {
        Camera.main.transform.position = new Vector3(player.position.x, player.position.y, Camera.main.transform.position.z);
    }
}
