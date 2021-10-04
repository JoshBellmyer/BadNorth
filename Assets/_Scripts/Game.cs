using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

	public static Game instance;

	[Header("Tile Sets")]
	public TileSet testTileSet;
	public TileSet tileSet1;

	public GameObject[] otherMeshes;
	public int numPlayers;


	private void Start () {
		if (instance == null || instance == this) {
			instance = this;
		}
		else {
			Destroy(gameObject);
		}

		DontDestroyOnLoad(gameObject);
	}
}