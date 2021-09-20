using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePlacer : MonoBehaviour {

	private static Dictionary<string, int> tileEdges;
	private static TileData tileData;
	private static TileSet tileSet;
	private static CombineInstance[] cInstances;
	private static List<GameObject> tempTiles;
	private static Mesh mesh;
	private static int cIndex;


	// Places tiles based on the given tileData and tileSet, and combines them into a single mesh
	public static Mesh PlaceTiles (TileData _tileData, TileSet _tileSet) {
		tileData = _tileData;
		tileSet = _tileSet;

		if (tileEdges == null) {
			InitializeEdges();
		}

		mesh = new Mesh();
		tempTiles = new List<GameObject>();
		cInstances = new CombineInstance[tileData.tileLocations.Count];
		cIndex = 0;

		foreach (TileLocation tileLoc in tileData.tileLocations) {
			switch (tileLoc.type) {
				case TileType.Cube:
					PlaceCube(tileLoc.pos.x, tileLoc.pos.y, tileLoc.pos.z);
				break;

				case TileType.FullRampU:
					PlaceSlope(tileLoc.pos.x, tileLoc.pos.y, tileLoc.pos.z, 0);
				break;

				case TileType.FullRampD:
					PlaceSlope(tileLoc.pos.x, tileLoc.pos.y, tileLoc.pos.z, 2);
				break;

				case TileType.FullRampL:
					PlaceSlope(tileLoc.pos.x, tileLoc.pos.y, tileLoc.pos.z, 3);
				break;

				case TileType.FullRampR:
					PlaceSlope(tileLoc.pos.x, tileLoc.pos.y, tileLoc.pos.z, 1);
				break;
			}
		}

		mesh.CombineMeshes(cInstances);

		foreach (GameObject obj in tempTiles) {
			Destroy(obj);
		}

		return mesh;
	}

	// Places a cube-shaped tile at the given x, y, z coords
	private static void PlaceCube (int x, int y, int z) {
		Vector3Int pos = new Vector3Int(x, y, z);
		int front = GetEdge(pos, 0, 0)[0] ? 0b1000 : 0b0000;
		int back = GetEdge(pos, 1, 0)[0] ? 0b0100 : 0b0000;
		int left = GetEdge(pos, 2, 0)[0] ? 0b0010 : 0b0000;
		int right = GetEdge(pos, 3, 0)[0] ? 0b0001 : 0b0000;

		int edgeBool = (front | back | left | right);
		string[] index = new string[4];
		int tileIndex = -1;
		int rotation = 0;

		index[0] = $"{edgeBool},{0b0000},{0b0000}";
		index[1] = $"{RotateEdges(edgeBool, 1)},{0b0000},{0b0000}";
		index[2] = $"{RotateEdges(edgeBool, 2)},{0b0000},{0b0000}";
		index[3] = $"{RotateEdges(edgeBool, 3)},{0b0000},{0b0000}";

		if (tileEdges.ContainsKey(index[0])) {
			tileIndex = tileEdges[index[0]];
		}
		else if (tileEdges.ContainsKey(index[1])) {
			tileIndex = tileEdges[index[1]];
			rotation = 1;
		}
		else if (tileEdges.ContainsKey(index[2])) {
			tileIndex = tileEdges[index[2]];
			rotation = 2;
		}
		else if (tileEdges.ContainsKey(index[3])) {
			tileIndex = tileEdges[index[3]];
			rotation = 3;
		}

		if (tileIndex >= 0) {
			PlaceTile(x, y, z, rotation, tileIndex);
		}
	}

	// Places a full slope tile at the given
	private static void PlaceSlope (int x, int y, int z, int rotation) {
		Vector3Int pos = new Vector3Int(x, y, z);
		int front = GetEdge(pos, 0, 0)[0] ? 0b1000 : 0b0000;
		int back = GetEdge(pos, 1, 0)[0] ? 0b0100 : 0b0000;
		int left = GetEdge(pos, 2, 0)[0] ? 0b0010 : 0b0000;
		int right = GetEdge(pos, 3, 0)[0] ? 0b0001 : 0b0000;

		int frontR = GetEdge(pos, 0, 0)[2] ? 0b1000 : 0b0000;
		int backR = GetEdge(pos, 1, 0)[2] ? 0b0100 : 0b0000;
		int leftR = GetEdge(pos, 2, 0)[2] ? 0b0010 : 0b0000;
		int rightR = GetEdge(pos, 3, 0)[2] ? 0b0001 : 0b0000;

		front = (front | frontR);
		back = (back | backR);
		left = (left | leftR);
		right = (right | rightR);

		int edgeBool = (front | back | left | right);
		string index = $"{0b0000},{0b0000},{RotateEdges(edgeBool, rotation)}";

		if (tileEdges.ContainsKey(index)) {
			PlaceTile(x, y, z, rotation, tileEdges[index]);
		}
	}


	// Places a tile and adds it to the mesh
	private static void PlaceTile (int x, int y, int z, int rotation, int tileIndex) {
		GameObject tileObject = (GameObject)Instantiate(tileSet.models[tileIndex].RandomVariation(20));
		tempTiles.Add(tileObject);

		tileObject.transform.position = new Vector3(x, y, z);
		tileObject.transform.eulerAngles = new Vector3(0, 90 * rotation, 0);

		GameObject meshObject = tileObject.transform.GetChild(0).gameObject;

		cInstances[cIndex] = new CombineInstance();
		cInstances[cIndex].mesh = meshObject.GetComponent<MeshFilter>().mesh;
		cInstances[cIndex].transform = meshObject.transform.localToWorldMatrix;
		cIndex++;
	}

	// Returns the edge data from the given pos in the given direction
	private static bool[] GetEdge (Vector3Int pos, int direction, int type) {
		Vector3Int newPos = pos + GetDirectionVector(direction);

		if (newPos.x < 0 || newPos.x >= tileData.sizeX) {
			return new bool[] {false, false, false};
		}
		if (newPos.z < 0 || newPos.z >= tileData.sizeZ) {
			return new bool[] {false, false, false};
		}

		bool[] results = new bool[3];
		int[] edges = TileData.GetEdges(tileData.tileTypes[newPos.x, newPos.y, newPos.z]);
		int opDir = GetOppositeDirection(direction);
		int mask = (0b1000 >> opDir);

		results[0] = (edges[0] & mask) != 0;
		results[1] = (edges[1] & mask) != 0;
		results[2] = (edges[2] & mask) != 0;

		return results;
	}

	// Returns an integer vector corresponding to the direction that the given integer represents
	private static Vector3Int GetDirectionVector (int direction) {
		Vector3Int[] vectors = new Vector3Int[] {
			new Vector3Int(0, 0, 1),
			new Vector3Int(0, 0, -1),
			new Vector3Int(-1, 0, 0),
			new Vector3Int(1, 0, 0),
		};

		return vectors[direction];
	}

	// Returns an integer representing the opposite direction of the given direction
	// 0 represents positive Z
	// 1 represents negative Z
	// 2 represents negative X
	// 3 represents positive X
	private static int GetOppositeDirection (int direction) {
		int newDir = 0;

		switch (direction) {
			case 0:
				newDir = 1;
			break;

			case 1:
				newDir = 0;
			break;

			case 2:
				newDir = 3;
			break;

			case 3:
				newDir = 2;
			break;
		}

		return newDir;
	}

	// Rotates the given tile edge data by [90 degrees times the given amount]
	private static int RotateEdges (int edges, int amount) {
		int newEdges = edges;

		for (int i = 0; i < amount; i++) {
			int front = (newEdges & 0b0001) != 0 ? 0b1000 : 0b0000;
			int back = (newEdges & 0b0010) != 0 ? 0b0100 : 0b0000;
			int left = (newEdges & 0b1000) != 0 ? 0b0010 : 0b0000;
			int right = (newEdges & 0b0100) != 0 ? 0b0001 : 0b0000;

			newEdges = (front | back | left | right);
		}

		return newEdges;
	}


	// Initializes the lookup table for tile edges
	private static void InitializeEdges () {
		tileEdges = new Dictionary<string, int>();

		// Cubes
		tileEdges.Add($"{0b1111},{0b0000},{0b0000}", 0);
		tileEdges.Add($"{0b1011},{0b0000},{0b0000}", 1);
		tileEdges.Add($"{0b1001},{0b0000},{0b0000}", 2);
		tileEdges.Add($"{0b1100},{0b0000},{0b0000}", 3);
		tileEdges.Add($"{0b1000},{0b0000},{0b0000}", 4);
		tileEdges.Add($"{0b0000},{0b0000},{0b0000}", 5);

		// Ramps
		tileEdges.Add($"{0b0000},{0b0000},{0b1011}", 6);
		tileEdges.Add($"{0b0000},{0b0000},{0b1001}", 7);
		tileEdges.Add($"{0b0000},{0b0000},{0b1010}", 8);
		tileEdges.Add($"{0b0000},{0b0000},{0b1000}", 9);
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



















