using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CursorController : MonoBehaviour
{
    [SerializeField] RectTransform cursorTransform;
    [SerializeField] RectTransform canvasTransform;
    [SerializeField] PlayerInput playerInput;

    private Vector2 rawInputDelta;
    private new Camera camera;
    private Ray cursorSelectRay;

    private PlayerController playerController;

    private void Start()
    {
        camera = playerInput.camera;
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        cursorTransform.localPosition += new Vector3(rawInputDelta.x, rawInputDelta.y);
        float clampX = Mathf.Clamp(cursorTransform.localPosition.x, -canvasTransform.rect.width / 2, canvasTransform.rect.width / 2);
        float clampY = Mathf.Clamp(cursorTransform.localPosition.y, -canvasTransform.rect.height / 2, canvasTransform.rect.height / 2);
        cursorTransform.localPosition = new Vector3(clampX, clampY);
    }

    public void OnCursorMove(InputAction.CallbackContext value)
    {
        rawInputDelta = value.ReadValue<Vector2>();
    }

    public void OnCursorSelect(InputAction.CallbackContext context)
    {
        if (Game.instance.IsPlayerRegistered(playerController)) // Accounts for Unity bug, see https://forum.unity.com/threads/player-input-manager-adds-an-extra-player-with-index-1.1039000/
        {
            if (context.performed)
            {
                if (camera == null) return; // OnCursorSelect() can happen before Start() ???
                RaycastHit hit;
                Vector3 cursorPosition = cursorTransform.localPosition + new Vector3(canvasTransform.rect.width / 2, canvasTransform.rect.height / 2);
                Vector3 viewPortPosition = new Vector3(cursorPosition.x / canvasTransform.rect.width, cursorPosition.y / canvasTransform.rect.height); // view port is normalized
                cursorSelectRay = camera.ViewportPointToRay(viewPortPosition);

                if(Physics.Raycast(cursorSelectRay, out hit))
                {
                    Vector3 location = hit.point;
                    Debug.Log(location);
                }

                Debug.Log("Select");
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(cursorSelectRay);

    }
}
