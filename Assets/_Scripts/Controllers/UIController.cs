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

        SoundPlayer.PlaySound(Sound.MenuClick, 1, false);

        if (!isDeploying && player.Boat == null) {
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
                SoundPlayer.PlaySoundLocal(Sound.MenuMove, 1, false);

                if (isDeploying)
                {
                    player.CancelBoat();
                    isDeploying = false;
                }
                else // This else has to be here due to Unity bug, see https://issuetracker.unity3d.com/issues/input-system-control-index-is-out-of-range-exception-is-thrown-when-pressing-buttons
                {
                    player.SelectedUnitIndex += (int)context.ReadValue<float>();
                }
            }
        }
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (Game.instance.IsPlayerRegistered(playerController)) // Accounts for Unity bug, see https://forum.unity.com/threads/player-input-manager-adds-an-extra-player-with-index-1.1039000/
        {
            if (context.performed)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                Game.instance.Pause(playerController);
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
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

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
