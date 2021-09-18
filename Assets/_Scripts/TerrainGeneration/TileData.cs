using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileData {

	public TileType[,,] tileTypes;

	private static Dictionary<TileType, int[]> tileEdges;
	

	public TileData (float[,] noise, float meshScale) {
		int sizeX = noise.GetLength(0);
		int sizeZ = noise.GetLength(1);
		int sizeY = (int)Mathf.Floor(noise.GetLength(0) / 2.0f);

		tileTypes = new TileType[sizeX, sizeY, sizeZ];

		for (int x = 0; x < sizeX; x++) {
			for (int z = 0; z < sizeZ; z++) {
				int height = (int)Mathf.Round(noise[x, z] * meshScale);

				tileTypes[x, height, z] = TileType.Cube;
			}
		}
	}

	// Returns the edges of the given tile type
	public static int[] GetEdges (TileType type) {
		if (tileEdges == null) {
			InitializeEdges();
		}

		return tileEdges[type];
	}

	// ^ Initializes the edges of the different tile types
	private static void InitializeEdges () {
		tileEdges = new Dictionary<TileType, int[]>();

		tileEdges.Add(TileType.Cube, new int[] {0b1111, 0b0000});

		tileEdges.Add(TileType.FullRampU, new int[] {0b1000, 0b0000, 0b0000});
		tileEdges.Add(TileType.FullRampD, new int[] {0b0100, 0b0000, 0b0000});
		tileEdges.Add(TileType.FullRampL, new int[] {0b0010, 0b0000, 0b0000});
		tileEdges.Add(TileType.FullRampR, new int[] {0b0001, 0b0000, 0b0000});

		tileEdges.Add(TileType.HalfRampU, new int[] {0b0000, 0b1000});
		tileEdges.Add(TileType.HalfRampD, new int[] {0b0000, 0b0100});
		tileEdges.Add(TileType.HalfRampL, new int[] {0b0000, 0b0010});
		tileEdges.Add(TileType.HalfRampR, new int[] {0b0000, 0b0001});

		tileEdges.Add(TileType.RaisedRampU, new int[] {0b1000, 0b0100});
		tileEdges.Add(TileType.RaisedRampD, new int[] {0b0100, 0b1000});
		tileEdges.Add(TileType.RaisedRampL, new int[] {0b0010, 0b0001});
		tileEdges.Add(TileType.RaisedRampR, new int[] {0b0001, 0b0010});
	}
}


public enum TileType {
	Cube = 1,

	FullRampU = 2,
	FullRampD = 3,
	FullRampL = 4,
	FullRampR = 5,

	HalfRampU = 6,
	HalfRampD = 7,
	HalfRampL = 8,
	HalfRampR = 9,

	RaisedRampU = 10,
	RaisedRampD = 11,
	RaisedRampL = 12,
	RaisedRampR = 13,
}














