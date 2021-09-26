using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public PlayerInput playerInput;
    private string currentControlScheme;

    private void Start()
    {
        currentControlScheme = playerInput.currentControlScheme;
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
