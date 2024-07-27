using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraMovement : MonoBehaviour
{
    private static readonly float PanSpeed = 10f;

    private static readonly float[] BoundsX = new float[] { -3f, 3f };
    private static readonly float[] BoundsY = new float[] { -3f, 3f }; // Added bounds for y-axis
    private static readonly float[] BoundsZ = new float[] { -10f, 0f };

    private Camera cam;

    private Vector3 lastPanPosition;
    private int panFingerId; // Touch mode only

    private bool wasZoomingLastFrame; // Touch mode only
    private Vector2[] lastZoomPositions; // Touch mode only

    bool _cameraCanMove = false;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        if (Input.touchSupported && Application.platform != RuntimePlatform.WebGLPlayer)
        {
            HandleTouch();
        }
        else
        {
            HandleMouse();
        }
    }

    void HandleTouch()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                _cameraCanMove = false;
                if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                {
                    _cameraCanMove = true;
                }

                lastPanPosition = touch.position;
                panFingerId = touch.fingerId;
            }
            else if (_cameraCanMove && touch.fingerId == panFingerId && touch.phase == TouchPhase.Moved)
            {
                PanCamera(touch.position);
            }
        }
    }

    void HandleMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _cameraCanMove = !TouchOnUI();
            lastPanPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0) && _cameraCanMove)
        {
            PanCamera(Input.mousePosition);
        }
    }

    void PanCamera(Vector3 newPanPosition)
    {
        // Determine how much to move the camera
        Vector3 offset = cam.ScreenToViewportPoint(lastPanPosition - newPanPosition);
        Vector3 move = new Vector3(offset.x * PanSpeed, offset.y * PanSpeed, 0);

        // Perform the movement
        transform.Translate(move, Space.World);

        // Ensure the camera remains within bounds.
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(transform.position.x, BoundsX[0], BoundsX[1]);
        pos.y = Mathf.Clamp(transform.position.y, BoundsY[0], BoundsY[1]); // Added clamping for y-axis
        pos.z = Mathf.Clamp(transform.position.z, BoundsZ[0], BoundsZ[1]);
        transform.position = pos;

        // Cache the position
        lastPanPosition = newPanPosition;
    }

    bool TouchOnUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
}
