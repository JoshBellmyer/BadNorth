using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreen : UIScreen
{
    [SerializeField] Text missingPlayerText;

    [SerializeField] Text playerStatus1;
    [SerializeField] Text playerStatus2;

    bool playerConnected1;
    bool playerConnected2;

    private void Start()
    {
        InputSystem.onDeviceChange += OnDeviceChange;
        UpdateDevices();
    }

    private void OnDeviceChange(InputDevice arg1, InputDeviceChange arg2)
    {
        UpdateDevices();
    }

    private void UpdateDevices()
    {
        missingPlayerText.gameObject.SetActive(false);
        if(InputSystem.GetDevice<Keyboard>() != null)
        {
            playerStatus1.text = "Keyboard";
            playerConnected1 = true;
        }
        else
        {
            playerStatus1.text = "Not connected";
            playerConnected1 = false;
        }

        if (InputSystem.GetDevice<Gamepad>() != null)
        {
            playerStatus2.text = "Gamepad";
            playerConnected2 = true;
        }
        else
        {
            playerStatus2.text = "Not connected";
            playerConnected2 = false;
        }
    }

    public void OnPlay()
    {
        if(playerConnected1 && playerConnected2)
        {
            SceneManager.LoadScene("Island");
        }
        else
        {
            missingPlayerText.gameObject.SetActive(true);
        }
    }

    public void OnGameSettings()
    {
        manager.SetUIScreen("Game Settings Screen");
    }

    public void OnExitGame()
    {
        Game.instance.ExitGame();
    }

    public void OnDestroy () {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }
}
