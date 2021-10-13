using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Game : MonoBehaviour {

	public static Game instance;
	public static MapGenerator mapGenerator;

	[Header("Prefabs")]
	[SerializeField] private Boat boatPrefab;

	private bool isPaused;
	private List<PlayerController> players;


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

		Boat boat = Instantiate<Boat>(boatPrefab);
		boat.SetPlayer(playerControl);
    }

	public void Pause(PlayerController playerControl)
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
}