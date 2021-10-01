using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    private PlayerInput playerInput;
    private string currentControlScheme;

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        currentControlScheme = playerInput.currentControlScheme;

        Game.instance.RegisterPlayer(this);
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
