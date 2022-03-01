using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DeviceManager : Singleton<DeviceManager>
{
	public List<InputDevice> supportedDevices = new List<InputDevice>();

	public InputDevice[] playerDevices = new InputDevice[2];

	public bool HasValidDevices
    {
        get
        {
			foreach(InputDevice device in playerDevices)
            {
				if (device == null) return false;
            }
			return true;
        }
	}

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
			Destroy(gameObject);
			return;
        }
		DontDestroyOnLoad(gameObject);
	}

    private void Start()
	{
		InputSystem.onDeviceChange += OnDeviceChange;
		SceneManager.sceneLoaded += HandleSceneChange;

		foreach (InputDevice device in InputSystem.devices)
		{
			InputSystem.EnableDevice(device);
		}

		SetUpSupportedDevices();
	}

    private void HandleSceneChange(Scene scene, LoadSceneMode mode)
    {
		if (scene.name != "Title")
		{
			FinalizeDevices();
		}
	}

	private void FinalizeDevices()
	{
		foreach (InputDevice device in InputSystem.devices)
		{
			InputSystem.DisableDevice(device);
		}

		foreach(InputDevice device in playerDevices) {
			if (device != null) {
				InputSystem.EnableDevice(device);
			}
		}
	}

	public void SetPlayerDevice(int player, int deviceNum)
    {
        try
		{
			playerDevices[player] = supportedDevices[deviceNum];
        }
        catch (ArgumentOutOfRangeException)
        {
			playerDevices[player] = null;
		}
	}

	private void OnDeviceChange(InputDevice arg1, InputDeviceChange arg2)
    {
		SetUpSupportedDevices();
	}

    private void SetUpSupportedDevices()
	{
		supportedDevices.Clear();
		foreach (InputDevice device in InputSystem.devices)
		{
			if (IsDeviceSupported(device))
			{
				supportedDevices.Add(device);
			}
		}
	}

	private bool IsDeviceSupported(InputDevice device)
	{
		if (device is Gamepad || device is Keyboard || device is Mouse)
		{
			return true;
		}

		return false;
	}
}
