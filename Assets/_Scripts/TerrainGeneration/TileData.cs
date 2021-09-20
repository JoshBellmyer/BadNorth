using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileData {

	public TileType[,,] tileTypes;
	public List<TileLocation> tileLocations;
	public int sizeX;
	public int sizeY;
	public int sizeZ;

	private float[,] noise;
	private float meshScale;
	private int[,] tileHeights;

	private static Dictionary<TileType, int[]> tileEdges;
	

	public TileData (float[,] _noise, float _meshScale) {
		noise = _noise;
		meshScale = _meshScale;

		sizeX = noise.GetLength(0);
		sizeZ = noise.GetLength(1);
		sizeY = (int)meshScale + 2;

		tileTypes = new TileType[sizeX, sizeY, sizeZ];
		tileLocations = new List<TileLocation>();
		tileHeights = new int[sizeX, sizeZ];

		// Place initial cube-shaped tiles
		for (int x = 0; x < sizeX; x++) {
			for (int z = 0; z < sizeZ; z++) {
				int height = (int)Mathf.Round(noise[x, z] * meshScale);
				tileHeights[x, z] = height;

				tileTypes[x, height, z] = TileType.Cube;
				tileLocations.Add( new TileLocation(TileType.Cube, new Vector3Int(x, height, z)) );
			}
		}

		EnhanceTiles();
	}


	// Enhances the terrain by placing slope tiles and some additional cube-shaped tiles
	private void EnhanceTiles () {
		for (int x = 0; x < sizeX; x++) {
			for (int z = 0; z < sizeZ; z++) {
				PlaceSlope(x, z);

				int height = tileHeights[x, z] - 1;
				bool placedFinal = false;

				// Place additional cubes
				while (height >= 0 && !placedFinal) {
					int neighbors = GetNeighborBelow(x, height, z);

					tileTypes[x, height, z] = TileType.Cube;
					tileLocations.Add( new TileLocation(TileType.Cube, new Vector3Int(x, height, z)) );

					if (neighbors == 0b1111) {
						placedFinal = true;
					}

					height--;
				}
			}
		}
	}

	// Places a slope at the given location if it is valid
	private void PlaceSlope (int x, int z) {
		int height = tileHeights[x, z] + 1;
		int neighbors = GetNeighborDifference(x, height, z, 1);
		TileType type = TileType.None;
		int upChance = 0;
		int downChance = 0;
		int leftChance = 0;
		int rightChance = 0;

		switch (neighbors) {
			case 0b1000:
				upChance = 5;
			break;

			case 0b0100:
				downChance = 5;
			break;

			case 0b0010:
				leftChance = 5;
			break;

			case 0b0001:
				rightChance = 5;
			break;

			case 0b1010:
				upChance = 10;
				leftChance = 10;
			break;

			case 0b1001:
				upChance = 10;
				rightChance = 10;
			break;

			case 0b0110:
				downChance = 10;
				leftChance = 10;
			break;

			case 0b0101:
				downChance = 10;
				rightChance = 10;
			break;
		}

		int rand = Random.Range(0, 100);
		bool placed = false;

		if (rand < upChance && upChance > 0) {
			tileTypes[x, height, z] = TileType.FullRampU;
			tileLocations.Add( new TileLocation(TileType.FullRampU, new Vector3Int(x, height, z)) );

			return;
		}

		rand -= upChance;

		if (rand < downChance && downChance > 0) {
			tileTypes[x, height, z] = TileType.FullRampD;
			tileLocations.Add( new TileLocation(TileType.FullRampD, new Vector3Int(x, height, z)) );

			return;
		}

		rand -= downChance;

		if (rand < leftChance && leftChance > 0) {
			tileTypes[x, height, z] = TileType.FullRampL;
			tileLocations.Add( new TileLocation(TileType.FullRampL, new Vector3Int(x, height, z)) );

			return;
		}

		rand -= leftChance;

		if (rand < rightChance && rightChance > 0) {
			tileTypes[x, height, z] = TileType.FullRampR;
			tileLocations.Add( new TileLocation(TileType.FullRampR, new Vector3Int(x, height, z)) );

			return;
		}
	}

	// Returns an int representing whether the neighboring heights in each direction are lower
	private int GetNeighborBelow (int x, int height, int z) {
		int front = 0b0000;
		int back = 0b0000;
		int left = 0b0000;
		int right = 0b0000;

		if (z < sizeZ - 1) {
			front = (height <= tileHeights[x, z + 1]) ? 0b1000 : 0b0000;
		}
		if (z > 0) {
			back = (height <= tileHeights[x, z - 1]) ? 0b0100 : 0b0000;
		}
		if (x > 0) {
			left = (height <= tileHeights[x - 1, z]) ? 0b0010 : 0b0000;
		}
		if (x < sizeX - 1) {
			right = (height <= tileHeights[x + 1, z]) ? 0b0001 : 0b0000;
		}

		return (front | back | left | right);
	}

	// Returns an int representing whether the neighboring heights in each direction have the given height difference
	private int GetNeighborDifference (int x, int height, int z, int difference) {
		int front = 0b0000;
		int back = 0b0000;
		int left = 0b0000;
		int right = 0b0000;

		if (z < sizeZ - 1) {
			front = (tileHeights[x, z + 1] - height == difference) ? 0b1000 : 0b0000;
		}
		if (z > 0) {
			back = (tileHeights[x, z - 1] - height == difference) ? 0b0100 : 0b0000;
		}
		if (x > 0) {
			left = (tileHeights[x - 1, z] - height == difference) ? 0b0010 : 0b0000;
		}
		if (x < sizeX - 1) {
			right = (tileHeights[x + 1, z] - height == difference) ? 0b0001 : 0b0000;
		}

		return (front | back | left | right);
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
	None = 0,
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




















