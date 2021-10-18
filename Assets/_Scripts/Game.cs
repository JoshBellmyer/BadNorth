using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

public class Game : MonoBehaviour {

	public static Game instance;
	public static MapGenerator mapGenerator;

	[Header("Prefabs")]
	public Boat boatPrefab;
	public GameObject selectionPrefab;

	private bool isPaused;
	private List<PlayerController> players;

	public const int everythingMask = 0x7FFFFFFF; 


	private void Start () {
		if (instance == null || instance == this) {
			instance = this;
		}
		else {
			Destroy(gameObject);

			return;
		}

		DontDestroyOnLoad(gameObject);

		players = new List<PlayerController>();

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

	public void RegisterPlayer(PlayerController playerControl)
    {
		players.Add(playerControl);
    }

	public void Pause()
    {
		isPaused = true;
		foreach(PlayerController player in players)
        {
			player.SetControlsActivated(false);
        }
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
		bool hitGround = Physics.Raycast(new Vector3(xPos, pos.y + 1, zPos), Vector3.down, out hit, 1.5f, LayerMask.GetMask("Terrain"));

		if (hitGround) {
			yPos = hit.point.y;
		}

		return new Vector3(xPos, yPos, zPos);
	}

	// Returns the position at the same x and z as the given pos but, level with the ground
	public static Vector3 GetGroundLevelPos (Vector3 pos) {
		float yPos = pos.y;
		RaycastHit hit;
		bool hitGround = Physics.Raycast(new Vector3(pos.x, pos.y + 1, pos.z), Vector3.down, out hit, 1.5f, LayerMask.GetMask("Terrain"));

		if (hitGround) {
			yPos = hit.point.y;
		}

		return new Vector3(pos.x, yPos, pos.z);
	}

	public static Vector3 GetTopFromSide (Vector3 pos, Vector3 normal) {
		Vector3 newPos = pos + (normal * -0.5f) + new Vector3(0, 0.5f, 0);

		return GetGridPos(newPos);
	}
}












