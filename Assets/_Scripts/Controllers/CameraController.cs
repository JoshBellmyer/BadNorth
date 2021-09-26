using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    public new Camera camera;

    private void Start()
    {
        camera = playerInput.camera;
    }

    private void Update()
    {
        // Rotation
        Vector2 rotation = -rawInputRotation * rotateSpeed;
        camera.transform.RotateAround(rotationPoint, Vector3.up, rotation.x * Time.deltaTime);
        if ((camera.transform.rotation.eulerAngles.x < rotateMax && rotation.y < 0) || (camera.transform.rotation.eulerAngles.x > rotateMin && rotation.y > 0))
        {
            camera.transform.RotateAround(rotationPoint, -camera.transform.right, rotation.y * Time.deltaTime);
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
}
