using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    public Vector3 rotationPoint = Vector3.zero;

    public float rotateSpeed;
    public float rotateMin;
    public float rotateMax;

    public float zoomSpeed;
    public float zoomMin;
    public float zoomMax;

    private Vector2 rawInputRotation;
    private float rawInputZoom;

    private new Camera camera;

    private void Start()
    {
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
}
