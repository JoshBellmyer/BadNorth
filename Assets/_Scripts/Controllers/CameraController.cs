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

    [SerializeField] float zoomMin;
    [SerializeField] float zoomMax;

    private Vector2 rawInputRotation;
    private float rawInputZoom;

    private new Camera camera;
    private Player player;

    private void Start()
    {
        camera = playerInput.camera;
        player = GetComponent<Player>();

        SetCullingMask(player.playerId);
    }

    private void Update()
    {
        // Rotation
        Vector2 rotation = -rawInputRotation * player.settings.rotateSensitivity;
        camera.transform.RotateAround(rotationPoint, Vector3.up, rotation.x * Time.deltaTime);
        if ((camera.transform.rotation.eulerAngles.x < rotateMax && rotation.y < 0) || (camera.transform.rotation.eulerAngles.x > rotateMin && rotation.y > 0))
        {
            camera.transform.RotateAround(rotationPoint, -camera.transform.right, rotation.y * Time.deltaTime);
        }

        // Zoom
        float zoom = -rawInputZoom * player.settings.zoomSensitivity;
        camera.orthographicSize += zoom;
        camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, zoomMin, zoomMax);
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













