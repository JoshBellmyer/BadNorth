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

	// Returns the position at the center of the grid square containing the given pos
	public static Vector3 GetGridPos (Vector3 pos) {
		float xPos = Mathf.Floor(pos.x) + 0.5f;
		float yPos = pos.y;
		float zPos = Mathf.Floor(pos.z) + 0.5f;

		RaycastHit hit;
		bool hitGround = Physics.Raycast(new Vector3(xPos, pos.y + 1, zPos), Vector3.down, out hit, 2.2f, LayerMask.GetMask("Terrain"));

		if (hitGround) {
			yPos = hit.point.y;
		}

		return new Vector3(xPos, yPos, zPos);
	}

	public static Vector3 GetGridNormal (Vector3 pos) {
		float xPos = Mathf.Floor(pos.x) + 0.5f;
		float zPos = Mathf.Floor(pos.z) + 0.5f;

		RaycastHit hit;
		bool hitGround = Physics.Raycast(new Vector3(xPos, pos.y + 1, zPos), Vector3.down, out hit, 2.2f, LayerMask.GetMask("Terrain"));

		if (hitGround) {
			return hit.normal;
		}

		return Vector3.zero;
	}

	// Returns the position at the same x and z as the given pos but, level with the ground
	public static Vector3 GetGroundLevelPos (Vector3 pos) {
		float yPos = pos.y;
		RaycastHit hit;
		bool hitGround = Physics.Raycast(new Vector3(pos.x, pos.y + 1, pos.z), Vector3.down, out hit, 2.2f, LayerMask.GetMask("Terrain"));

		if (hitGround) {
			yPos = hit.point.y;
		}

		return new Vector3(pos.x, yPos, pos.z);
	}

	public static Vector3 GetTopFromSide (Vector3 pos, Vector3 normal) {
		Vector3 newPos = pos + (normal * -0.5f) + new Vector3(0, 0.5f, 0);

		return GetGridPos(newPos);
	}

	public static Vector3 GetSideGridPos (Vector3 pos, Vector3 normal) {
		float xPos = pos.x;
		float yPos = Mathf.Floor(pos.y) + 0.5f;
		float zPos = pos.z;

		if (normal.x != 0) {
			zPos = Mathf.Floor(pos.z) + 0.5f;
		}
		else {
			xPos = Mathf.Floor(pos.x) + 0.5f;
		}

		return new Vector3(xPos, yPos, zPos);
	}

	public static bool CheckForTerrainAbove (Vector3 pos) {
		Collider[] cols = Physics.OverlapSphere(pos + new Vector3(0, 0.75f, 0), 0.35f, LayerMask.GetMask("Terrain"));

		return (cols.Length > 0);
	}
}
