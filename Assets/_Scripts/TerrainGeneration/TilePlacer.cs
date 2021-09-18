using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePlacer : MonoBehaviour {

	public static void PlaceTiles (TileData tileData) {

	}
}


[System.Serializable]
public class TileSet {

	public string setName;
	public TileGroup[] models;
	public Material material;
}


[System.Serializable]
public class TileGroup {

	public GameObject[] variations;

	public GameObject RandomVariation (int varChance) {
		int chance = Random.Range(0, 100);

		if (chance < varChance && variations.Length > 1) {
			int rand = Random.Range(0, variations.Length - 1);

			return variations[rand + 1];
		}

		return variations[0];
	}
}














