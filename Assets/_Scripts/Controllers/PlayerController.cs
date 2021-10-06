using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    private PlayerInput playerInput;
    private string currentControlScheme;
    private MultiplayerEventSystem eventSystem;

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        currentControlScheme = playerInput.currentControlScheme;

        Game.instance.RegisterPlayer(this);
        eventSystem = GetComponent<MultiplayerEventSystem>();
    }

    public void SetControlsActivated(bool enabled)
    {
        if (enabled)
        {
            playerInput.ActivateInput();
        }
        else
        {
            playerInput.DeactivateInput();
        }
    }

    public void SetActionMap(string name)
    {
        playerInput.SwitchCurrentActionMap(name);
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (Game.instance.IsPlayerRegistered(this)) // Accounts for Unity bug, see https://forum.unity.com/threads/player-input-manager-adds-an-extra-player-with-index-1.1039000/
        {
            if (context.performed)
            {
                Game.instance.Pause(this);
                eventSystem.SetSelectedGameObject(FindObjectOfType<Selectable>().gameObject, new BaseEventData(eventSystem));
            }
        }            
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
