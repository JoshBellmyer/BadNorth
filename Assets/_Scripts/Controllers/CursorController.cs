using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
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
    private Player player;

    private void Start()
    {
        camera = playerInput.camera;
        playerController = GetComponent<PlayerController>();
        player = GetComponent<Player>();
    }

    private void Update()
    {
        cursorTransform.localPosition += new Vector3(rawInputDelta.x, rawInputDelta.y) * player.settings.cursorSensitivity;
        float clampX = Mathf.Clamp(cursorTransform.localPosition.x, -canvasTransform.rect.width / 2, canvasTransform.rect.width / 2);
        float clampY = Mathf.Clamp(cursorTransform.localPosition.y, -canvasTransform.rect.height / 2, canvasTransform.rect.height / 2);
        cursorTransform.localPosition = new Vector3(clampX, clampY);

        Tuple<bool, RaycastHit> hitData = CastFromCursor(LayerMask.GetMask("Terrain"));

        if (hitData.Item1) {
            if (hitData.Item2.normal.y > 0) {
                MoveSelection(hitData.Item2.point, hitData.Item2.normal);
            }
            else {
                SetSelectionActive(false);
            }
        }
        else {
            SetSelectionActive(false);
        }
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

                Tuple<bool, RaycastHit> hitData = CastFromCursor(Game.everythingMask);

                if (hitData.Item1) {
                    Vector3 location = hitData.Item2.point;

                    Debug.Log($"{location} -> {Game.GetGridPos(location)}");
                }

                Debug.Log("Select");
            }
        }
    }

    public Tuple<bool, RaycastHit> CastFromCursor (int layerMask) {
        RaycastHit hit;
        Vector3 cursorPosition = cursorTransform.localPosition + new Vector3(canvasTransform.rect.width / 2, canvasTransform.rect.height / 2);
        Vector3 viewPortPosition = new Vector3(cursorPosition.x / canvasTransform.rect.width, cursorPosition.y / canvasTransform.rect.height); // view port is normalized
        cursorSelectRay = camera.ViewportPointToRay(viewPortPosition);

        bool hitSomething = Physics.Raycast(cursorSelectRay, out hit, layerMask);

        return new Tuple<bool, RaycastHit>(hitSomething, hit);
    }

    private void MoveSelection (Vector3 position, Vector3 normal) {
        if (player.GridSelection == null) {
            return;
        }

        SetSelectionActive(true);
        player.GridSelection.transform.position = Game.GetGridPos(position);
        player.GridSelection.transform.up = normal;
    }

    private void SetSelectionActive (bool newActive) {
        if (player.GridSelection == null) {
            return;
        }

        player.GridSelection.SetActive(newActive);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(cursorSelectRay);

    }
}











