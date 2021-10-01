using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIController : MonoBehaviour
{
    PlayerController playerController;
    private void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    public void OnUnpause(InputAction.CallbackContext context)
    {
        if (Game.instance.IsPlayerRegistered(playerController))
        {
            if (context.performed)
            {
                Game.instance.Unpause();
            }
        }
    }
}
