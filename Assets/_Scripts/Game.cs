using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

	public static Game instance;

	public TileSet testTileSet;
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