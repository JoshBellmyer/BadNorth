using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    private PlayerInput playerInput;
    private string currentControlScheme;
    public Player player;


    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        player = GetComponent<Player>();
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

    public void SetDevice(InputDevice device)
    {
        if(device is Keyboard)
        {
            playerInput.SwitchCurrentControlScheme(device, DeviceManager.Instance.mouse);
        }
        else
        {
            playerInput.SwitchCurrentControlScheme(device);
        }
    }

    //This is automatically called from PlayerInput, when the input device has changed
    //(IE: Keyboard -> Xbox Controller)
    public void OnControlsChanged()
    {

        if (playerInput.currentControlScheme != currentControlScheme)
        {
            currentControlScheme = playerInput.currentControlScheme;

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
