using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;


public class UIController : MonoBehaviour
{
    [SerializeField] PlayerUIManager playerUIManager;
    Player player;

    PlayerController playerController;
    private MultiplayerEventSystem eventSystem;

    private bool isDeploying;

    private void Start()
    {
        player = GetComponent<Player>();
        eventSystem = GetComponent<MultiplayerEventSystem>();
        playerController = GetComponent<PlayerController>();
        player.SelectedUnitIndex = 0;
        playerUIManager.switchMenuEvent += OnSwitchMenu;
    }

    public void OnSwitchMenu()
    {
        SelectNavigationStart();
    }

    public void OnDeployUnit(InputAction.CallbackContext context)
    {
        if (!Game.instance.IsPlayerRegistered(playerController)) {
            return;
        }
        if (!context.performed) {
            return;
        }

        if (!isDeploying) {
            if(player.TryPrepBoat()) isDeploying = true;
        }
        else if (player.Boat != null) {
            if ( player.DeployBoat() ) {
                isDeploying = false;
            }
        }
    }

    public void OnUnitSelectChange(InputAction.CallbackContext context)
    {
        if (Game.instance.IsPlayerRegistered(playerController))
        {
            if (context.performed)
            {
                player.SelectedUnitIndex += (int)context.ReadValue<float>();
            }
        }
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (Game.instance.IsPlayerRegistered(playerController)) // Accounts for Unity bug, see https://forum.unity.com/threads/player-input-manager-adds-an-extra-player-with-index-1.1039000/
        {
            if (context.performed)
            {
                if (isDeploying)
                {
                    player.CancelBoat();
                    isDeploying = false;
                    return;
                }

                Game.instance.Pause();

                playerController.SetControlsActivated(true);
                playerController.SetActionMap("UI");
                playerUIManager.SwitchMenu(typeof(PauseMenu));
            }
        }
    }

    public void OnUnpause(InputAction.CallbackContext context)
    {
        if (Game.instance.IsPlayerRegistered(playerController))
        {
            if (context.performed)
            {
                Game.instance.Unpause();
                playerUIManager.SwitchMenu(typeof(OverlayMenu));
            }
        }
    }

    public void SelectNavigationStart()
    {
        eventSystem.SetSelectedGameObject(playerUIManager.SelectNavigationStart(), new BaseEventData(eventSystem));
    }
}
