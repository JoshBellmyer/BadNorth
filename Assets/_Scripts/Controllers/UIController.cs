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

        OverlayMenu overlayMenu = playerUIManager.GetMenu<OverlayMenu>();

        if (overlayMenu.unitsVisible) {
            Type type = UnitManager.UnitEnumToType((UnitType)player.SelectedUnitIndex);
            var typeG = typeof (Group<>).MakeGenericType(type);

            if (type != null) {
                dynamic unitGroup = Activator.CreateInstance(typeG);
                unitGroup.Initialize($"{player.playerId}");
                Boat boat = Instantiate<Boat>(Game.instance.boatPrefab);
                boat.SetPlayer(playerController);
                player.Boat = boat;
                unitGroup.CanMove = false;
                unitGroup.CanAttack = false;
                boat.MountUnits( unitGroup.GetUnitsBase() );

                overlayMenu.SetUnitsVisible(false);
            }
        }
        else {
            player.Boat.SetSail();
        }
    }

    public void OnUnitSelectChange(InputAction.CallbackContext context)
    {
        if (Game.instance.IsPlayerRegistered(playerController))
        {
            if (context.performed)
            {
                player.SelectedUnitIndex += (int)context.ReadValue<float>();
                playerUIManager.GetMenu<OverlayMenu>().SetSelectedUnitIndex(player.SelectedUnitIndex);
            }
        }
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (Game.instance.IsPlayerRegistered(playerController)) // Accounts for Unity bug, see https://forum.unity.com/threads/player-input-manager-adds-an-extra-player-with-index-1.1039000/
        {
            if (context.performed)
            {
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
