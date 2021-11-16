using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileData {

	public TileType[,,] tileTypes;
	public List<TileLocation> tileLocations;
	public int sizeX;
	public int sizeY;
	public int sizeZ;
	public int slopeCount;
	public int maxHeight;

	private float[,] noise;
	private float meshScale;
	private int[,] tileHeights;

	private static Dictionary<TileType, int[]> tileEdges;
	private List<string> tempSlopes;
	

	public TileData (float[,] _noise, float _meshScale) {
		noise = _noise;
		meshScale = _meshScale;

		sizeX = noise.GetLength(0);
		sizeZ = noise.GetLength(1);
		sizeY = (int)meshScale + 5;
		maxHeight = 0;

		tileTypes = new TileType[sizeX, sizeY, sizeZ];
		tileLocations = new List<TileLocation>();
		tileHeights = new int[sizeX, sizeZ];

		// Place initial cube-shaped tiles
		for (int x = 0; x < sizeX; x++) {
			for (int z = 0; z < sizeZ; z++) {
				int height = (int)Mathf.Round(noise[x, z] * meshScale);
				tileHeights[x, z] = height;

				if (height > maxHeight) {
					maxHeight = height;
				}

				if (height > 0) {
					tileTypes[x, height, z] = TileType.Cube;
					tileLocations.Add( new TileLocation(TileType.Cube, new Vector3Int(x, height, z)) );
				}
			}
		}

		EnhanceTiles();
	}


	// Enhances the terrain by placing slope tiles and some additional cube-shaped tiles
	private void EnhanceTiles () {
		tempSlopes = new List<string>();

		for (int x = 0; x < sizeX; x++) {
			for (int z = 0; z < sizeZ; z++) {
				PlaceSlope(x, z);

				int height = tileHeights[x, z] - 1;
				bool placedFinal = false;

				// Place additional cubes
				while (height >= 0 && !placedFinal) {
					int neighbors = GetNeighborBelow(x, height + 1, z);

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

		if (height < 2) {
			return;
		}

		int neighbors = GetNeighborDifference(x, height, z, 0);
		int neighbors2 = GetNeighborDifference(x, height, z, -1);
		bool longSlope = false;
		int upChance = 0;
		int downChance = 0;
		int leftChance = 0;
		int rightChance = 0;

		int randSlope = Random.Range(0, 10);

		// neighbors = (neighbors << 4) | neighbors2;

		switch (neighbors) {
			case 0b1000:
				upChance = 15;
			break;

			case 0b0100:
				downChance = 15;
			break;

			case 0b0010:
				leftChance = 15;
			break;

			case 0b0001:
				rightChance = 15;
			break;

			case 0b1010:
				upChance = 40;
				leftChance = 40;
				longSlope = (randSlope < 50);
			break;

			case 0b1001:
				upChance = 40;
				rightChance = 40;
				longSlope = (randSlope < 50);
			break;

			case 0b0110:
				downChance = 40;
				leftChance = 40;
				longSlope = (randSlope < 50);
			break;

			case 0b0101:
				downChance = 40;
				rightChance = 40;
				longSlope = (randSlope < 50);
			break;
		}

		int rand = Random.Range(0, 100);

		if (rand < upChance && upChance > 0 && (neighbors2 & 0b0100) > 0) {
			if (longSlope) {
				PlaceLongSlope(x, height, z, new Vector3Int(0, 0, -1), 0b0100, TileType.FullRampU);
			}
			else {
				PlaceFullSlope(x, height, z, TileType.FullRampU);
			}

			return;
		}

		rand -= upChance;

		if (rand < downChance && downChance > 0 && (neighbors2 & 0b1000) > 0) {
			if (longSlope) {
				PlaceLongSlope(x, height, z, new Vector3Int(0, 0, 1), 0b1000, TileType.FullRampD);
			}
			else {
				PlaceFullSlope(x, height, z, TileType.FullRampD);
			}

			return;
		}

		rand -= downChance;

		if (rand < leftChance && leftChance > 0 && (neighbors2 & 0b0001) > 0) {
			if (longSlope) {
				PlaceLongSlope(x, height, z, new Vector3Int(1, 0, 0), 0b0001, TileType.FullRampL);
			}
			else {
				PlaceFullSlope(x, height, z, TileType.FullRampL);
			}

			return;
		}

		rand -= leftChance;

		if (rand < rightChance && rightChance > 0 && (neighbors2 & 0b0010) > 0) {
			if (longSlope) {
				PlaceLongSlope(x, height, z, new Vector3Int(-1, 0, 0), 0b0010, TileType.FullRampR);
			}
			else {
				PlaceFullSlope(x, height, z, TileType.FullRampR);
			}

			return;
		}
	}

	// Places a 45 degree slope
	private void PlaceFullSlope (int x, int y, int z, TileType type) {
		if (tempSlopes.Contains($"{new Vector3Int(x, y, z)}")) {
			return;
		}

		tempSlopes.Add($"{new Vector3Int(x, y, z)}");

		tileTypes[x, y, z] = type;
		tileLocations.Add( new TileLocation(type, new Vector3Int(x, y, z)) );

		slopeCount++;
	}

	// Places a long 22.5 degree slope if it fits, otherwise places a 45 degree slope
	private void PlaceLongSlope (int x, int y, int z, Vector3Int backDir, int mask, TileType type) {
		if (tempSlopes.Contains($"{new Vector3Int(x, y, z)}")) {
			return;
		}
		if (tempSlopes.Contains($"{new Vector3Int(x + backDir.x, y, z + backDir.z)}")) {
			return;
		}

		int neighbors = GetNeighborDifference(x + backDir.x, y, z + backDir.z, -1);

		if (tileTypes[x + backDir.x, y, z + backDir.z] != TileType.None) {
			return;
		}
		if ((neighbors & mask) == 0) {
			PlaceFullSlope(x, y, z, type);

			return;
		}

		tempSlopes.Add($"{new Vector3Int(x, y, z)}");
		tempSlopes.Add($"{new Vector3Int(x + backDir.x, y, z + backDir.z)}");

		int temp = (int)type - (int)TileType.FullRampU;
		TileType raisedType = (TileType)((int)TileType.RaisedRampU + temp);
		TileType halfType = (TileType)((int)TileType.HalfRampU + temp);

		tileTypes[x, y, z] = raisedType;
		tileLocations.Add( new TileLocation(raisedType, new Vector3Int(x, y, z)) );

		tileTypes[x + backDir.x, y, z + backDir.z] = halfType;
		tileLocations.Add( new TileLocation(halfType, new Vector3Int(x + backDir.x, y, z + backDir.z)) );

		slopeCount += 2;
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
			return new int[] {0b0000, 0b0000, 0b0000, 0b0000, 0b0000};
		}

		return tileEdges[type];
	}

	// ^ Initializes the edges of the different tile types
	// {FullBlock,  FullRampSide, RaisedRampSide, HalfRampSide, HalfBlock}
	private static void InitializeEdges () {
		tileEdges = new Dictionary<TileType, int[]>();

		tileEdges.Add(TileType.Cube, new int[] {0b1111, 0b0000, 0b0000, 0b0000, 0b0000});

		tileEdges.Add(TileType.FullRampU, new int[] {0b1000, 0b0011, 0b0000, 0b0000, 0b0000});
		tileEdges.Add(TileType.FullRampD, new int[] {0b0100, 0b0011, 0b0000, 0b0000, 0b0000});
		tileEdges.Add(TileType.FullRampL, new int[] {0b0010, 0b1100, 0b0000, 0b0000, 0b0000});
		tileEdges.Add(TileType.FullRampR, new int[] {0b0001, 0b1100, 0b0000, 0b0000, 0b0000});

		tileEdges.Add(TileType.HalfRampU, new int[] {0b0000, 0b0000, 0b0000, 0b0011, 0b1000});
		tileEdges.Add(TileType.HalfRampD, new int[] {0b0000, 0b0000, 0b0000, 0b0011, 0b0100});
		tileEdges.Add(TileType.HalfRampL, new int[] {0b0000, 0b0000, 0b0000, 0b1100, 0b0010});
		tileEdges.Add(TileType.HalfRampR, new int[] {0b0000, 0b0000, 0b0000, 0b1100, 0b0001});

		tileEdges.Add(TileType.RaisedRampU, new int[] {0b1000, 0b0000, 0b0011, 0b0000, 0b0100});
		tileEdges.Add(TileType.RaisedRampD, new int[] {0b0100, 0b0000, 0b0011, 0b0000, 0b1000});
		tileEdges.Add(TileType.RaisedRampL, new int[] {0b0010, 0b0000, 0b1100, 0b0000, 0b0001});
		tileEdges.Add(TileType.RaisedRampR, new int[] {0b0001, 0b0000, 0b1100, 0b0000, 0b0010});
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




















