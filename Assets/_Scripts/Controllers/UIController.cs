using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject settingsMenu;
    [SerializeField] OverlayMenu overlayMenu;

    PlayerController playerController;
    private MultiplayerEventSystem eventSystem;

    

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        overlayMenu.SelectedUnitIndex = 0;
        eventSystem = GetComponent<MultiplayerEventSystem>();
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(false);
    }

    public void OnDeployUnit(InputAction.CallbackContext context)
    {
        if (Game.instance.IsPlayerRegistered(playerController))
        {
            if (context.performed)
            {
                Debug.Log("Deploying: " + overlayMenu.SelectedUnitType);
                playerController.Boat.SetSail();
            }
        }
    }

    public void OnUnitSelectChange(InputAction.CallbackContext context)
    {
        if (Game.instance.IsPlayerRegistered(playerController))
        {
            if (context.performed)
            {
                overlayMenu.SelectedUnitIndex += (int)context.ReadValue<float>();
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
                pauseMenu.SetActive(true);

                SelectSomething();
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
                ClearUI();
            }
        }
    }

    public void ClearUI()
    {
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(false);
    }

    public void SelectSomething()
    {
        eventSystem.SetSelectedGameObject(FindObjectOfType<Selectable>().gameObject, new BaseEventData(eventSystem));
    }
}
