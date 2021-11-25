using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public PlayerInput playerInput;

    private Vector3 rotationPoint = Vector3.zero;

    [SerializeField] float rotateMin;
    [SerializeField] float rotateMax;
    bool canRotate;
    float currentRotation; // need to keep track of this because rotation.eulerAngles is inconsistant

    [SerializeField] float zoomMin;
    [SerializeField] float zoomMax;
    bool canZoom;

    private Vector2 rawInputRotation;
    private float rawInputZoom;

    private new Camera camera;
    private Player player;

    bool isMouse;

    private void Start()
    {
        camera = playerInput.camera;
        currentRotation = camera.transform.rotation.eulerAngles.x;
        player = GetComponent<Player>();
        if(playerInput.devices[0] is Mouse)
        {
            canRotate = false;
            InputAction enableRotateAction = playerInput.currentActionMap.FindAction("Enable Rotate");
            enableRotateAction.performed += ctx => canRotate = true;
            enableRotateAction.canceled += ctx => canRotate = false;
            InputAction enableZoomAction = playerInput.currentActionMap.FindAction("Enable Zoom");
            enableZoomAction.performed += ctx => canZoom = true;
            enableZoomAction.canceled += ctx => canZoom = false;
            isMouse = true;
        }
        else
        {
            canRotate = true;
            canZoom = true;
        }

        SetCullingMask(player.playerId);
    }

    private void Update()
    {
        // Rotation
        if (canRotate)
        {
            if (isMouse) rawInputRotation = -rawInputRotation;
            Vector2 rotation = -rawInputRotation * player.settings.rotateSensitivity * Time.deltaTime;
            camera.transform.RotateAround(rotationPoint, Vector3.up, rotation.x);
            if ((currentRotation < rotateMax && rotation.y < 0) || (currentRotation > rotateMin && rotation.y > 0))
            {
                camera.transform.RotateAround(rotationPoint, -camera.transform.right, rotation.y);
                currentRotation += -rotation.y;
            }
        }

        // Zoom
        if (canZoom)
        {
            float zoom = -rawInputZoom * player.settings.zoomSensitivity;
            camera.orthographicSize += zoom;
            camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, zoomMin, zoomMax);
        }
    }

    public void ZoomOut () {
        camera.orthographicSize = zoomMax;

        float remainingAngle = camera.transform.eulerAngles.x - rotateMin;
        camera.transform.RotateAround(rotationPoint, -camera.transform.right, remainingAngle);
    }

    public void OnRotate(InputAction.CallbackContext value)
    {
        Vector2 inputRotate = value.ReadValue<Vector2>();
        rawInputRotation = inputRotate;
    }

    public void OnZoom(InputAction.CallbackContext value)
    {
        float inputZoom = value.ReadValue<float>();
        rawInputZoom = inputZoom;

    }

    private void SetCullingMask (int number) {
        int mask = 0;

        for (int i = 1; i <= 4; i++) {
            if (i != number) {
                mask = mask | LayerMask.GetMask($"Player {i}");
            }
        }

        camera.cullingMask = ~mask;
    }
}













