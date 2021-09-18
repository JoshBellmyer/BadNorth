using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileData {

	public TileType[,,] tileTypes;
	public List<TileLocation> tileLocations;
	public int sizeX;
	public int sizeY;
	public int sizeZ;

	private static Dictionary<TileType, int[]> tileEdges;
	

	public TileData (float[,] noise, float meshScale) {
		sizeX = noise.GetLength(0);
		sizeZ = noise.GetLength(1);
		sizeY = (int)meshScale + 2;

		tileTypes = new TileType[sizeX, sizeY, sizeZ];
		tileLocations = new List<TileLocation>();

		for (int x = 0; x < sizeX; x++) {
			for (int z = 0; z < sizeZ; z++) {
				int height = (int)Mathf.Round(noise[x, z] * meshScale);

				tileTypes[x, height, z] = TileType.Cube;
				tileLocations.Add( new TileLocation(TileType.Cube, new Vector3Int(x, height, z)) );

				int temp = height - 1;

				while (temp >= 0) {
					tileTypes[x, temp, z] = TileType.Cube;
					tileLocations.Add( new TileLocation(TileType.Cube, new Vector3Int(x, temp, z)) );
					temp--;
				}
			}
		}
	}

	// Returns the edges of the given tile type
	public static int[] GetEdges (TileType type) {
		if (tileEdges == null) {
			InitializeEdges();
		}

		if (!tileEdges.ContainsKey(type)) {
			return new int[] {0b0000, 0b0000, 0b0000};
		}

		return tileEdges[type];
	}

	// ^ Initializes the edges of the different tile types
	private static void InitializeEdges () {
		tileEdges = new Dictionary<TileType, int[]>();

		tileEdges.Add(TileType.Cube, new int[] {0b1111, 0b0000, 0b0000});

		tileEdges.Add(TileType.FullRampU, new int[] {0b1000, 0b0000, 0b0011});
		tileEdges.Add(TileType.FullRampD, new int[] {0b0100, 0b0000, 0b0011});
		tileEdges.Add(TileType.FullRampL, new int[] {0b0010, 0b0000, 0b1100});
		tileEdges.Add(TileType.FullRampR, new int[] {0b0001, 0b0000, 0b1100});

		tileEdges.Add(TileType.HalfRampU, new int[] {0b0000, 0b1000, 0b0011});
		tileEdges.Add(TileType.HalfRampD, new int[] {0b0000, 0b0100, 0b0011});
		tileEdges.Add(TileType.HalfRampL, new int[] {0b0000, 0b0010, 0b1100});
		tileEdges.Add(TileType.HalfRampR, new int[] {0b0000, 0b0001, 0b1100});

		tileEdges.Add(TileType.RaisedRampU, new int[] {0b1000, 0b0100, 0b0011});
		tileEdges.Add(TileType.RaisedRampD, new int[] {0b0100, 0b1000, 0b0011});
		tileEdges.Add(TileType.RaisedRampL, new int[] {0b0010, 0b0001, 0b1100});
		tileEdges.Add(TileType.RaisedRampR, new int[] {0b0001, 0b0010, 0b1100});
	}
}


public class TileLocation {

	public TileType type;
	public Vector3Int pos;

	public TileLocation (TileType type, Vector3Int pos) {
		this.type = type;
		this.pos = pos;
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














