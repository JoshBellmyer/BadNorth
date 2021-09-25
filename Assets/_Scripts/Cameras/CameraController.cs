using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    public PlayerInput playerInput;
    public Vector3 rotationPoint = Vector3.zero;

    public float rotateSpeed;
    public float rotateMin;
    public float rotateMax;

    public float zoomSpeed;
    public float zoomMin;
    public float zoomMax;

    private Vector2 rawInputRotation;
    private float rawInputZoom;

    private string currentControlScheme;
    private Camera camera;

    private void Start()
    {
        currentControlScheme = playerInput.currentControlScheme;
        camera = GetComponent<Camera>();
    }

    private void Update()
    {
        // Rotation
        Vector2 rotation = -rawInputRotation * rotateSpeed;
        transform.RotateAround(rotationPoint, Vector3.up, rotation.x * Time.deltaTime);
        if ((transform.rotation.eulerAngles.x < rotateMax && rotation.y < 0) || (transform.rotation.eulerAngles.x > rotateMin && rotation.y > 0))
        {
            transform.RotateAround(rotationPoint, -transform.right, rotation.y * Time.deltaTime);
        }

        // Zoom
        float zoom = -rawInputZoom * zoomSpeed;
        camera.orthographicSize += zoom;
        camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, zoomMin, zoomMax);
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

    //This is automatically called from PlayerInput, when the input device has changed
    //(IE: Keyboard -> Xbox Controller)
    public void OnControlsChanged()
    {

        if (playerInput.currentControlScheme != currentControlScheme)
        {
            currentControlScheme = playerInput.currentControlScheme;

            Debug.Log("OnControlsChanged");
            RemoveAllBindingOverrides();
        }
    }

    public void OnDeviceLost()
    {
        Debug.Log("OnDeviceLost");
    }


    public void OnDeviceRegained()
    {
        Debug.Log("OnDeviceRegained");
    }

    void RemoveAllBindingOverrides()
    {
        InputActionRebindingExtensions.RemoveAllBindingOverrides(playerInput.currentActionMap);
    }
}
