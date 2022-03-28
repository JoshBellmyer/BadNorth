using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.Netcode.Samples;


public class Game : MonoBehaviour {

	public static Game instance;
	public static NetworkManager networkManager;
	public static NetworkHelper networkHelper;

	public bool isPaused;
	public List<PlayerController> players;

	public const int everythingMask = 0x7FFFFFFF;

	public TerrainSettings terrainSettings;

	public static bool online;
	public static bool isHost;


	private void Start()
	{
		if (instance == null || instance == this)
		{
			instance = this;
		}
		else
		{
			Destroy(gameObject);

			return;
		}

		DontDestroyOnLoad(gameObject);

		PrefabFactory.Initialize();
		ParticleSpawner.LoadParticles();

		SetUpSceneManagement();
	}


	private void SetUpSceneManagement()
    {
        SceneManager.sceneLoaded += HandleSceneChange;
        if (SceneManager.GetActiveScene().name != "Title")
        {
            SetupGame();
        }
    }

	public void OnGameOver(string losingTeam)
    {
		string winningColor = losingTeam == "1" ? "yellow" : "blue";

		UIManager.Instance.DisplayWinner(winningColor);

		Pause(null);
		StartCoroutine(SwitchToMainMenuCoroutine(10));
	}

	private IEnumerator SwitchToMainMenuCoroutine(int delay)
    {
		for(float i = delay; i>0; i -= Time.deltaTime)
        {
			UIManager.Instance.UpdateCountdown(Mathf.CeilToInt(i));
			yield return null;
        }
		SwitchToMainMenu();
    }

    public void HandleSceneChange(Scene scene, LoadSceneMode mode)
    {
		if(scene.name != "Title")
		{
			SetupGame();
        }
    }

	public void SetupGame()
    {
    	if (online) {
    		if (isHost) {
    			SetupGameHost();
				PrefabFactory.CreateTerrainGenerator();
			}
    		else {
    			SetupGameClient();
    		}
    		return;
    	}

		Player.numPlayers = 2;
		players = new List<PlayerController>();
		PlayerController playerController1 = PrefabFactory.CreatePlayerController();
		PlayerController playerController2 = PrefabFactory.CreatePlayerController();
		RegisterPlayer(playerController1);
		RegisterPlayer(playerController2);
		playerController1.player.playerId = 1;
		playerController2.player.playerId = 2;

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		PrefabFactory.CreateTerrainGenerator();
	}

	public void SetupGameHost () {
		Player.numPlayers = 2;
		players = new List<PlayerController>();
		PlayerController playerController = PrefabFactory.CreatePlayerController();
		RegisterPlayer(playerController);
		playerController.player.playerId = 1;

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	public void SetupGameClient () {
		Player.numPlayers = 2;
		players = new List<PlayerController>();
		PlayerController playerController = PrefabFactory.CreatePlayerController();
		RegisterPlayer(playerController);
		playerController.player.playerId = 2;

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	// Clear the networking components from a gameobject to prevent errors in local mode
	public static void ClearNetworking (GameObject obj) {
		NetworkObject tempNO = obj.GetComponent<NetworkObject>();
		NetworkTransform tempNT = obj.GetComponent<NetworkTransform>();
		ClientNetworkTransform tempCNT = obj.GetComponent<ClientNetworkTransform>();

		if (tempNT != null) {
			DestroyImmediate(tempNT);
		}
		if (tempNO != null) {
			DestroyImmediate(tempNO);
		}
		if (tempCNT != null) {
			DestroyImmediate(tempCNT);
		}
	}


	public bool IsPlayerRegistered(PlayerController player)
    {
		return players.Contains(player);
    }

    internal void ExitGame()
    {
		Application.Quit();
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#endif
	}

	public static OnlinePlayer GetLocalPlayer () {
		NetworkObject obj = networkManager.LocalClient.PlayerObject;

		return obj.GetComponent<OnlinePlayer>();
	}

	public void SwitchToMainMenu()
	{
		Unpause();

		if (online) {
			networkManager.Shutdown();
		}

		Debug.Log(networkManager.gameObject);
		Destroy(networkManager.gameObject);

		online = false;
		isHost = false;

		SceneManager.LoadScene("Title");
    }

	public void RegisterPlayer(PlayerController playerControl)
    {
    	if (IsPlayerRegistered(playerControl)) {
    		return;
    	}

		players.Add(playerControl);
    }

	public void Pause(PlayerController playerController)
    {
    	if (!online) {
			isPaused = true;
    	}

		foreach(PlayerController player in players)
        {
			player.SetControlsActivated(false);
		}

		playerController?.SetControlsActivated(true);
		playerController?.SetActionMap("UI");
	}

	public void Unpause()
    {
		isPaused = false;

		foreach (PlayerController player in players)
		{
			player.SetControlsActivated(true);
			player.SetActionMap("Player");
		}
	}

	public static void SetParent (GameObject obj, GameObject newParent) {
		if (online) {
			NetworkObject netObj = obj.GetComponent<NetworkObject>();

			if (netObj != null && newParent == null) {
				GetLocalPlayer().UnsetParentServerRpc(netObj);

				return;
			}

			NetworkObject netParent = newParent.GetComponent<NetworkObject>();

			if (netObj != null && netParent != null) {
				GetLocalPlayer().SetParentServerRpc(netObj, netParent);
			}
			else {
				obj.transform.SetParent(newParent.transform);
			}
		}
		else {
			obj.transform.SetParent(newParent.transform);
		}
	}

	public static void SetPosition (GameObject obj, Vector3 newPos) {
		if (online) {
			NetworkObject netObj = obj.GetComponent<NetworkObject>();

			if (netObj != null) {
				GetLocalPlayer().SetPositionServerRpc(netObj, newPos);
			}
		}
		
		obj.transform.position = newPos;
	}

	public static void SetRotation (GameObject obj, Vector3 newRot) {
		if (online) {
			NetworkObject netObj = obj.GetComponent<NetworkObject>();

			if (netObj != null) {
				GetLocalPlayer().SetRotationServerRpc(netObj, newRot);
			}
		}

		obj.transform.eulerAngles = newRot;
	}
}
