using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSetLoader {

	// Loads a tileset based on index (starting from 0)
	public static TileSet LoadTileSet (int index) {
		TileSet tileSet = Resources.Load<TileSet>($"TileSets/TileSet {index + 1}");

		return tileSet;
	}

	// Loads all tilesets and returns them in a List
	public static List<TileSet> LoadAllTileSets () {
		int length = 0;
		TileSet tileSet = LoadTileSet(length);
		List<TileSet> tileSets = new List<TileSet>();

		while (tileSet != null) {
			tileSets.Add(tileSet);
			length++;
			tileSet = LoadTileSet(length);
		}

		return tileSets;
	}

	// Returns an array containing the names of all tilesets
	//  This has to load all of the tilesets anyways so it might not be super useful.
	public static string[] GetTileSetNames () {
		List<TileSet> tileSets = LoadAllTileSets();

		string[] names = new string[tileSets.Count];

		for (int i = 0; i < names.Length; i++) {
			names[i] = tileSets[i].setName;
		}

		return names;
	}
}