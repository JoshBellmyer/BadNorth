using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Game : MonoBehaviour {

	public static Game instance;

	[Header("Tile Sets")]
	public TileSet testTileSet;
	public TileSet tileSet1;

	public GameObject[] otherMeshes;

	private bool isPaused;
	private List<PlayerController> players;


	private void Start () {
		if (instance == null || instance == this) {
			instance = this;
		}
		else {
			Destroy(gameObject);
		}

		DontDestroyOnLoad(gameObject);

		players = new List<PlayerController>();
	}

	public bool IsPlayerRegistered(PlayerController player)
    {
		return players.Contains(player);
    }

	public void RegisterPlayer(PlayerController playerControl)
    {
		players.Add(playerControl);
    }

	public void Pause(PlayerController playerControl)
    {
		isPaused = true;
		foreach(PlayerController player in players)
        {
			player.SetControlsActivated(false);
        }
		playerControl.SetControlsActivated(true);
		playerControl.SetActionMap("UI");

		UIManager.instance.Pause();
    }

	public void Unpause()
    {
		isPaused = false;
		foreach (PlayerController player in players)
		{
			player.SetControlsActivated(true);
			player.SetActionMap("Player");
		}

		UIManager.instance.Unpause();
	}
}