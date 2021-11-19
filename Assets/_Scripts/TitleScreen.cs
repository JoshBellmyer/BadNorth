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

    [SerializeField] Dropdown player1DeviceSelection;
    [SerializeField] Dropdown player2DeviceSelection;

    private void Start()
    {
        InputSystem.onDeviceChange += OnDeviceChange;
        player1DeviceSelection.onValueChanged.AddListener(OnDevice1SelectionChange);
        player2DeviceSelection.onValueChanged.AddListener(OnDevice2SelectionChange);
        SetUpDeviceSelection();
    }

    private void OnDeviceChange(InputDevice arg1, InputDeviceChange arg2)
    {
        SetUpDeviceSelection();
    }

    private void SetUpDeviceSelection()
    {
        player1DeviceSelection.options.Clear();
        player2DeviceSelection.options.Clear();
        foreach (InputDevice device in DeviceManager.Instance.supportedDevices)
        {
            Dropdown.OptionData data = new Dropdown.OptionData(device.name);
            player1DeviceSelection.options.Add(data);
            player2DeviceSelection.options.Add(data);
        }
        SetInitialDeviceSelections();

        missingPlayerText.gameObject.SetActive(false);
    }

    private void SetInitialDeviceSelections() // TODO: load previous selections
    {
        player1DeviceSelection.value = 1; // set twice to get the correct text
        player1DeviceSelection.value = 0;

        if (player1DeviceSelection.options.Count > 1)
        {
            player2DeviceSelection.value = 1;
            player2DeviceSelection.enabled = true;
        }
        else
        {
            player2DeviceSelection.captionText.text = "Missing";
            player2DeviceSelection.enabled = false;
            if (player2DeviceSelection.gameObject == TitleUIManager.instance.GetSelected())
                TitleUIManager.instance.SetSelected(firstSelected);
        }
    }

    private void OnDevice1SelectionChange(int value)
    {
        if(value == player2DeviceSelection.value)
        {
            player2DeviceSelection.value = (player2DeviceSelection.value + 1) % player2DeviceSelection.options.Count;
        }
        missingPlayerText.gameObject.SetActive(false);
    }

    private void OnDevice2SelectionChange(int value)
    {
        if (value == player1DeviceSelection.value)
        {
            player1DeviceSelection.value = (player1DeviceSelection.value + 1) % player1DeviceSelection.options.Count;
        }
        missingPlayerText.gameObject.SetActive(false);
    }

    public void OnPlay()
    {
        DeviceManager.Instance.SetPlayerDevice(0, player1DeviceSelection.value);
        DeviceManager.Instance.SetPlayerDevice(1, player2DeviceSelection.value);
        if (DeviceManager.Instance.HasValidDevices)
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
